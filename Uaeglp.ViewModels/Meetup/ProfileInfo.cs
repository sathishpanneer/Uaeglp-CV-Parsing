using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.Meetup
{
	public class ProfileInfo
	{

        public int profileId { get; set; }
		public int? GroupProfileImageId { get; set; }

		public MeetupLangView ProfileName { get; set; }

		public string ProfileIDEncrypted { get; set; }
		public MeetupLangView Designation { get; set; }
        public string GroupProfileImageURL
        {
            get
            {
                if (GroupProfileImageId != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + GroupProfileImageId;
                }

                return null;
            }
        }
    }
}
