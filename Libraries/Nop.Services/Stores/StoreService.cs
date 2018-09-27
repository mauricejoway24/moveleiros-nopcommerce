using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store service
    /// </summary>
    public partial class StoreService : IStoreService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string STORES_ALL_KEY = "Nop.stores.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string STORES_BY_ID_KEY = "Nop.stores.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : short Url
        /// </remarks>
        private const string STORES_BY_PROFILE_SHORT_URL = "Nop.stores.profileShortUrl-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string STORES_PATTERN_KEY = "Nop.stores.";

        #endregion
        
        #region Fields
        
        private readonly IRepository<Store> _storeRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICityService cityService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="storeRepository">Store repository</param>
        /// <param name="eventPublisher">Event published</param>
        public StoreService(ICacheManager cacheManager,
            IRepository<Store> storeRepository,
            IEventPublisher eventPublisher,
            ICityService cityService)
        {
            this._cacheManager = cacheManager;
            this._storeRepository = storeRepository;
            this._eventPublisher = eventPublisher;
            this.cityService = cityService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void DeleteStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            var allStores = GetAllStores();
            if (allStores.Count == 1)
                throw new Exception("You cannot delete the only configured store");

            _storeRepository.Delete(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(store);
        }

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <returns>Stores</returns>
        public virtual IList<Store> GetAllStores()
        {
            string key = STORES_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from s in _storeRepository.Table
                            orderby s.DisplayOrder, s.Id
                            select s;
                var stores = query.ToList();
                return stores;
            });
        }

        public IPagedList<Store> GetPagedStores(Store model, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _storeRepository.Table;

            if (!string.IsNullOrEmpty(model.Name))
                query = query.Where(t => t.Name.Contains(model.Name));

            if (!string.IsNullOrEmpty(model.CompanyAddress))
                query = query.Where(t => t.CompanyAddress.Contains(model.CompanyAddress));

            if (model.LojistaId > 0)
                query = query.Where(t => t.LojistaId == model.LojistaId);

            query = query
                .OrderBy(t => t.DisplayOrder)
                .ThenBy(t => t.Name);

            var stores = new PagedList<Store>(
                query, 
                pageIndex - 1, 
                pageSize
            );

            return stores;
        }

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Store</returns>
        public virtual Store GetStoreById(int storeId)
        {
            if (storeId == 0)
                return null;
            
            string key = string.Format(STORES_BY_ID_KEY, storeId);
            return _cacheManager.Get(key, () => _storeRepository.GetById(storeId));
        }

        /// <summary>
        /// Gets a store by its ProfileShortUrl
        /// </summary>
        /// <param name="shortUrl">Short url</param>
        /// <returns>Store</returns>
        public virtual Store GetStoreByProfileShorUrl(string shortUrl)
        {
            if (string.IsNullOrEmpty(shortUrl))
                return null;

            string key = string.Format(STORES_BY_PROFILE_SHORT_URL, shortUrl);

            var store = _cacheManager.Get(key, () =>
                _storeRepository
                    .Table
                    .Where(t => t.ProfileShortUrl == shortUrl)
                    .FirstOrDefault()
            );

            if (store == null)
                return null;

            if (store.CityId > 0)
                store.CompanyCity = cityService.GetCityById(store.CityId)?.UF ?? "";

            return store;
        }

        /// <summary>
        /// Inserts a store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void InsertStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            _storeRepository.Insert(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(store);
        }

        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void UpdateStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            _storeRepository.Update(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(store);
        }

        #endregion

        #region Moveleiros

        public string GetChatByStoreId(int storeId)
        {
            var query = from store in _storeRepository.Table
                        where store.Id == storeId
                        select store.ChatScript;

            return query.FirstOrDefault();
        }

        public Store GetStoreByLojistaId(int lojistaId)
        {
            var query = from store in _storeRepository.Table
                        where store.LojistaId == lojistaId
                        select store;

            return query.FirstOrDefault();
        }

        #endregion

        #region Extensions by QuanNH

        public virtual IList<Store> GetStoreNameById(int[] storeId)
        {
            var query = from s in _storeRepository.Table
                        where storeId.Contains(s.Id)
                        orderby s.DisplayOrder, s.Name
                        select s;

            var stores = query.ToList();

            return stores;
        }
        public IList<Store> GetAllStoresByEntityName(int entityId, string entityName)
        {
            var _storeMappingRepository = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IRepository<StoreMapping>>();

            var query = from sm in _storeMappingRepository.Table
                        join s in _storeRepository.Table on sm.StoreId equals s.Id
                        where sm.EntityId == entityId &&
                        sm.EntityName == entityName
                        orderby s.DisplayOrder
                        select s;
            var stores = query.ToList();
            return stores;
        }

        #endregion
    }
}