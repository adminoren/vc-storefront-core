using Moq;
using VirtoCommerce.Storefront.Domain.CustomerReview;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.CustomerReviews;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.CustomerReviews
{
    public class SubmitCustomerReviewsRulesCheckerTests
    {
        [Fact]
        public void Passes_When_NoRules()
        {
            //Arrange
            var rulesChecker = new MultipleRulesChecker(new ICheckRule[0]);

            //Act
            var result = rulesChecker.Check();

            //Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Not_Passes_When_Exists_No_Pass_Rule()
        {
            //Arrange
            var noPassingRule = new Mock<ICheckRule>();
            noPassingRule.Setup(r => r.Check()).Returns(new ValidationResult(false, "rule is not passed"));

            var noPassingRule2 = new Mock<ICheckRule>();
            noPassingRule2.Setup(r => r.Check()).Returns(new ValidationResult(false, "rule2 is not passed"));

            var rulesChecker = new MultipleRulesChecker(new ICheckRule[] { noPassingRule.Object, noPassingRule2.Object });

            //Act
            var result = rulesChecker.Check();

            //Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, m => m == "rule is not passed");
            Assert.Contains(result.Errors, m => m == "rule2 is not passed");
        }

        [Fact]
        public void Passes_When_Not_Exists_No_Pass_Rule()
        {
            //Arrange
            var passingRule = new Mock<ICheckRule>();
            passingRule.Setup(r => r.Check()).Returns(ValidationResult.OK());
            var rulesChecker = new MultipleRulesChecker(new ICheckRule[] { passingRule.Object });

            //Act
            var result = rulesChecker.Check();

            //Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
