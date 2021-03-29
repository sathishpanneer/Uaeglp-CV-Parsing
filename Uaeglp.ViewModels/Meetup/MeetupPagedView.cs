using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.Meetup
{
	public class MeetupPagedView
    {
        public int GroupID { get; set; }

        public string GroupIDEncrypted { get; set; }

        public int? GroupImageID { get; set; }

        public MeetupLangView GroupName { get; set; }

        public MeetupLangView GroupDescription { get; set; }

        public int NumberOfFollowing { get; set; }

        public bool? IsFollowedGroup { get; set; }

        public List<ProfileInfo> profileInfos { get; set; }

        public List<MeetupView> MeetupList { get; set; }

        public int ModelCount { get; set; }

        public PagingView pagingView { get; set; }

        public string CoverImageURL
        {
            get
            {
                if (GroupImageID != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + GroupImageID;
                }

                return null;
            }
        }
        public MeetupPagedView()
        {
            this.MeetupList = new List<MeetupView>();
            this.profileInfos = new List<ProfileInfo>();
        }
    }
}
