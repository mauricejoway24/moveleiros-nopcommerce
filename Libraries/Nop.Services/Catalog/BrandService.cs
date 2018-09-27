using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Catalog
{
    public class BrandService : IBrandService
    {
        private const string BRAND_ALL_KEY = "Mov.brands.all";
        private const string BRAND_ALL_KEY_BY_PRODUCT = "Mov.brands.byproduct-{0}";
        private const string BRAND_PATTERN_KEY = "Mov.brands.";
        private const string BRANDS_BY_ID_KEY = "Mov.brands.id-{0}";

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<ProductBrand> _productBrandRepository;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="brandRepository">Brand repository</param>
        public BrandService(ICacheManager cacheManager, 
            IRepository<Brand> brandRepository,
            IRepository<ProductBrand> productBrandRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _brandRepository = brandRepository;
            _eventPublisher = eventPublisher;
            _productBrandRepository = productBrandRepository;
        }

        /// <summary>
        /// Gets all brands
        /// </summary>
        /// <returns>Stores</returns>
        public virtual IPagedList<Brand> GetAllBrands(string brandName = "",
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false)
        {
            var key = BRAND_ALL_KEY;

            return _cacheManager.Get(key, () =>
            {
                var query = _brandRepository.Table;

                if (!showHidden)
                    query = query.Where(m => m.Published);

                if (!string.IsNullOrWhiteSpace(brandName))
                    query = query.Where(m => m.Name.Contains(brandName));

                query = query
                    .Where(m => !m.Deleted)
                    .OrderBy(m => m.DisplayOrder)
                    .ThenBy(m => m.Id);

                var brands = new PagedList<Brand>(query, pageIndex, pageSize);

                return brands;
            });
        }

        /// <summary>
        /// Get the next order display number
        /// </summary>
        /// <returns>The next order display</returns>
        public virtual int GetNextOrderDisplay()
        {
            return _brandRepository
                .Table
                .Max(t => t.DisplayOrder) + 1;
        }

        /// <summary>
        /// Gets a brand
        /// </summary>
        /// <param name="brandId">Brand identifier</param>
        /// <returns>Brand</returns>
        public virtual Brand GetBrandById(int brandId)
        {
            if (brandId == 0)
                return null;

            string key = string.Format(BRANDS_BY_ID_KEY, brandId);
            return _cacheManager.Get(key, () => _brandRepository.GetById(brandId));
        }

        /// <summary>
        /// Inserts a brand
        /// </summary>
        /// <param name="brand">Brand</param>
        public virtual void InsertBrand(Brand brand)
        {
            if (brand == null)
                throw new ArgumentNullException("brand");

            _brandRepository.Insert(brand);

            //cache
            _cacheManager.RemoveByPattern(BRAND_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(brand);
        }

        /// <summary>
        /// Updates the brand
        /// </summary>
        /// <param name="brand">Brand</param>
        public virtual void UpdateBrand(Brand brand)
        {
            if (brand == null)
                throw new ArgumentNullException("brand");

            _brandRepository.Update(brand);

            //cache
            _cacheManager.RemoveByPattern(BRAND_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(brand);
        }

        /// <summary>
        /// Deletes a brand
        /// </summary>
        /// <param name="brand">Brand</param>
        public virtual void DeleteBrand(Brand brand)
        {
            if (brand == null)
                throw new ArgumentNullException("brand");

            brand.Deleted = true;

            UpdateBrand(brand);

            //cache
            _cacheManager.RemoveByPattern(BRAND_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(brand);
        }
    }
}
