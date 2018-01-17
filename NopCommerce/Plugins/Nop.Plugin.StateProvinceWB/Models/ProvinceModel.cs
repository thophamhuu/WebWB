using Nop.Services.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Worldbuy.StateProvinceWB.Models
{
    public class StateProvinceWBModel
    {
        public int STT { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string Abbreviation { get; set; }
    }
}
