using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class CustomerReviewConverter
    {
        public static CustomerReview ToShopifyModel(this StorefrontModel.CustomerReviews.CustomerReview customerReview)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidCustomerReview(customerReview);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual CustomerReview ToLiquidCustomerReview(StorefrontModel.CustomerReviews.CustomerReview customerReview)
        {
            var result = new CustomerReview();

            result.AuthorNickName = customerReview.AuthorNickname;
            result.Content = customerReview.Content;
            result.CreatedDate = customerReview.CreatedDate;

            return result;
        }
    }
}
