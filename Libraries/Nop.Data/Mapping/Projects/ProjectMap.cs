using Nop.Core.Domain.Projects;

namespace Nop.Data.Mapping.Projects
{
    public partial class ProjectMap : NopEntityTypeConfiguration<Project>
    {
        public ProjectMap()
        {
            ToTable("Project");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(a => a.Description).IsRequired();
            this.Property(a => a.PromobFile).IsRequired();
            this.HasRequired(a => a.Designer)
                .WithMany(a => a.Projects)
                .HasForeignKey(a => a.DesignerId);
            this.HasRequired(a => a.Brand)
                .WithMany(a => a.Projects)
                .HasForeignKey(a => a.BrandId);
        }
    }
}