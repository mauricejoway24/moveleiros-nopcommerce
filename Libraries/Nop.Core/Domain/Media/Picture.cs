using System.Collections.Generic;
using Nop.Core.Domain.Projects;

namespace Nop.Core.Domain.Media
{
    /// <summary>
    /// Represents a picture
    /// </summary>
    public partial class Picture : BaseEntity
    {
        private ICollection<Project> _projects;
        private ICollection<ProjectPicture> _projectPictures;

        /// <summary>
        /// Gets or sets the picture binary
        /// </summary>
        public byte[] PictureBinary { get; set; }

        /// <summary>
        /// Gets or sets the picture mime type
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the SEO friednly filename of the picture
        /// </summary>
        public string SeoFilename { get; set; }

        /// <summary>
        /// Gets or sets the "alt" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. product name)
        /// </summary>
        public string AltAttribute { get; set; }

        /// <summary>
        /// Gets or sets the "title" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. product name)
        /// </summary>
        public string TitleAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the picture is new
        /// </summary>
        public bool IsNew { get; set; }

        #region Moveleiros

        public int PictureTypeId { get; set; }

        /// <summary>
        /// Gets or sets the picture type
        /// </summary>
        public PictureType PictureType
        {
            get => (PictureType)PictureTypeId;
            set => PictureTypeId = (int)value;
        }

        /// <summary>
        /// Gets or sets the panorama
        /// </summary>
        public bool IsPanorama { get; set; }


        public ICollection<Project> Projects
        {
            get { return _projects ?? (_projects = new List<Project>()); }
            protected set { _projects = value; }
        }
        public ICollection<ProjectPicture> ProjectPictures
        {
            get { return _projectPictures ?? (_projectPictures = new List<ProjectPicture>()); }
            protected set { _projectPictures = value; }
        }
        #endregion
    }
}
