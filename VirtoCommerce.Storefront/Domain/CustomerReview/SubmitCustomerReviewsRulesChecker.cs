using System.Collections.Generic;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.CustomerReviews;

namespace VirtoCommerce.Storefront.Domain.CustomerReview
{
    public class MultipleRulesChecker : ICheckRules
    {
        private readonly IEnumerable<ICheckRule> _rules;

        public MultipleRulesChecker(IEnumerable<ICheckRule> rules)
        {
            _rules = rules;
        }

        public ValidationResult Check()
        {
            var validationResults = new List<ValidationResult>();

            foreach (var rule in _rules)
            {
                validationResults.Add(rule.Check());
            }

            return ValidationResult.Join(validationResults);
        }
    }
}
