using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Livechat
{
    public class LivechatChannel : StringKeyBaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public bool IsFinished { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }

        public virtual ICollection<LivechatMessagePack> Messages { get; set; }
        public virtual ICollection<LivechatUser> Users { get; set; }
    }
}
