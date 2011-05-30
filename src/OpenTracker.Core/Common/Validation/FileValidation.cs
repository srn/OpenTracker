using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenTracker.Core.Common.Validation
{
    public class FileAttribute : ValidationAttribute
    {
        public int MaxContentLength = int.MaxValue;
        public string[] AllowedFileExtensions;
        public string[] AllowedContentTypes;

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;

            //this should be handled by [Required]
            if (file == null)
                return true;

            if (file.ContentLength > MaxContentLength)
            {
                ErrorMessage = string.Format("File is too large, maximum allowed is: {0} KB", MaxContentLength / 1024);
                return false;
            }

            if (AllowedFileExtensions != null)
            {
                if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                {
                    ErrorMessage = "Please upload file of type: " + string.Join(", ", AllowedFileExtensions);
                    return false;
                }
            }

            if (AllowedContentTypes != null)
            {
                if (!AllowedContentTypes.Contains(file.ContentType))
                {
                    ErrorMessage = "Please upload file of type: " + string.Join(", ", AllowedContentTypes);
                    return false;
                }
            }

            return true;
        }
    }
}
