using Nop.Core.Domain.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.Ebay.Models
{
    public class CategoryEbayModel : BaseNopEntityModel, ILocalizedEntity
    {
        public int EbayId { get; set; }
        public string Name { get; set; }
        public int ParentCategoryId { get; set; }
        public int Level { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public int CategoryMapID { get; set; } = 0;
        public int CategoryID { get; set; } = 0;
        public string CategoryName { get; set; }

    }
}
