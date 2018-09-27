using Nop.Core.Domain.Projects;

namespace Nop.Data.Mapping.Projects
{
    public partial class ProjectCategoryMap : NopEntityTypeConfiguration<ProjectCategory>
    {
        public ProjectCategoryMap()
        {
            ToTable("ProjectCategory");
            this.HasKey(p => p.Id);
            this.HasRequired(a => a.Category)
                .WithMany(a => a.Projects)
                .HasForeignKey(a => a.CategoryId);
            this.HasRequired(a => a.Project)
                .WithMany(a => a.Categories)
                .HasForeignKey(a => a.ProjectId);
        }
    }
}