using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.CustomerReviews;

namespace VirtoCommerce.Storefront.Domain.CustomerReview
{
    public class SpentSumRule : ICheckRule
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ISpentSumCalculator _calculator;

        public SpentSumRule(IWorkContextAccessor workContextAccessor, ISpentSumCalculator calculator)
        {
            _workContextAccessor = workContextAccessor;
            _calculator = calculator;
        }

        public ValidationResult Check()
        {
            var context = _workContextAccessor.WorkContext;
            var store = context.CurrentStore;
            var user = context.CurrentUser;

            if (user == null)
            {
                throw new StorefrontException(nameof(context.CurrentUser));
            }

            if (store == null)
            {
                throw new StorefrontException(nameof(context.CurrentStore));
            }

            var customerReviewsMinSpentSumToContributeSetting = store.Settings.FirstOrDefault(s => s.Name == "CustomerReviews.MinSpentSumToContribute");
            if (customerReviewsMinSpentSumToContributeSetting == null)
            {
                throw new StorefrontException("CustomerReviews.MinSpentSumToContribute setting is missing");
            }

            var minSpentSumInDollars = store.Settings.GetSettingValue<int>("CustomerReviews.MinSpentSumToContribute", 0);
            if (minSpentSumInDollars > 0)
            {
                var spentSumInBaseCurrency = _calculator.CalculateInPrimaryCurrency();
                var dollarCurrency = context.AllCurrencies.FirstOrDefault(c => c.Code == "USD");
                var spentSumInDollars = spentSumInBaseCurrency.ConvertTo(dollarCurrency).Amount;

                if (spentSumInDollars < minSpentSumInDollars)
                {
                    return new ValidationResult(false, $"User must spend at least ${minSpentSumInDollars} to submit a review");
                }
            }

            return ValidationResult.OK();


        }
    }
}

