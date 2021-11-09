using System;

namespace Subless.PayoutCalculator
{
    public class CalculatorConfiguration
    {
        public string BucketName;
        public bool RunOnStart;
        // Did this instead of timespan due to months being a wonky division, and not available for some calculations
        public int ExecutionsPerYear;
    }
}
