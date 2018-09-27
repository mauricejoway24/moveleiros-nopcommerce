using Nop.Core.Domain.Projects;

namespace Nop.Data.Mapping.Projects
{
    public partial class ProjectBestTopicMap : NopEntityTypeConfiguration<ProjectBestTopic>
    {
        public ProjectBestTopicMap()
        {
            ToTable("ProjectBestTopic");
            this.HasKey(p => p.Id);
            this.Property(p => p.TopicName).IsRequired().HasMaxLength(400);
        }
    }
}