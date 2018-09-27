using Nop.Core.Domain.Projects;
using System.Collections.Generic;

namespace Nop.Services.Projects
{
    public interface IProjectService
    {
        void InsertProject(Project project);
        IList<Project> ListMyProjects(int designerId);
        IList<ProjectBestTopic> GetBestTopics();
        Project GetById(int id);
        void EditProject(Project project);
        void RemovePictureProject(List<ProjectPicture> picturesDeleted);
    }
}
