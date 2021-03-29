using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.Meetup
{
    public class ParticipantsView
    {
        public int Userid { get; set; }
        public int? ParticipantsImageId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string ParticipantsImageURL
        {
            get
            {
                if (ParticipantsImageId != null)
                {
                    return ConstantUrlPath.ProfileImagePath + ParticipantsImageId;
                }

                return null;
            }
        }
    }
}
