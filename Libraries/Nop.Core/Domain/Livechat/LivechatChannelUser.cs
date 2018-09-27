
namespace Nop.Core.Domain.Livechat
{
    public class LivechatChannelUser : StringKeyBaseEntity
    {
        public string LivechatUserId { get; set; }
        public string LivechatChannelId { get; set; }

        public virtual LivechatUser User { get; set; }
        public virtual LivechatChannel Channel { get; set; }
    }
}
