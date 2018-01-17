using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Worldbuy.StateProvinceWB.Domain;
using Nop.Plugin.Worldbuy.StateProvinceWB.Models;

namespace Nop.Plugin.Worldbuy.StateProvinceWB.Services
{
    public partial class StateProvinceWBService : StateProvinceService, IStateProvinceWBService
    {
        #region Fields

        private readonly IRepository<StateProvincePostalCode> _stateProvincePostalCodeRepository;
        #endregion

        #region Ctor
        public StateProvinceWBService(ICacheManager cacheManager,
           IRepository<StateProvince> stateProvinceRepository,
           IEventPublisher eventPublisher,
           IRepository<StateProvincePostalCode> stateProvincePostalCodeRepository) : base(cacheManager, stateProvinceRepository, eventPublisher)
        {
            this._stateProvincePostalCodeRepository = stateProvincePostalCodeRepository;
        }


        #endregion

        #region Methods
        public void Delete(StateProvincePostalCode record)
        {
            this._stateProvincePostalCodeRepository.Delete(record);
        }
        public void Delete(IList<StateProvincePostalCode> record)
        {
            this._stateProvincePostalCodeRepository.Delete(record);
        }
        public IList<StateProvinceWBModel> GetAllStateProvinceWBByCountryId(int countryId)
        {
            var result = new List<StateProvinceWBModel>();
            var stateProvinces = this.GetStateProvincesByCountryId(countryId);
            if (stateProvinces != null)
            {
                int i = 0;
                stateProvinces.ToList().ForEach(x =>
                {
                    var item = this.GetStateProvinceWBModelByStateProvinceId(x.Id);
                    if (item != null)
                    {
                        result.Add(item);
                    }
                });
            }
            return result;
        }

        public StateProvincePostalCode GetById(int Id)
        {
            return _stateProvincePostalCodeRepository.GetById(Id);
        }

        public StateProvincePostalCode GetByPostalCodeAndProvinceID(string postalCode,int provinceId)
        {
            return _stateProvincePostalCodeRepository.Table.FirstOrDefault(x => x.PostalCode == postalCode && x.StateProvinceID==provinceId);
        }

        public IList<StateProvincePostalCode> GetStateProvinceWBsByStateProvinceId(int stateprovinceId)
        {
            return this._stateProvincePostalCodeRepository.Table.Where(x => x.StateProvinceID == stateprovinceId).ToList();
        }
        public StateProvinceWBModel GetStateProvinceWBModelByStateProvinceId(int stateprovinceId)
        {
            var result = new StateProvinceWBModel();
            var stateProvince = this.GetStateProvinceById(stateprovinceId);
            if (stateProvince != null)
            {
                result.Abbreviation = stateProvince.Abbreviation;
                result.Id = stateProvince.Id;
                result.Name = stateProvince.Name;
                var stateProvinceWBs = this.GetStateProvinceWBsByStateProvinceId(stateprovinceId);
                if (stateProvinceWBs != null)
                    result.PostalCode = String.Join(",", stateProvinceWBs.Select(x => x.PostalCode).ToList());
            }
            return result;
        }

        public StateProvincePostalCode Insert(StateProvincePostalCode record)
        {
            this._stateProvincePostalCodeRepository.Insert(record);
            return record;
        }
        public void Insert(IList<StateProvincePostalCode> record)
        {
            this._stateProvincePostalCodeRepository.Insert(record);
        }

        public StateProvincePostalCode Update(StateProvincePostalCode record)
        {
            this._stateProvincePostalCodeRepository.Insert(record);
            return record;
        }
        #endregion
    }
}
