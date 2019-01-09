using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.CustomerReviews;
using VirtoCommerce.Storefront.Model.Order;

namespace VirtoCommerce.Storefront.Domain.CustomerReviews
{
    public class TotalSpentCalculator : ISpentSumCalculator
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public TotalSpentCalculator(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public Money CalculateInPrimaryCurrency()
        {
            var context = _workContextAccessor.WorkContext;
            IEnumerable<CustomerOrder> orders = context.CurrentUser.Orders;

            var primaryCurrency = context.AllCurrencies.FirstOrDefault(c => c.ExchangeRate == 1);

            decimal sumInBaseCurrency = 0;
            if (orders != null)
            {
                sumInBaseCurrency = orders
                        .Where(o => o.StoreId == context.CurrentUser.StoreId)
                        .Select(o => o.Total.ConvertTo(primaryCurrency))
                        .Sum(o => o.InternalAmount);
            }

            return new Money(sumInBaseCurrency, primaryCurrency);
        }

    }
}
