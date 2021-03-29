using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.Meetup
{
    public class GroupView
    {
        public GroupView()
        {
            this.Tags = new List<TopicView>();
        }

        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public MeetupLangView Name { get; set; }

        public MeetupLangView Description { get; set; }

        public int? CoverImgID { get; set; }
        public int? MobCoverImageFileID { get; set; }
        public IFormFile CoverImage { get; set; }

        public int NumberOfFollowing { get; set; }

        public AutocompleteView TagAutoComplete { get; set; }

        public List<TopicView> Tags { get; set; }

        public List<int> AdminIDs { get; set; }

        public bool CanEdit { get; set; }

        public bool? IsFollowed { get; set; }

        public MeetupLangView GroupDescription { get; set; }

        public MeetupLangView GroupName { get; set; }
        public string CoverImageURL
        {
            get
            {
                if (MobCoverImageFileID != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + MobCoverImageFileID;
                }

                return null;
            }
        }
    }
}
