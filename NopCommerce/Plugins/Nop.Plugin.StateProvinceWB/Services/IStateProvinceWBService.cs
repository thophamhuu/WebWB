using Nop.Plugin.Worldbuy.StateProvinceWB.Domain;
using Nop.Plugin.Worldbuy.StateProvinceWB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Worldbuy.StateProvinceWB.Services
{
    public partial interface IStateProvinceWBService
    {
        StateProvincePostalCode GetById(int Id);
        StateProvincePostalCode GetByPostalCodeAndProvinceID(string postalCode, int provinceId);
        StateProvincePostalCode Insert(StateProvincePostalCode record);
        void Insert(IList<StateProvincePostalCode> record);
        StateProvincePostalCode Update(StateProvincePostalCode record);
        void Delete(StateProvincePostalCode record);
        void Delete(IList<StateProvincePostalCode> record);
        IList<StateProvinceWBModel> GetAllStateProvinceWBByCountryId(int countryId);
        IList<StateProvincePostalCode> GetStateProvinceWBsByStateProvinceId(int stateprovinceId);
        StateProvinceWBModel GetStateProvinceWBModelByStateProvinceId(int stateprovinceId);
    }
}
