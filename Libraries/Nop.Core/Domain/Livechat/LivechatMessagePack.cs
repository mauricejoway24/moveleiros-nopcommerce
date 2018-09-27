using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Livechat
{
    public class LivechatMessagePack : StringKeyBaseEntity
    {
        public string FromName { get; set; }
        public string Message { get; set; }
        public string FromConnectionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SerializedContext { get; set; }
        public string ChannelId { get; set; }
        public string LivechatUserId { get; set; }

        public virtual LivechatChannel Channel { get; set; }
    }
}
