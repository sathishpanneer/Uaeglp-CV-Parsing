using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Meetup
{
    public class FollwGroupView
    {
        public int ID { get; set; }

        public MeetupLangView Name { get; set; }
      
        public int NumberOfFollowing { get; set; }

        public bool IsFollowed { get; set; }

    }
}
