using Nop.Core.Data;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Projects;
using Nop.Services.Media;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Nop.Services.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<ProjectBestTopic> _projectBestTopicRepository;
        private readonly IRepository<ProjectPicture> _projectPictureRepository;
        private readonly IPictureService _pictureService;

        public ProjectService(IRepository<Project> projectRepository,
            IRepository<ProjectBestTopic> projectBestTopicRepository,
            IRepository<ProjectPicture> projectPictureRepository,
            IPictureService pictureService)
        {
            _projectRepository = projectRepository;
            this._pictureService = pictureService;
            this._projectBestTopicRepository = projectBestTopicRepository;
            this._projectPictureRepository = projectPictureRepository;
        }

        public void EditProject(Project project)
        {
            _projectRepository.Update(project);
        }

        public IList<ProjectBestTopic> GetBestTopics()
        {
            return _projectBestTopicRepository.Table.ToList();
        }

        public Project GetById(int id)
        {
            var project = _projectRepository
                .Table
                .Include(a => a.Pictures)
                .Include("Pictures.Picture")
                .FirstOrDefault(a => a.Id == id);

            foreach (var picture in project.Pictures)
                picture.UrlImage = _pictureService.GetPictureUrl(picture.Picture);

            return project;
        }

        public void InsertProject(Project project)
        {
            project.Active = true;
            _projectRepository.Insert(project);
        }

        public IList<Project> ListMyProjects(int designerId)
        {
            var projects = _projectRepository
                .Table
                .Include(a => a.Pictures)
                .Include("Pictures.Picture")
                .Where(a => a.DesignerId == designerId && a.Active)
                .ToList();

            foreach (var project in projects)
                foreach (var picture in project.Pictures)
                    picture.UrlImage = _pictureService.GetPictureUrl(picture.Picture);

            return projects;
        }

        public void RemovePictureProject(List<ProjectPicture> picturesDeleted)
        {
            _projectPictureRepository.Delete(picturesDeleted);
        }
    }
}
