using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.Storefront.Model
{
    public class ValidationResult
    {
        public bool IsValid { get; private set; }

        public IEnumerable<string> Errors { get; private set; }

        public ValidationResult(bool isValid, IEnumerable<string> errors = null)
        {
            IsValid = isValid;
            Errors = errors ?? Enumerable.Empty<string>();
        }

        public ValidationResult(bool isValid, string error)
        {
            IsValid = isValid;
            Errors = new string[] { error };
        }

        public string ErrorsString
        {
            get
            {
                return string.Join(Environment.NewLine, Errors);
            }
        }

        public static ValidationResult OK()
        {
            return new ValidationResult(true);
        }

        public static ValidationResult Join(IEnumerable<ValidationResult> results)
        {
            var isValid = results.All(r => r.IsValid);
            return new ValidationResult(isValid, results.SelectMany(r => r.Errors));
        }
    }
}
