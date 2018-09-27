using Nop.Core.Domain.Livechat;

namespace Nop.Data.Mapping.Livechat
{
    public class LivechatMessagePackMap : NopEntityTypeConfiguration<LivechatMessagePack>
    {
        public LivechatMessagePackMap()
        {
            ToTable("LivechatMessagePack");
            HasKey(t => t.Id);
        }
    }
}
