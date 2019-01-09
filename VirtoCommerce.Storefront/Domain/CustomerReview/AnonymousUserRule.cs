using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.CustomerReviews;

namespace VirtoCommerce.Storefront.Domain.CustomerReview
{
    public class AnonymousUserRule : ICheckRule
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public AnonymousUserRule(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
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

            var canAnonymousSubmitReviewSetting = store.Settings.FirstOrDefault(s => s.Name == "CustomerReviews.CustomerReviewsEnabledForAnonymous");
            if (canAnonymousSubmitReviewSetting == null)
            {
                throw new StorefrontException("CustomerReviews.CustomerReviewsEnabledForAnonymous setting is missing");
            }

            if (!user.IsRegisteredUser && !store.Settings.GetSettingValue<bool>("CustomerReviews.CustomerReviewsEnabledForAnonymous", false))
            {
                return new ValidationResult(false, "Only registered users can submit customer review");
            }

            return ValidationResult.OK();
        }
    }
}
