using Nop.Core.Domain.Stores;

namespace Nop.Core
{
    /// <summary>
    /// Store context
    /// </summary>
    public interface IStoreContext
    {
        /// <summary>
        /// Gets or sets the current store
        /// </summary>
        Store CurrentStore { get; }

        #region Moveleiros

        /// <summary>
        /// Gets whether store is supreme or not
        /// </summary>
        bool IsSupremeStore { get; }

        #endregion
    }
}
