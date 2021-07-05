using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services;

namespace Subless.PayoutCalculator
{
    public class CalculatorService : ICalculatorService
    {
        //TODO, these need to be configurable or something
        public const double PartnerFraction = .2;
        public const double SublessFraction = .02;
        public readonly string SublessPayoneerId;
        public const int CurrencyPrecision = 2;
        private readonly IStripeService _stripeService;
        private readonly IHitService _hitService;
        private readonly ICreatorService _creatorService;
        private readonly IPartnerService _partnerService;
        private readonly IPaymentLogsService _paymentLogsService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFileStorageService _s3Service;
        private readonly ILogger _logger;

        public CalculatorService(
            IStripeService stripeService,
            IHitService hitService,
            ICreatorService creatorService,
            IPartnerService partnerService,
            IPaymentLogsService paymentLogsService,
            IFileStorageService s3Service,
            IOptions<StripeConfig> stripeOptions,
            ILoggerFactory loggerFactory)
        {
            _stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
            _hitService = hitService ?? throw new ArgumentNullException(nameof(hitService));
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            _paymentLogsService = paymentLogsService ?? throw new ArgumentNullException(nameof(paymentLogsService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _s3Service = s3Service ?? throw new ArgumentNullException(nameof(s3Service));
            _logger = _loggerFactory.CreateLogger<CalculatorService>();
            SublessPayoneerId = stripeOptions.Value.SublessPayoneerId ?? throw new ArgumentNullException(nameof(stripeOptions.Value.SublessPayoneerId));
        }

        public void CalculatePayments(DateTime startDate, DateTime endDate)
        {
            Dictionary<string, double> allPayouts = new Dictionary<string, double>();
            // get what we were paid (after fees), and by who
            var payers = GetPayments(startDate, endDate);
            // for each user
            foreach (var payer in payers)
            {
                var payees = new List<Payee>();
                // get who they visited
                var hits = RetrieveUsersMonthlyHits(payer.UserId, startDate, endDate);
                // group all visits to payee
                var creatorVisits = GetVisitsPerCreator(hits);
                // get partner share
                var partnerVisits = GetVisitsPerPartner(hits);
                // fraction each creator by the percentage of total visits
                // multiply payment by that fraction
                payees.AddRange(GetCreatorPayees(payer.Payment, creatorVisits, hits.Count(), PartnerFraction, SublessFraction));
                payees.AddRange(GetPartnerPayees(payer.Payment, creatorVisits, hits.Count(), PartnerFraction, SublessFraction));
                // set aside 2% for us
                payees.Add(GetSublessPayment(payer.Payment, SublessFraction));
                // ensure total payment adds up
                var totalPayments = payees.Sum(payee => payee.Payment);
                if (totalPayments > payer.Payment)
                {
                    throw new Exception($"The math did not add up for payer:{payer.UserId}");
                }
                // record each outgoing payment to master list
                SavePaymentDetails(payees, payer, endDate);
                AddPayeesToMasterList(allPayouts, payees);
            }
            // record to database
            SaveMasterList(allPayouts, endDate);
            // record to s3 bucket
            SavePayoutsToS3(allPayouts);
        }


        private IEnumerable<Payer> GetPayments(DateTime startDate, DateTime endDate)
        {
            return _stripeService.GetInvoicesForRange(startDate, endDate);
        }

        private Payee GetSublessPayment(double Payment, double sublessFraction)
        {
            return new Payee
            {
                Payment = Math.Round(Payment * sublessFraction, CurrencyPrecision, MidpointRounding.ToZero),
                PayoneerId = SublessPayoneerId
            };
        }

        private IEnumerable<Hit> RetrieveUsersMonthlyHits(Guid userId, DateTime startDate, DateTime endDate)
        {
            return _hitService.GetHitsByDate(startDate, endDate, userId);
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

        public IEnumerable<Payee> GetCreatorPayees(Double payment, Dictionary<Guid, int> creatorHits, int totalHits, double partnerHitFraction, double sublessHitFraction)
        {
            var payees = new List<Payee>();
            foreach (var creatorVisits in creatorHits)
            {
                var creatorPayment = ((double)creatorVisits.Value / totalHits) * (payment * (1 - sublessHitFraction) * (1 - partnerHitFraction));
                creatorPayment = Math.Round(creatorPayment, CurrencyPrecision, MidpointRounding.ToZero);
                var creator = _creatorService.GetCreator(creatorVisits.Key);
                payees.Add(new Payee
                {
                    Payment = creatorPayment,
                    PayoneerId = creator.PayoneerId
                });
            }
            return payees;
        }

        public IEnumerable<Payee> GetPartnerPayees(Double payment, Dictionary<Guid, int> creatorHits, int totalHits, double partnerHitFraction, double sublessHitFraction)
        {
            var payees = new List<Payee>();
            foreach (var creatorVisits in creatorHits)
            {
                var partnerPayment = (partnerHitFraction) * ((double)creatorVisits.Value / totalHits) * (payment * (1 - sublessHitFraction));
                partnerPayment = Math.Round(partnerPayment, CurrencyPrecision, MidpointRounding.ToZero);
                var creator = _creatorService.GetCreator(creatorVisits.Key);
                var partner = _partnerService.GetPartner(creator.PartnerId);
                payees.Add(new Payee
                {
                    Payment = partnerPayment,
                    PayoneerId = partner.PayoneerId
                });
            }
            return payees;
        }

        private void SavePaymentDetails(IEnumerable<Payee> payees, Payer payer, DateTime endDate)
        {
            var logs = new List<Payment>();
            foreach (var payee in payees)
            {
                logs.Add(new Payment
                {
                    Payee = payee,
                    Payer = payer,
                    DateSent = endDate
                });
            }
            _paymentLogsService.SaveLogs(logs);

        }

        private void AddPayeesToMasterList(Dictionary<string, double> masterPayoutList, IEnumerable<Payee> payees)
        {
            foreach (var payee in payees)
            {
                if (masterPayoutList.ContainsKey(payee.PayoneerId))
                {
                    masterPayoutList[payee.PayoneerId] += payee.Payment;
                }
                else
                {
                    masterPayoutList.Add(payee.PayoneerId, payee.Payment);
                }
            }
        }

        private void SaveMasterList(Dictionary<string, double> masterPayoutList, DateTime endDate)
        {
            var payments = masterPayoutList.Select(x => new PaymentAuditLog() { Payment = x.Value, PayoneerId = x.Key, DatePaid = DateTime.UtcNow });
            _paymentLogsService.SaveAuditLogs(payments);
        }

        private void SavePayoutsToS3(Dictionary<string, double> masterPayoutList)
        {
            _s3Service.WritePaymentsToCloudFileStore(masterPayoutList);
        }
    }
}
