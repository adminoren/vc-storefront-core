using VirtoCommerce.Storefront.Model.CustomerReviews;
using reviewDto = VirtoCommerce.Storefront.AutoRestClients.CustomerReviewsModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain.CustomerReview
{
    public static partial class CustomerReviewConverter
    {
        public static Model.CustomerReviews.CustomerReview ToCustomerReview(this reviewDto.CustomerReview itemDto)
        {
            var result = new Model.CustomerReviews.CustomerReview
            {
                Id = itemDto.Id,
                AuthorNickname = itemDto.AuthorNickname,
                Content = itemDto.Content,
                CreatedBy = itemDto.CreatedBy,
                CreatedDate = itemDto.CreatedDate,
                IsActive = itemDto.IsActive,
                ModifiedBy = itemDto.ModifiedBy,
                ModifiedDate = itemDto.ModifiedDate,
                ProductId = itemDto.ProductId,
                StoreId = itemDto.StoreId
            };

            return result;
        }

        public static reviewDto.CustomerReview ToCustomerReviewDto(this Model.CustomerReviews.CustomerReview item)
        {
            var result = new reviewDto.CustomerReview
            {
                Id = item.Id,
                AuthorNickname = item.AuthorNickname,
                Content = item.Content,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                IsActive = item.IsActive,
                ModifiedBy = item.ModifiedBy,
                ModifiedDate = item.ModifiedDate,
                ProductId = item.ProductId,
                StoreId = item.StoreId
            };

            return result;
        }

        public static reviewDto.CustomerReviewSearchCriteria ToSearchCriteriaDto(this CustomerReviewSearchCriteria criteria)
        {
            var result = new reviewDto.CustomerReviewSearchCriteria
            {
                IsActive = criteria.IsActive,
                ProductIds = criteria.ProductIds,

                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.Sort
            };

            return result;
        }
    }
}
