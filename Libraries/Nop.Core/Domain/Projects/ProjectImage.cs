using Nop.Core.Domain.Media;

namespace Nop.Core.Domain.Projects
{
    public class ProjectPicture : BaseEntity
    {
        public int ProjectId { get; set; }
        public int PictureId { get; set; }
        public string UrlImage { get; set; }

        public Project Project { get; set; }
        public Picture Picture { get; set; }
    }
}
