using Nop.Core.Domain.Projects;

namespace Nop.Data.Mapping.Projects
{
    public partial class ProjectPictureMap : NopEntityTypeConfiguration<ProjectPicture>
    {
        public ProjectPictureMap()
        {
            ToTable("ProjectPicture");
            this.HasKey(p => p.Id);
            this.HasRequired(a => a.Picture)
                .WithMany(a => a.ProjectPictures)
                .HasForeignKey(a => a.PictureId);
            this.HasRequired(a => a.Project)
                .WithMany(a => a.Pictures)
                .HasForeignKey(a => a.ProjectId);
            Ignore(a => a.UrlImage);
        }
    }
}