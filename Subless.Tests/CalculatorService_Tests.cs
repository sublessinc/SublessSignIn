using System;
using System.Collections.Generic;
using System.Linq;
using Subless.PayoutCalculator;
using Xunit;

namespace Subless.Tests
{
    public class CalculatorService_Tests
    {
        [Fact]
        public void CalculatorService_WithNoPayment_SendsNoPayment()
        {
            //Arrange
            var allPayments = new Dictionary<string, double>();
            var sut = CalculatorServiceBuilder();

            //Act
            sut.CalculatePayments(DateTime.Now.AddMonths(-1), DateTime.Now);

            //Assert
            Assert.NotEmpty(allPayments); // We should have a payment directed at subless, even if it's zero
            Assert.Equal(0, allPayments.Sum(payment => payment.Value)); //We shouldn't be paying anyone
            
        }

        private CalculatorService CalculatorServiceBuilder()
        {
            return new CalculatorService();
        }
    }
}
