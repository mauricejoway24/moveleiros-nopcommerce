using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store mapping service
    /// </summary>
    public partial class StoreMappingService : IStoreMappingService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        private const string STOREMAPPING_BY_ENTITYID_NAME_KEY = "Nop.storemapping.entityid-name-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string STOREMAPPING_PATTERN_KEY = "Nop.storemapping.";

        #endregion

        #region Fields

        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStoreContext _storeContext;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Store> storeRepository;
        private readonly IRepository<Customer> customerRepository;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event publisher</param>
        public StoreMappingService(ICacheManager cacheManager, 
            IStoreContext storeContext,
            IRepository<StoreMapping> storeMappingRepository,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher,
            IRepository<Store> storeRepository,
            IRepository<Customer> customerRepository)
        {
            this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._storeMappingRepository = storeMappingRepository;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
            this.storeRepository = storeRepository;
            this.customerRepository = customerRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a store mapping record
        /// </summary>
        /// <param name="storeMapping">Store mapping record</param>
        public virtual void DeleteStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentNullException("storeMapping");

            _storeMappingRepository.Delete(storeMapping);

            //cache
            _cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(storeMapping);
        }

        /// <summary>
        /// Gets a store mapping record
        /// </summary>
        /// <param name="storeMappingId">Store mapping record identifier</param>
        /// <returns>Store mapping record</returns>
        public virtual StoreMapping GetStoreMappingById(int storeMappingId)
        {
            if (storeMappingId == 0)
                return null;

            return _storeMappingRepository.GetById(storeMappingId);
        }

        /// <summary>
        /// Gets store mapping records
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Store mapping records</returns>
        public virtual IList<StoreMapping> GetStoreMappings<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            var query = from sm in _storeMappingRepository.Table
                        where sm.EntityId == entityId &&
                        sm.EntityName == entityName
                        select sm;
            var storeMappings = query.ToList();
            return storeMappings;
        }


        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <param name="storeMapping">Store mapping</param>
        public virtual void InsertStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentNullException("storeMapping");

            _storeMappingRepository.Insert(storeMapping);

            //cache
            _cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(storeMapping);
        }

        /// <summary>
        /// Inserts a store mapping record
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store id</param>
        /// <param name="entity">Entity</param>
        public virtual void InsertStoreMapping<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (storeId == 0)
                throw new ArgumentOutOfRangeException("storeId");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            var storeMapping = new StoreMapping
            {
                EntityId = entityId,
                EntityName = entityName,
                StoreId = storeId
            };

            InsertStoreMapping(storeMapping);
        }

        /// <summary>
        /// Updates the store mapping record
        /// </summary>
        /// <param name="storeMapping">Store mapping</param>
        public virtual void UpdateStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentNullException("storeMapping");

            _storeMappingRepository.Update(storeMapping);

            //cache
            _cacheManager.RemoveByPattern(STOREMAPPING_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(storeMapping);
        }

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>Store identifiers</returns>
        public virtual int[] GetStoresIdsWithAccess<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            string key = string.Format(STOREMAPPING_BY_ENTITYID_NAME_KEY, entityId, entityName);
            return _cacheManager.Get(key, () =>
            {
                var query = from sm in _storeMappingRepository.Table
                            where sm.EntityId == entityId &&
                            sm.EntityName == entityName
                            select sm.StoreId;
                return query.ToArray();
            });
        }

        /// <summary>
        /// Authorize whether entity could be accessed in the current store (mapped to this store)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Wntity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            return Authorize(entity, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                return false;

            if (storeId == 0)
                //return true if no store specified/found
                return true;

            if (_catalogSettings.IgnoreStoreLimitations)
                return true;

            if (!entity.LimitedToStores)
                return true;

            foreach (var storeIdWithAccess in GetStoresIdsWithAccess(entity))
                if (storeId == storeIdWithAccess)
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }

        #endregion

        #region Extensions by QuanNH

        public virtual IList<StoreMapping> GetAllStoreMapping(string entityName)
        {
            var query = from s in _storeMappingRepository.Table
                        where s.EntityName.Contains(entityName) || s.EntityName.Contains("Admin")
                        orderby s.StoreId
                        select s;
            var stores = query.ToList();
            return stores;
        }

        public virtual List<int> GetStoreIdByEntityId(int entityId, string entityName)
        {
            var query = from sm in _storeMappingRepository.Table
                        where sm.EntityId == entityId &&
                        sm.EntityName == entityName
                        select sm.StoreId;
            var result = query.ToList();
            return result;
        }

        public virtual List<int> GetEntityIdByListStoreId(int[] storeIds, string entityName)
        {
            var query = from sm in _storeMappingRepository.Table
                        where storeIds.Contains(sm.StoreId) &&
                        sm.EntityName == entityName
                        select sm.EntityId;
            var result = query.ToList();
            return result;
        }

        public virtual void InsertStoreMapping(int entityId, string entityName, int storeId)
        {
            if (storeId == 0)
                throw new ArgumentOutOfRangeException("storeId");

            var storeMapping = new StoreMapping()
            {
                EntityId = entityId,
                EntityName = entityName,
                StoreId = storeId
            };

            InsertStoreMapping(storeMapping);
        }

        public virtual bool TableAuthorize<T>(T entity, int storeId = 0) where T : BaseEntity
        {
            if (entity == null)
                return false;

            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IWorkContext>();
            List<int> storeIds = this.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Stores");

            if (storeId == storeIds.FirstOrDefault())
                return true;

            if (storeIds.Count <= 0)
                //return true if no store specified/found
                return true;

            //no permission found
            return false;
        }

        public virtual bool TableEdit(int storeId = 0)
        {
            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IWorkContext>();
            List<int> customerIds = this.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Stores");

            if (storeId == customerIds.FirstOrDefault())
            {
                return true;
            }

            if (customerIds.Count <= 0)
                //return true if no store specified/found
                return true;

            return false;

        }

        public virtual bool TableEdit(int entityId, string entityName)
        {
            if (entityId == 0)
            {
                return false;
            }

            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IWorkContext>();
            List<int> customerIds = this.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Stores");
            int storeId = this.GetStoreIdByEntityId(entityId, entityName).FirstOrDefault();

            if (customerIds.Count > 0 && !customerIds.Contains(storeId))
            {
                return false;
            }

            return true;
        }

        public int CurrentStore()
        {
            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IWorkContext>();
            List<int> storeIds = this.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Stores");
            return storeIds.Count > 0 ? storeIds.FirstOrDefault() : 0;
        }

        public IPagedList<StoreMapping> GetAllPagedStoreMapping(
            string storeName, 
            string entityName, 
            int pageIndex, 
            int pageSize,
            bool removeCustomerProfile = false)
        {
            var query = _storeMappingRepository.Table;

            if (!string.IsNullOrEmpty(storeName))
            {
                var storeIds = storeRepository.Table
                    .Where(t => t.Name.Contains(storeName))
                    .Select(t => t.Id)
                    .ToList();

                query = query.Where(t => storeIds.Any(c => c == t.StoreId));
            }

            if (removeCustomerProfile)
            {
                var allowedNonCustomerProfile = customerRepository.Table
                    .Where(t => t.CustomerRoles.Any(cr => !"ConsumidorSpecialDayPlanejados,Registered".Contains(cr.SystemName) && !t.Deleted))
                    .Select(t => t.Id)
                    .ToList();

                query = query.Where(t => allowedNonCustomerProfile.Any(a => a == t.EntityId));
            }

            query = from s in query
                    where s.EntityName.Contains(entityName) || s.EntityName.Contains("Admin")
                    orderby s.Id
                    select s;

            var stores = new PagedList<StoreMapping>(
                query,
                pageIndex - 1,
                pageSize
            );

            return stores;
        }

        #endregion


    }
}