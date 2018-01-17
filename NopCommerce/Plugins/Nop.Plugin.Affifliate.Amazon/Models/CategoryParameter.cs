using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Affiliate.Amazon.Models
{
    public class CategoryParameter
    {
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Category.CategoryID")]
        public int CategoryID { get; set; }
        public IList<SelectListItem> Categories { get; set; }
    }
    public class ProductParameter
    {
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Product.Keywords")]
        public string Keywords { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Product.CategoryID")]
        public int CategoryID { get; set; }
        public IList<SelectListItem> Categories { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Product.Id")]
        public int Id { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Product.Properties")]
        public int[] Properties { get; set; }
        public SyncProperties SyncProperties
        {
            get
            {
                if (Properties == null)
                    return 0;
                int i = 0;
                foreach (var p in Properties)
                    i += p;
                return (SyncProperties)i;
            }
            set
            {
                List<int> i = new List<int>();

                Enum.GetValues(typeof(SyncProperties)).Cast<SyncProperties>().ToList().ForEach(x =>
                {
                    if (value.HasFlag(x))
                        i.Add((int)x);
                });
                this.Properties = i.ToArray();
            }
        }
        public IList<SelectListItem> SyncPropertyList
        {
            get
            {
                return Enum.GetValues(typeof(SyncProperties)).Cast<SyncProperties>().Select(x => new SelectListItem
                {
                    Text = x.ToString(),
                    Value = ((int)x).ToString(),
                    Selected = SyncProperties.HasFlag(x)
                }).ToList();
            }
        }
        public bool? IsPublished { get; set; }
    }

    public class CategorySearch
    {
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Category.BrowseNodeId")]
        public string BrowseNodeId { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Category.CategoryName")]
        public string CategoryName { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Category.CompareType")]
        public int CompareType { get; set; }
        public List<SelectListItem> CompareTypes
        {
            get
            {
                return Enum.GetValues(typeof(CompareType)).Cast<CompareType>().Select(x => new SelectListItem
                {
                    Text = x.ToString(),
                    Value = ((int)x).ToString()
                }).ToList();
            }
        }
    }

    public enum CompareType
    {
        Starts_With = 1,
        Contains = 2

    }

    public enum SyncProperties
    {
        Name = 1 << 0,
        ShortDescription = 1 << 1,
        FullDescription = 1 << 2,
        Price = 1 << 3,
        Attributes = 1 << 4,
        Variations = 1 << 5,
        Images = 1 << 6,
        DetailPageURL = 1 << 7,
        All = Name | Images | ShortDescription | FullDescription | Price | Attributes | Variations | DetailPageURL
    }
}