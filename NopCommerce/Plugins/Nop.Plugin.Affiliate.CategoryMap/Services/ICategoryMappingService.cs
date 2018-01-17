using Nop.Plugin.Affiliate.CategoryMap.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.CategoryMap.Services
{
    public interface ICategoryMappingService
    {
        CategoryMapping GetById(int id);
        IEnumerable<CategoryMapping> GetAllByCategoryId(int source, int categoryId);
    }
}
