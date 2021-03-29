using System;
using MongoDB.Bson;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels
{
	public class CommentView
    {
		public string Id { get; set; }
		public int UserID { get; set; }
		public string Text { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsAdminCreated { get; set; }
        public string JobTitle { get; set; }
		public DateTime Created { get; set; } = DateTime.Now;
		public DateTime Modified { get; set; } = DateTime.Now;
		public string NameEn { get; set; }
		public string NameAr { get; set; }
		public string TitleEn { get; set; }
		public string TitleAr { get; set; }
        public string PostId { get; set; }
		public int UserImageFileId { get; set; }
		public string ProfileImageUrl => ConstantUrlPath.ProfileImagePath + UserImageFileId;

	}
}
	