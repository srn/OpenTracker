using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using OpenTracker.Core;
using OpenTracker.Core.Common;
using OpenTracker.Core.Common.Validation;

namespace OpenTracker.Models.Tracker
{
    public class UploadModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [File(
            AllowedFileExtensions = new[] { ".torrent" }, 
            MaxContentLength = 1024 * 1024 * 8, // 8196 kB
            ErrorMessage = "Invalid File"
        )]
        [Display(Name = ".torrent file:")]
        public HttpPostedFileBase TorrentFile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TorrentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [File(
            AllowedFileExtensions = new[] { ".nfo" },
            MaxContentLength = 1024 * 1024 * 8, // 8196 kB
            ErrorMessage = "Invalid File"
        )]
        [Display(Name = ".nfo file:")]
        public HttpPostedFileBase NFO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CategoryId { get; set; }

        private List<SelectListItem> _categories = new List<SelectListItem>();
        public List<SelectListItem> Categories
        {
            get
            {
                using (var db = new OpenTrackerDbContext())
                {
                    var ctgrs = (from tor in db.categories
                                       select tor).ToList();
                    foreach (var category in ctgrs)
                    {
                        _categories.Add(new SelectListItem { Text = category.name, Value = category.id.ToString() });
                    }
                    return _categories;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SmallDescription { get; set; }
    }

}