using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uaeglp.Utilities;
//using Uaeglp.MongoModels;

namespace Uaeglp.ViewModels
{
    public class RoomViewModel
    {
		////public string ID { get; set; }
		//public string RoomTitle { get; set; }
		//public int OwnerID { get; set; }
		////public int RoomTypeID { get; set; }
		////public DateTime CreatedOn { get; set; }
		////public int NumberOfMembers { get; set; }
		////public DateTime LastModifiedOn { get; set; }
		//public List<int> MembersIDs { get; set; }
		//public IList<int> ArchivedMembersIDs { get; set; }
  //      //public IList<MessageView> Messages { get; set; }
  //      //public IList<UnreadMessageView> UnreadMessages { get; set; }
  //      //public string ModifiedBy { get; set; }

        public string ID { get; set; }
        public string RoomTitle { get; set; }
        public int OwnerID { get; set; }
        public RecipientUserView RoomOwnerInfo { get; set; }
        public int RoomTypeID { get; set; }
        public DateTime CreatedOn { get; set; }
        public int NumberOfMembers { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public List<MemberModel> MembersIDs { get; set; }
        public IList<int> ArchivedMembersIDs { get; set; }
        public IList<MessageViewModel> Messages { get; set; }
        public IList<UnreadMessageView> UnreadMessages { get; set; }
        public string ModifiedBy { get; set; }

    }

    public class MemberModel
    {
        public int MembersID { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public int? MemberImageFileId { get; set; }
        public string DesignationEn { get; set; }
        public string DesignationAr { get; set; }
        public string MemberImageFileURL
        {
            get
            {
                if (MemberImageFileId != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + MemberImageFileId;
                }

                return null;
            }
        }
    }
    public class MessageViewModel
    {

        public string ID { get; set; }
        public int OwnerID { get; set; }
        public RecipientUserView OwnerInfo { get; set; }
        public string MessageText { get; set; }
        public int TypeID { get; set; }
        public IList<string> FilesIDs { get; set; }
        public IList<string> ImagesIDs { get; set; }
        public DateTime Created { get; set; }
        public IList<int> SeenByIDs { get; set; }
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

    }
}
