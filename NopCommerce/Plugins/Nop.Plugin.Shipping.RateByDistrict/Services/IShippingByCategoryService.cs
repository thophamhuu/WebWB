using Nop.Core;
using Nop.Plugin.Shipping.RateByDistrict.Domain;
using Nop.Plugin.Shipping.RateByDistrict.Models;

namespace Nop.Plugin.Shipping.RateByDistrict.Services
{
    public interface IShippingByCategoryService
    {
        ShippingByCategoryRecord FindRecord(int storeId, int productTypeId,int categoryId);
        IPagedList<ShippingByCategoryRecord> GetAll(int productTypeId,int pageIndex = 0, int pageSize = int.MaxValue);
        void InsertShippingByCategoryRecord(ShippingByCategoryRecord shippingByCategoryRecord);
        ShippingByCategoryRecord GetById(int shippingByCategoryRecordId);
        ShippingByCategoryModel GetCategoryByCategoryId(int Id);
        ShippingByProductTypeModel GetProductTypeByCategoryId(int Id);
        void UpdateShippingByCategoryRecord(ShippingByCategoryRecord shippingByCategoryRecord);
        void DeleteShippingByCategoryRecord(ShippingByCategoryRecord shippingByCategoryRecord);
    }
}
