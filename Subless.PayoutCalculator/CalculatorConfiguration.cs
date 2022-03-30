namespace Subless.PayoutCalculator
{
    public class CalculatorConfiguration
    {
        /// <summary>
        /// Remote storage bucket name. S3 Bucket.
        /// </summary>
        public string BucketName;
        public bool RunOnStart;
        // Did this instead of timespan due to months being a wonky division, and not available for some calculations
        public int ExecutionsPerYear;
        public string CalcuationRangeStart;
        public string CalcuationRangeEnd;
        public string Domain;
        public string PoolId;
    }
}
