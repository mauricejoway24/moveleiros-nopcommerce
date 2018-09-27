namespace Nop.Core.Domain.Projects
{
    public class ProjectBestTopic : BaseEntity
    {
        public string TopicName { get; set; }
        public int Order { get; set; }
    }
}