using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subless.Models;

namespace Subless.PayoutCalculator
{
    public class CalculatorService
    {
        const double PartnerFraction = .2;
        const double SublessFraction = .02;
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
                payees.AddRange(GetCreatorPayees(creatorVisits, hits.Count, PartnerFraction, SublessFraction));
                payees.AddRange(GetPartnerPayees(creatorVisits, hits.Count, PartnerFraction, SublessFraction));
                // set aside 2% for us
                payees.Add(GetSublessPayment(payer.Payment, SublessFraction));
                // ensure total payment adds up
                var totalPayments = payees.Sum(payee => payee.Payment);
                if (totalPayments>= payer.Payment)
                {
                    throw new Exception($"The math did not add up for payer:{payer.UserId}");
                }
                // record each outgoing payment to master list
                SaveAuditRecords(payees, payer);
                AddPayeesToMasterList(allPayouts, payees);
            }
            // record to database
            SaveMasterList(allPayouts);
            // record to s3 bucket
            SavePayoutsToS3(allPayouts);
        }


        private List<Payer> GetPayments(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        private Payee GetSublessPayment(double Payment, double sublessFraction)
        {
            throw new NotImplementedException();
        }

        private List<Hit> RetrieveUsersMonthlyHits(Guid userId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        private Dictionary<Guid, int> GetVisitsPerCreator(List<Hit> hits)
        {
            throw new NotImplementedException();
        }

        private Dictionary<Guid, int> GetVisitsPerPartner(List<Hit> hits)
        {
            throw new NotImplementedException();
        }

        private List<Payee> GetCreatorPayees(Dictionary<Guid,int> creatorHits, int totalHits, double partnerHitFraction, double sublessHitFraction)
        {
            throw new NotImplementedException();
        }

        private List<Payee> GetPartnerPayees(Dictionary<Guid, int> creatorHits, int totalHits, double partnerHitFraction, double sublessHitFraction)
        {
            throw new NotImplementedException();
        }

        private void SaveAuditRecords(List<Payee> payees, Payer payer)
        {
            throw new NotImplementedException();
        }

        private void AddPayeesToMasterList(Dictionary<string, double> masterPayoutList, List<Payee> payees)
        {
            foreach(var payee in payees)
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

        private void SaveMasterList(Dictionary<string, double> masterPayoutList)
        {
            throw new NotImplementedException();
        }

        private void SavePayoutsToS3(Dictionary<string, double> masterPayoutList)
        {
            throw new NotImplementedException();
        }

        private class Payee
        {
            public double Payment { get; set; }
            public string PayoneerId { get; set; }
        }

        private class PaymentLog
        {
            public Payee Payee { get; set; }
            public Payer Payer { get; set; }
            public double Payment { get; set; }
            public DateTime DateSent { get; set; }
        }

        private class Payer
        {
            public Guid UserId { get; set; }
            public double Payment { get; set; }
        }
    }
}
