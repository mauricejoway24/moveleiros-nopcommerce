using System;

namespace Nop.Core
{
    public abstract partial class StringKeyBaseEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public new string Id { get; set; }
    }
}
