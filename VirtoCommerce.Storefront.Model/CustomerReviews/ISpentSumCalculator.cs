using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.CustomerReviews
{
    public interface ISpentSumCalculator
    {
        Money CalculateInPrimaryCurrency();
    }
}
