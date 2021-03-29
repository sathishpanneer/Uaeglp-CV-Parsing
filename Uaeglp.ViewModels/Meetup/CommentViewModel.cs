using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.Meetup
{
    public class CommentViewModel
    {
        public string ID { get; set; }
        public int UserID { get; set; }
        public string Text { get; set; }
        public int TypeID { get; set; }
        public List<string> FilesIDs { get; set; }
        public List<string> ImagesIDs { get; set; }
        public string FileName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAdminCreated { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Modified { get; set; } = DateTime.UtcNow;
        public string ImageFileUrl
        {
            get
            {
                if (!ImagesIDs.Any())
                {
                    return "";
                }

                return ConstantUrlPath.PostFileDownloadPath + ImagesIDs.FirstOrDefault();
            }
        }

        public string DocumentFileUrl
        {
            get
            {
                if (!FilesIDs.Any())
                {
                    return "";
                }

                return ConstantUrlPath.PostFileDownloadPath + FilesIDs.FirstOrDefault();
            }
        }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public int MeetupID { get; set; }
        public int? UserImageFileId { get; set; }
        public string ProfileImageUrl
        {
            get
            {
                if (UserImageFileId != null)
                {
                    return ConstantUrlPath.ProfileImagePath + UserImageFileId;
                }

                return null;
            }
        }
    }
}
