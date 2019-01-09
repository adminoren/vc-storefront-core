using System;
using System.Collections.Generic;
using Moq;
using VirtoCommerce.Storefront.Domain.CustomerReview;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.CustomerReviews;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.CustomerReviews
{
#pragma warning disable S3881
    public class SpentSumRuleTests : IDisposable
    {
        private TestContext _testContext;

        internal class TestContext
        {
            public WorkContext WorkContext { get; set; }
            public SettingEntry MinSpentSumToContributeSetting { get; set; }
            public Currency PrimaryCurrency { get; set; }
            public IWorkContextAccessor WorkContextAccessor { get; set; }
        }


        public SpentSumRuleTests()
        {
            var context = new WorkContext();

            var storeId = "Electronics";
            var store = new Model.Stores.Store() { Id = storeId };
            var user = new Model.Security.User() { StoreId = storeId };
            context.CurrentUser = user;
            context.CurrentStore = store;
            var currency = new Currency(new Language("en-US"), "USD") { ExchangeRate = 1 };
            context.AllCurrencies = new List<Currency>() { currency };

            var minSpentSumToContribute = new SettingEntry() { Name = "CustomerReviews.MinSpentSumToContribute", Value = "0", ValueType = "int" };
            store.Settings.Add(minSpentSumToContribute);

            var mock = new Mock<IWorkContextAccessor>();
            mock.Setup(m => m.WorkContext).Returns(context);

            _testContext = new TestContext()
            {
                WorkContext = context,
                MinSpentSumToContributeSetting = minSpentSumToContribute,
                PrimaryCurrency = currency,
                WorkContextAccessor = mock.Object
            };
        }

        public void Dispose()
        {
            _testContext = null;
        }

        [Fact]
        public void RuleThrowsExceptionWhenMinSpentSumToContributeSettingNotExists()
        {
            //Arrange
            var mockCalculator = new Mock<ISpentSumCalculator>();
            _testContext.WorkContext.CurrentStore.Settings.Clear();
            var rule = new SpentSumRule(_testContext.WorkContextAccessor, mockCalculator.Object);

            //Act
            //Assert
            Assert.Throws<StorefrontException>(() => rule.Check());
        }

        [Fact]
        public void RulePassesWhenMinSpentSumToContributeSettingIsZero()
        {
            //Arrange
            var mockCalculator = new Mock<ISpentSumCalculator>();
            mockCalculator.Setup(m => m.CalculateInPrimaryCurrency()).Returns(new Money(0m, _testContext.PrimaryCurrency));
            var rule = new SpentSumRule(_testContext.WorkContextAccessor, mockCalculator.Object);

            //Act
            var res = rule.Check();

            //Assert
            Assert.True(res.IsValid);
        }

        [Fact]
        public void RulePassesWhenMinSpentSumToContributeSettingLessThanOrdersSum()
        {
            //Arrange
            var mockCalculator = new Mock<ISpentSumCalculator>();
            _testContext.MinSpentSumToContributeSetting.Value = "0";
            mockCalculator.Setup(m => m.CalculateInPrimaryCurrency()).Returns(new Money(50m, _testContext.PrimaryCurrency));
            var rule = new SpentSumRule(_testContext.WorkContextAccessor, mockCalculator.Object);

            //Act
            var res = rule.Check();

            //Assert
            Assert.True(res.IsValid);
        }

        [Fact]
        public void RulePassesWhenMinSpentSumToContributeSettingEqualsOrdersSum()
        {
            //Arrange
            var mockCalculator = new Mock<ISpentSumCalculator>();
            _testContext.MinSpentSumToContributeSetting.Value = "50";
            mockCalculator.Setup(m => m.CalculateInPrimaryCurrency()).Returns(new Money(50m, _testContext.PrimaryCurrency));
            var rule = new SpentSumRule(_testContext.WorkContextAccessor, mockCalculator.Object);

            //Act
            var res = rule.Check();

            //Assert
            Assert.True(res.IsValid);
        }

        [Fact]
        public void Rule_NotPasses_When_MinSpentSumToContributeSetting_More_Than_OrdersSum()
        {
            //Arrange
            var mockCalculator = new Mock<ISpentSumCalculator>();
            _testContext.MinSpentSumToContributeSetting.Value = "50";
            mockCalculator.Setup(m => m.CalculateInPrimaryCurrency()).Returns(new Money(10m, _testContext.PrimaryCurrency));
            var rule = new SpentSumRule(_testContext.WorkContextAccessor, mockCalculator.Object);

            //Act
            var res = rule.Check();

            //Assert
            Assert.False(res.IsValid);
        }
    }
}
