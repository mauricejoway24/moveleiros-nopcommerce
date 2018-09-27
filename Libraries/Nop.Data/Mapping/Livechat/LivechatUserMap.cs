using Nop.Core.Domain.Livechat;

namespace Nop.Data.Mapping.Livechat
{
    public class LivechatUserMap : NopEntityTypeConfiguration<LivechatUser>
    {
        public LivechatUserMap()
        {
            ToTable("LivechatUser");
            HasKey(t => t.Id);
        }
    }
}
