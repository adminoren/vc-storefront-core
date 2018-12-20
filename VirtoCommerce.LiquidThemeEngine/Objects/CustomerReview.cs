using System;
using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class CustomerReview : Drop
    {
        public string AuthorNickName { get; set; }
        public string Content { get; set; }
        public bool? IsActive { get; set; }
        public string ProductId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
