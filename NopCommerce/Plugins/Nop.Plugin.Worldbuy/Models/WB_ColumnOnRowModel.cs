using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Worldbuy.Models
{
    public partial class WB_ColumnOnRowModel
    {
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.ColumnPerRow_1280")]
        public int ColumnPerRow_1280 { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.ColumnPerRow_1000")]
        public int ColumnPerRow_1000 { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.ColumnPerRow_768")]
        public int ColumnPerRow_768 { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.ColumnPerRow_480")]
        public int ColumnPerRow_480 { get; set; }
        public List<SelectListItem> Columns
        {
            get
            {
                var columns = Enum.GetValues(typeof(ColumnPerRow)).Cast<ColumnPerRow>().Select(x => new SelectListItem
                {
                    Text = (x.ToString()),
                    Value = ((int)x).ToString()
                }).ToList();
                return columns;
            }
        }
    }
}
