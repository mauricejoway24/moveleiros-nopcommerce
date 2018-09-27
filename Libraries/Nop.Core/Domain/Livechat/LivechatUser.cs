using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Livechat
{
    public class LivechatUser : StringKeyBaseEntity
    {
        public int? CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<LivechatChannel> Channels { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
