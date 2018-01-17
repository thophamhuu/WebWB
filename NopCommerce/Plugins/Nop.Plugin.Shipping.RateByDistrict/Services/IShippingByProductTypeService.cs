using Nop.Core;
using Nop.Plugin.Shipping.RateByDistrict.Domain;

namespace Nop.Plugin.Shipping.RateByDistrict.Services
{
    public interface IShippingByProductTypeService
    {
        ShippingByProductTypeRecord FindRecord(int storeId);
        IPagedList<ShippingByProductTypeRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);
        void InsertShippingByProductTypeRecord(ShippingByProductTypeRecord shippingByProductTypeRecord);
        ShippingByProductTypeRecord GetById(int shippingByProductTypeRecordId);
        void UpdateShippingByProductTypeRecord(ShippingByProductTypeRecord shippingByProductTypeRecord);
        void DeleteShippingByProductTypeRecord(ShippingByProductTypeRecord shippingByProductTypeRecord);
    }
}
