using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.Meetup;

namespace Uaeglp.ViewModels
{
    public class RecipientUserView
    {
        public int RecipientUserId { get; set; }
        public int? RecipientUserImageId { get; set; }
        public MeetupLangView RecipientName { get; set; }
        public MeetupLangView Designation { get; set; }
        public string RecipientUserImageURL
        {
            get
            {
                if (RecipientUserImageId != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + RecipientUserImageId;
                }

                return null;
            }
        }
    }
}
