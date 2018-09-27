using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Stores;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store service interface
    /// </summary>
    public partial interface IStoreService
    {
        /// <summary>
        /// Deletes a store
        /// </summary>
        /// <param name="store">Store</param>
        void DeleteStore(Store store);

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <returns>Stores</returns>
        IList<Store> GetAllStores();

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Store</returns>
        Store GetStoreById(int storeId);

        /// <summary>
        /// Gets a store by its ProfileShortUrl
        /// </summary>
        /// <param name="shortUrl">Short url</param>
        /// <returns>Store</returns>
        Store GetStoreByProfileShorUrl(string shortUrl);

        /// <summary>
        /// Inserts a store
        /// </summary>
        /// <param name="store">Store</param>
        void InsertStore(Store store);

        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="store">Store</param>
        void UpdateStore(Store store);

        /// <summary>
        /// Gets store' chat script
        /// </summary>
        /// <param name="storeId">Store Id</param>
        /// <returns>Chat Script</returns>
        string GetChatByStoreId(int storeId);

        /// <summary>
        /// Gets store' given LojistaId
        /// </summary>
        /// <param name="lojistaId">Lojista Id</param>
        /// <returns>Store </returns>
        Store GetStoreByLojistaId(int lojistaId);

        /// <summary>
        /// Gets paged Stores
        /// </summary>
        /// /// <param name="model">Store filter</param>
        /// <param name="pageIndex">Page to look for</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Stores</returns>
        IPagedList<Store> GetPagedStores(Store model, int pageIndex = 0, int pageSize = int.MaxValue);

        #region Extensions by QuanNH
        IList<Store> GetStoreNameById(int[] storeId);
        IList<Store> GetAllStoresByEntityName(int entityId, string entityName);
        #endregion
    }
}