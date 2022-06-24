using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services
{
    public class CalculatorService : ICalculatorService
    {
        //TODO, these need to be configurable or something
        public const double PartnerFraction = .2;
        public const double SublessFraction = .02;
        public readonly string SublessPayPalId;
        public const int CurrencyPrecision = 4;
        private readonly IStripeService _stripeService;
        private readonly IHitService _hitService;
        private readonly ICreatorService _creatorService;
        private readonly IPartnerService _partnerService;
        private readonly IPaymentLogsService _paymentLogsService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IUserService userService;
        private readonly ILogger _logger;

        public CalculatorService(
            IStripeService stripeService,
            IHitService hitService,
            ICreatorService creatorService,
            IPartnerService partnerService,
            IPaymentLogsService paymentLogsService,
            IUserService userService,
            IOptions<StripeConfig> stripeOptions,
            ILoggerFactory loggerFactory)
        {
            _stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
            _hitService = hitService ?? throw new ArgumentNullException(nameof(hitService));
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            _paymentLogsService = paymentLogsService ?? throw new ArgumentNullException(nameof(paymentLogsService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = _loggerFactory.CreateLogger<CalculatorService>();
            SublessPayPalId = stripeOptions.Value.SublessPayPalId ?? throw new ArgumentNullException(nameof(stripeOptions));
        }


        public CalculatorResult CaculatePayoutsOverRange(DateTimeOffset startDate, DateTimeOffset endDate, List<Guid> selectedUserIds = null)
        {
            var calculatorResult = new CalculatorResult();
            calculatorResult.EmailSent = false;
            // get what we were paid (after fees), and by who
            var payers = GetPayments(startDate, endDate);
            if (!payers.Any())
            {
                _logger.LogWarning("No Payments found in payment period, calculation skipped.");
                return null;
            }
            // for each user
            _logger.LogInformation("Preparing to process {0} payers' payments.", payers.Count());
            if (selectedUserIds != null)
            {
                _logger.LogWarning("Exclusing users from calculation!");
                _logger.LogWarning($"Only the following users are being evaluated \n {string.Join('\n', selectedUserIds)}");
                payers = payers.Where(x => selectedUserIds.Contains(x.UserId));
            }
            foreach (var payer in payers)
            {
                var payees = new List<Payee>();
                // get who they visited
                var hits = RetrieveUsersMonthlyHits(payer.UserId, startDate, endDate);
                // filter out incomplete creators
                _logger.LogInformation("We have {0} total hits of any kind from this user", hits.Count());
                hits = FilterInvalidCreators(hits);
                var user = userService.GetUser(payer.UserId);
                if (!hits.Any())
                {
                    calculatorResult.IdleCustomerStripeIds.Add(user.StripeCustomerId);
                    continue;
                }
                // group all visits to payee
                var creatorVisits = GetVisitsPerCreator(hits);
                // get partner share
                var partnerVisits = GetVisitsPerPartner(hits);
                // fraction each creator by the percentage of total visits
                // multiply payment by that fraction
                _logger.LogInformation($"Found {0} creators visited and {1} partners visited for a payer.", creatorVisits.Count, partnerVisits.Count);
                payees.AddRange(GetCreatorPayees(payer.Payment, creatorVisits, hits.Count(), PartnerFraction, SublessFraction));
                payees.AddRange(GetPartnerPayees(payer.Payment, creatorVisits, hits.Count(), PartnerFraction, SublessFraction));
                // set aside 2% for us
                payees.Add(GetSublessPayment(payer.Payment, SublessFraction));
                // ensure total payment adds up
                var totalPayments = Math.Round(payees.Sum(payee => payee.Payment), CurrencyPrecision, MidpointRounding.ToZero);
                if (totalPayments > payer.Payment)
                {
                    throw new ArithmeticException($"The math did not add up for payer:{payer.UserId}");
                }

                // record each outgoing payment to master list
                var payments = CollectPaymentDetails(payees, payer, endDate);
                // if someone has paid twice (cancelled and resubbed), we should just combine their payments
                if (calculatorResult.PaymentsPerPayer.ContainsKey(user.CognitoId))
                {
                    calculatorResult.PaymentsPerPayer[user.CognitoId].AddRange(payments);
                }
                else
                {
                    calculatorResult.PaymentsPerPayer.Add(user.CognitoId, payments);
                }
                AddPayeesToMasterList(calculatorResult.AllPayouts, payees, startDate, endDate);
            }
            DeductPaypalFees(calculatorResult.AllPayouts);
            // stripe sends payments in cents, paypal expects payouts in dollars
            ConvertCentsToDollars(calculatorResult.AllPayouts);
            // make sure we're not sending inappropriate fractions
            RoundPaymentsForFinalPayment(calculatorResult.AllPayouts);
            return calculatorResult;
        }

        private IEnumerable<Hit> FilterInvalidCreators(IEnumerable<Hit> hits)
        {
            var creatorIds = hits.Select(x => x.CreatorId).Distinct();
            _logger.LogInformation("Filter step 1: we have {0} unique creator IDs", creatorIds.Count());
            var creators = creatorIds.Select(x => _creatorService.GetCreator(x)).Where(x => x is not null);
            _logger.LogInformation("Filter step 2: we have {0} unique creators based on those IDs", creators.Count());
            var missingCreators = creatorIds.Where(x => !creators.Any(y => y.Id == x));
            _logger.LogInformation("Filter step 2: we have {0} missing creators", missingCreators.Count());
            var invalidCreators = creators.Where(x => x.ActivationCode is not null || string.IsNullOrWhiteSpace(x.PayPalId));
            _logger.LogInformation("Filter step 3: we have {0} invalid creators", invalidCreators.Count());
            var validHits = hits.Where(x => !missingCreators.Any(y => y == x.CreatorId) && !invalidCreators.Any(y => y.Id == x.CreatorId));
            _logger.LogInformation("Filter step 4: we have {0} valid hits", validHits.Count());
            return validHits;
        }

        private IEnumerable<Payer> GetPayments(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            _logger.LogDebug($"Searching in range {startDate} to end date {endDate}");
            var payers = _stripeService.GetPayersForRange(startDate, endDate);
            return payers;
        }

        private void RoundPaymentsForFinalPayment(List<PaymentAuditLog> masterPayoutList)
        {
            foreach (var payout in masterPayoutList)
            {
                payout.Payment = Math.Round(payout.Payment, 2, MidpointRounding.ToZero);
                payout.Revenue = Math.Round(payout.Revenue, 2, MidpointRounding.ToZero);
                payout.Fees = Math.Round(payout.Fees, 2, MidpointRounding.ToZero);
            }
        }

        private Payee GetSublessPayment(double Payment, double sublessFraction)
        {
            return new Payee
            {
                Name = "Subless",
                Payment = Math.Round(Payment * sublessFraction, CurrencyPrecision, MidpointRounding.ToZero),
                PayPalId = SublessPayPalId,
                TargetId = Guid.Empty,
                PayeeType = PayeeType.Subless
            };
        }

        private IEnumerable<Hit> RetrieveUsersMonthlyHits(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return _hitService.GetHitsByDate(startDate, endDate, userId).ToList();
        }

        private Dictionary<Guid, int> GetVisitsPerCreator(IEnumerable<Hit> hits)
        {
            var visits = new Dictionary<Guid, int>();
            foreach (var hit in hits)
            {
                if (!visits.ContainsKey(hit.CreatorId))
                {
                    visits.Add(hit.CreatorId, 0);
                }
                visits[hit.CreatorId] += 1;
            }
            return visits;
        }

        private Dictionary<Guid, int> GetVisitsPerPartner(IEnumerable<Hit> hits)
        {
            var visits = new Dictionary<Guid, int>();
            foreach (var hit in hits)
            {
                if (!visits.ContainsKey(hit.PartnerId))
                {
                    visits.Add(hit.PartnerId, 0);
                }
                visits[hit.PartnerId] += 1;
            }
            return visits;
        }

        public IEnumerable<Payee> GetCreatorPayees(double payment, Dictionary<Guid, int> creatorHits, int totalHits, double partnerHitFraction, double sublessHitFraction)
        {
            var payees = new List<Payee>();
            foreach (var creatorVisits in creatorHits)
            {
                var creatorPayment = (double)creatorVisits.Value / totalHits * (payment * (1 - sublessHitFraction) * (1 - partnerHitFraction));
                creatorPayment = Math.Round(creatorPayment, CurrencyPrecision, MidpointRounding.ToZero);
                var creator = _creatorService.GetCreator(creatorVisits.Key);
                payees.Add(new Payee
                {
                    Name = creator.Username,
                    Payment = creatorPayment,
                    PayPalId = creator.PayPalId,
                    TargetId = creator.Id,
                    PayeeType = PayeeType.Creator
                });
            }

            _logger.LogInformation($"For a patron who visited {0} creators, we've found {1} creator payees.", creatorHits.Count, payees.Count);
            return payees;
        }

        public IEnumerable<Payee> GetPartnerPayees(double payment, Dictionary<Guid, int> creatorHits, int totalHits, double partnerHitFraction, double sublessHitFraction)
        {
            var payees = new List<Payee>();
            foreach (var creatorVisits in creatorHits)
            {
                var partnerPayment = partnerHitFraction * ((double)creatorVisits.Value / totalHits) * (payment * (1 - sublessHitFraction));
                partnerPayment = Math.Round(partnerPayment, CurrencyPrecision, MidpointRounding.ToZero);
                var creator = _creatorService.GetCreator(creatorVisits.Key);
                var partner = _partnerService.GetPartner(creator.PartnerId);
                if (payees.Any(x => x.PayPalId == partner.PayPalId))
                {
                    var payee = payees.FirstOrDefault(x => x.PayPalId == partner.PayPalId);
                    payee.Payment = Math.Round(payee.Payment + partnerPayment, CurrencyPrecision, MidpointRounding.ToZero);
                }
                else
                {
                    var payee = new Payee()
                    {
                        Name = partner.Sites.First().Host,
                        PayPalId = partner.PayPalId,
                        Payment = partnerPayment,
                        TargetId = partner.Id,
                        PayeeType = PayeeType.Partner
                    };
                    payees.Add(payee);
                }
            }

            _logger.LogInformation($"For a patron who visited {0} partners, we've found {1} partner payees.", creatorHits.Count, payees.Count);
            return payees;
        }

        private List<Payment> CollectPaymentDetails(IEnumerable<Payee> payees, Payer payer, DateTimeOffset endDate)
        {
            _logger.LogInformation($"Collecting payment details for one patron and {0} payees", payees.Count());
            var logs = new List<Payment>();
            foreach (var payee in payees)
            {
                logs.Add(new Payment
                {
                    Payee = payee,
                    Payer = payer,
                    DateSent = endDate,
                    Amount = payee.Payment
                });
            }
            return logs;
        }

        private void AddPayeesToMasterList(List<PaymentAuditLog> masterPayoutList, IEnumerable<Payee> payees, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var newPayees = 0;
            foreach (var payee in payees)
            {
                if (masterPayoutList.Any(x => x.PayPalId == payee.PayPalId))
                {
                    masterPayoutList.Single(x => x.PayPalId == payee.PayPalId).Revenue += payee.Payment;
                }
                else
                {
                    masterPayoutList.Add(new PaymentAuditLog()
                    {
                        TargetId = payee.TargetId,
                        PayeeType = payee.PayeeType,
                        Revenue = payee.Payment,
                        PayPalId = payee.PayPalId,
                        DatePaid = DateTimeOffset.UtcNow,
                        PaymentPeriodStart = startDate,
                        PaymentPeriodEnd = endDate
                    });
                    newPayees += 1;
                }
            }

            _logger.LogInformation($"Updated payeeMasterList with {0} payees, {1} of which were newly added.",
                payees.Count(), newPayees);
        }

        private void ConvertCentsToDollars(List<PaymentAuditLog> masterPayoutList)
        {
            foreach (var payout in masterPayoutList)
            {
                payout.Payment = payout.Payment / 100;
                payout.Revenue = payout.Revenue / 100;
                payout.Fees = payout.Fees / 100;
            }
        }

        private void DeductPaypalFees(List<PaymentAuditLog> masterPayoutList)
        {
            foreach (var payout in masterPayoutList)
            {
                payout.Payment = payout.Revenue / 1.02;
                payout.Fees = payout.Revenue - payout.Payment;
            }
        }
    }
}
