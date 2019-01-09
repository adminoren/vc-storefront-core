using System;

namespace VirtoCommerce.Storefront.Model.CustomerReviews
{
    public class SubmitReviewDeniedException : Exception
    {
        public SubmitReviewDeniedException(string message) : base(message)
        {
        }
    }
}
