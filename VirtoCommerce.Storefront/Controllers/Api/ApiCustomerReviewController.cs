using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.CustomerReviews;


namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("customerReviews")]
    [ApiController]
    public class ApiCustomerReviewController : StorefrontControllerBase
    {
        private readonly ICustomerReviewService _customerReviewService;

        public ApiCustomerReviewController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ICustomerReviewService customerReviewService)
            : base(workContextAccessor, urlBuilder)
        {
            _customerReviewService = customerReviewService;
        }

        // POST: storefrontapi/customerReviews
        /// <summary>
        /// Save customer review for product
        /// </summary>
        /// <param name="review"></param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(typeof(CustomerReview), 200)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> SubmitReview([FromBody] CustomerReview review)
        {
            try
            {
                review.StoreId = WorkContext.CurrentStore.Id;
                review.IsActive = true;
                review.CreatedBy = WorkContext.CurrentUser.OperatorUserName;
                review.CreatedDate = DateTime.Now;
                await _customerReviewService.AddReviewAsync(review);
            }
            catch (SubmitReviewDeniedException e)
            {
                return StatusCode(403, e.Message);
            }
            return Ok(review);
        }

        // POST: storefrontapi/customerReviews/search
        /// <summary>
        /// Returns customer reviews
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<ActionResult> GetCustomerReviews(CustomerReviewSearchCriteria criteria)
        {
            var customerReviews = await _customerReviewService.SearchReviewsAsync(criteria);

            return Json(customerReviews);
        }

        [HttpGet("checksubmit")]
        public ActionResult CheckIfSumbitReviewAvailable()
        {
            var result = _customerReviewService.CheckSubmitReviewRules();
            return Json(
                new
                {
                    result.IsValid,
                    Message = result.ErrorsString
                });
        }
    }
}
