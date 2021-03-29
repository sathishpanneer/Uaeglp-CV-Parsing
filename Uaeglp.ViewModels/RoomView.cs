using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Uaeglp.ViewModels
{
	public class RoomView
	{
		public string ID { get; set; }
		public string RoomTitle { get; set; }
		public int OwnerID { get; set; }
		public int RoomTypeID { get; set; }
		public DateTime CreatedOn { get; set; }
		public int NumberOfMembers { get; set; }
		public DateTime LastModifiedOn { get; set; }
		public List<int> MembersIDs { get; set; }
		public IList<int> ArchivedMembersIDs { get; set; }
		public IList<MessageView> Messages { get; set; }
		public IList<UnreadMessageView> UnreadMessages { get; set; }
		public string ModifiedBy { get; set; }
		public List<MemberModel> MemberDetails { get; set; }
	}
	public class MessageView
	{
		
		public string ID { get; set; }
		public int OwnerID { get; set; }
		public string MessageText { get; set; }
		public int TypeID { get; set; }
		public IList<string> FilesIDs { get; set; }
		public IList<string> ImagesIDs { get; set; }
		public DateTime Created { get; set; }
		public IList<int> SeenByIDs { get; set; }
		public string DocumentFileName { get; set; }
		public IFormFile DocumentData { get; set; }
		public IFormFile ImageData { get; set; }


	}
	public class UnreadMessageView
	{
		
		public string ID { get; set; }
		public int UserID { get; set; }
		public int MessagesCount { get; set; }
	}
}
