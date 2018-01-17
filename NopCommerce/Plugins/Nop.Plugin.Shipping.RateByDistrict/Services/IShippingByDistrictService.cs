using Nop.Core;
using Nop.Plugin.Shipping.RateByDistrict.Domain;

namespace Nop.Plugin.Shipping.RateByDistrict.Services
{
    public interface IShippingByDistrictService
    {
        ShippingByDistrictRecord FindRecord(int shippingMethodId,
            int storeId, int warehouseId,
            int countryId, int stateProvinceId, string zip);
        IPagedList<ShippingByDistrictRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);
        void InsertShippingByDistrictRecord(ShippingByDistrictRecord shippingByDistrictRecord);
        ShippingByDistrictRecord GetById(int shippingByDistrictRecordId);
        void UpdateShippingByDistrictRecord(ShippingByDistrictRecord shippingByDistrictRecord);
        void DeleteShippingByDistrictRecord(ShippingByDistrictRecord shippingByDistrictRecord);
    }
}
