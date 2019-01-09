using System;
using Moq;
using VirtoCommerce.Storefront.Domain.CustomerReview;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.CustomerReviews
{
#pragma warning disable S3881
    public class AnonymousUserRuleTests : IDisposable
    {
        private TestContext _testContext;

        internal class TestContext
        {
            private readonly SettingEntry _allowAnonymousSetting;

            public TestContext(WorkContext workContext, SettingEntry allowAnonymousSetting, IWorkContextAccessor accessor)
            {
                WorkContext = workContext;
                WorkContextAccessor = accessor;
                _allowAnonymousSetting = allowAnonymousSetting;
            }

            public WorkContext WorkContext { get; private set; }

            public IWorkContextAccessor WorkContextAccessor { get; private set; }

            public bool UserIsRegistered
            {
                get { return WorkContext.CurrentUser.IsRegisteredUser; }
                set { WorkContext.CurrentUser.IsRegisteredUser = value; }
            }

            public void ClearCurrentStoreSettings()
            {
                WorkContext.CurrentStore.Settings.Clear();
            }

            public bool AllowAnonymousSettingValue
            {
                get { return _allowAnonymousSetting.Value == "true"; }
                set
                {
                    _allowAnonymousSetting.Value = value ? "true" : "false";
                }
            }

        }


        public AnonymousUserRuleTests()
        {
            var context = new WorkContext();

            var storeId = "Electronics";
            var store = new Model.Stores.Store() { Id = storeId };
            var user = new Model.Security.User() { StoreId = storeId };
            context.CurrentUser = user;
            context.CurrentStore = store;

            var allowAnonymousSetting = new SettingEntry() { Name = "CustomerReviews.CustomerReviewsEnabledForAnonymous", Value = "true", ValueType = "boolean" };
            store.Settings.Add(allowAnonymousSetting);

            var mock = new Mock<IWorkContextAccessor>();
            mock.Setup(m => m.WorkContext).Returns(context);

            _testContext = new TestContext(context, allowAnonymousSetting, mock.Object);
        }

        public void Dispose()
        {
            _testContext = null;
        }

        [Fact]
        public void Rule_Throws_Exception_When_AllowAnonymousSetting_NotExists()
        {
            //Arrange
            _testContext.ClearCurrentStoreSettings();
            var rule = new AnonymousUserRule(_testContext.WorkContextAccessor);

            //Act
            //Assert
            Assert.Throws<StorefrontException>(() => rule.Check());
        }

        [Fact]
        public void Rule_Passes_When_AllowAnonymousSetting_Is_True_And_UserIsAnonymous()
        {
            //Arrange
            var rule = new AnonymousUserRule(_testContext.WorkContextAccessor);
            _testContext.AllowAnonymousSettingValue = true;
            _testContext.UserIsRegistered = false;

            //Act
            var res = rule.Check();

            //Assert
            Assert.True(res.IsValid);
        }

        [Fact]
        public void Rule_NotPasses_When_AllowAnonymousSetting_Is_False_And_UserIsAnonymous()
        {
            //Arrange
            var rule = new AnonymousUserRule(_testContext.WorkContextAccessor);
            _testContext.AllowAnonymousSettingValue = false;
            _testContext.UserIsRegistered = false;

            //Act
            var res = rule.Check();

            //Assert
            Assert.False(res.IsValid);
        }

        [Fact]
        public void Rule_Passes_When_AllowAnonymousSetting_Is_False_And_UserIsRegistered()
        {
            //Arrange
            var rule = new AnonymousUserRule(_testContext.WorkContextAccessor);
            _testContext.AllowAnonymousSettingValue = false;
            _testContext.UserIsRegistered = true;

            //Act
            var res = rule.Check();

            //Assert
            Assert.True(res.IsValid);
        }

        [Fact]
        public void Rule_Passes_When_AllowAnonymousSetting_Is_True_And_UserIsRegistered()
        {
            //Arrange
            var rule = new AnonymousUserRule(_testContext.WorkContextAccessor);
            _testContext.AllowAnonymousSettingValue = true;
            _testContext.UserIsRegistered = true;

            //Act
            var res = rule.Check();

            //Assert
            Assert.True(res.IsValid);
        }

    }
}
