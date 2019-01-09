using System;
using System.Collections.Generic;
using Moq;
using VirtoCommerce.Storefront.Domain.CustomerReviews;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Order;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.CustomerReviews
{
#pragma warning disable S3881
    public class SpentSumCalculatorTests : IDisposable
    {
        private TestContext _testContext;

        internal class TestContext
        {
            public TestContext(WorkContext workContext, IWorkContextAccessor accessor, Currency currency)
            {
                WorkContext = workContext;
                WorkContextAccessor = accessor;
                PrimaryCurrency = currency;
            }

            public WorkContext WorkContext { get; private set; }

            public IWorkContextAccessor WorkContextAccessor { get; private set; }

            public Currency PrimaryCurrency { get; private set; }

            public void SetOrders(IEnumerable<CustomerOrder> orders)
            {
                WorkContext.CurrentUser.Orders = new MutablePagedList<CustomerOrder>(orders);
            }

        }


        public SpentSumCalculatorTests()
        {
            var context = new WorkContext();

            var storeId = "Electronics";
            var store = new Model.Stores.Store() { Id = storeId };
            var user = new Model.Security.User() { StoreId = storeId };
            context.CurrentUser = user;
            context.CurrentStore = store;

            var primaryCurrency = new Currency(new Language("en-US"), "USD") { ExchangeRate = 1 };
            context.AllCurrencies = new List<Currency>() { primaryCurrency };

            var mock = new Mock<IWorkContextAccessor>();
            mock.Setup(m => m.WorkContext).Returns(context);

            _testContext = new TestContext(context, mock.Object, primaryCurrency);
        }

        public void Dispose()
        {
            _testContext = null;
        }

        [Fact]
        public void Returns_Zero_When_No_Orders()
        {
            //Arrange
            var calculator = new TotalSpentCalculator(_testContext.WorkContextAccessor);

            //Act
            //Assert
            Assert.Equal(new Money(0m, _testContext.PrimaryCurrency), calculator.CalculateInPrimaryCurrency());
        }

        [Fact]
        public void Calculate_TotalSumFromOrders()
        {
            //Arrange
            var calculator = new TotalSpentCalculator(_testContext.WorkContextAccessor);
            var storeId = _testContext.WorkContext.CurrentUser.StoreId;
            var eurExchangeRate = 1.5m;
            var currencyEUR = new Currency(new Language("en-US"), "EUR") { ExchangeRate = eurExchangeRate };
            var orders = new List<CustomerOrder>(2)
            {
                new CustomerOrder(_testContext.PrimaryCurrency){ StoreId = storeId, Total = new Money(10m, _testContext.PrimaryCurrency)},
                new CustomerOrder(_testContext.PrimaryCurrency){ StoreId = storeId, Total = new Money(20m, _testContext.PrimaryCurrency)},
                new CustomerOrder(currencyEUR) { StoreId = storeId, Total = new Money(10m, currencyEUR) }
            };
            _testContext.SetOrders(orders);

            //Act
            //Assert
            Assert.Equal(new Money((10 + 20) + (10 * eurExchangeRate), _testContext.PrimaryCurrency), calculator.CalculateInPrimaryCurrency());
        }

    }
}
