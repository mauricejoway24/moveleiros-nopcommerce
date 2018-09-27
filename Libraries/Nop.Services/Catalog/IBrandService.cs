using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;

namespace Nop.Services.Catalog
{
    public interface IBrandService
    {
        IPagedList<Brand> GetAllBrands(string brandName = "",
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false);

        void InsertBrand(Brand brand);

        Brand GetBrandById(int brandId);

        void UpdateBrand(Brand brand);

        void DeleteBrand(Brand brand);

        int GetNextOrderDisplay();
    }
}