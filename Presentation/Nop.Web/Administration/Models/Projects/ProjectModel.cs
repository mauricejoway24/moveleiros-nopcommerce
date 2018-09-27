using Nop.Core.Domain.Projects;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Projects
{
    public class ProjectModel
    {
        public ProjectModel()
        {

        }

        public ProjectModel(Project project)
        {
            Id = project.Id;
            PromobFilePath = project.PromobFile;
            Name = project.Name;
            Description = project.Description;
            CategoriesId = project.Categories.Select(a => a.CategoryId).ToList();
            ProjectPictures = project.Pictures.Select(a => new ProjectPictureModel
            {
                PictureUrl = a.UrlImage,
                PictureId = a.PictureId
            }).ToList();
            BrandId = project.BrandId;
        }

        public int? Id { get; set; }

        public HttpPostedFileBase PromobFile { get; set; }
        public string PromobFilePath { get; set; }
        public int DesignerId { get; set; }
        public int Clicks { get; set; }

        [Required(ErrorMessage = "O nome do projeto é obrigatória")]
        public string Name { get; set; }
        [Required(ErrorMessage = "A descrição do projeto é obrigatória")]
        public string Description { get; set; }
        public List<int> CategoriesId { get; set; }

        public HttpPostedFileBase[] Pictures { get; set; }
        public int[] PicturesId { get; set; }
        public List<ProjectPictureModel> ProjectPictures { get; set; }
        [Required(ErrorMessage = "A marca do projeto é obrigatória")]
        public int BrandId { get; set; }

        public Project ToEntity()
        {
            return new Project
            {
                DesignerId = DesignerId,
                Clicks = Clicks,
                Name = Name,
                Description = Description,
                ShortDescription = null,
                FullDescription = null,
                AdminComment = null,
                ProjectContains = null,
                AsideObservation = null,
                DimensionDescription = null,
                FromArea = 0,
                ToArea = 0,
                Area = 0,
                BrandId = BrandId,
                PromobFile = PromobFilePath
            };
        }
    }

    public class ProjectPictureModel
    {
        public string PictureUrl { get; set; }
        public int PictureId { get; set; }
    }
}