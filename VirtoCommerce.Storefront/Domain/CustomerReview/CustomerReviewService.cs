using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CustomerReviewsModuleApi;
using VirtoCommerce.Storefront.Domain.CustomerReview;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.CustomerReviews;

namespace VirtoCommerce.Storefront.Domain
{
    public class CustomerReviewService : ICustomerReviewService
    {
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;
        private readonly ICustomerReviews _customerReviewsApi;
        private readonly ICheckRules _rulesChecker;

        public CustomerReviewService(IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher, ICustomerReviews customerReviewsApi, ICheckRules rulesChecker)
        {
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
            _customerReviewsApi = customerReviewsApi;
            _rulesChecker = rulesChecker;
        }

        public async Task AddReviewAsync(Model.CustomerReviews.CustomerReview review)
        {
            var validationResult = _rulesChecker.Check();

            if (!validationResult.IsValid)
            {
                throw new SubmitReviewDeniedException(validationResult.ErrorsString);
            }

            await _customerReviewsApi.UpdateAsync(new[] { review.ToCustomerReviewDto() });
        }

        public void AddReview(Model.CustomerReviews.CustomerReview review)
        {
            AddReviewAsync(review).GetAwaiter().GetResult();
        }

        public ValidationResult CheckSubmitReviewRules()
        {
            return _rulesChecker.Check();
        }

        public IPagedList<Model.CustomerReviews.CustomerReview> SearchReviews(CustomerReviewSearchCriteria criteria)
        {
            return SearchReviewsAsync(criteria).GetAwaiter().GetResult();
        }

        public async Task<IPagedList<Model.CustomerReviews.CustomerReview>> SearchReviewsAsync(CustomerReviewSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchReviewsAsync), criteria.GetCacheKey());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CustomerReviewCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                var result = await _customerReviewsApi.SearchCustomerReviewsAsync(criteria.ToSearchCriteriaDto());
                return new StaticPagedList<Model.CustomerReviews.CustomerReview>(result.Results.Select(x => x.ToCustomerReview()),
                    criteria.PageNumber, criteria.PageSize, result.TotalCount.Value);
            });
        }

    }
}
