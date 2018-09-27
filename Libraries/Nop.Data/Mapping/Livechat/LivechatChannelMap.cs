using Nop.Core.Domain.Livechat;

namespace Nop.Data.Mapping.Livechat
{
    public class LivechatChannelMap : NopEntityTypeConfiguration<LivechatChannel>
    {
        public LivechatChannelMap()
        {
            ToTable("LivechatChannel");
            HasKey(t => t.Id);

            HasMany(t => t.Users)
                .WithMany(c => c.Channels)
                .Map(t =>
                {
                    t.MapLeftKey("LivechatChannelId");
                    t.MapRightKey("LivechatUserId");
                    t.ToTable("LivechatChannelUser");
                });

            HasMany(t => t.Messages);
        }
    }
}
