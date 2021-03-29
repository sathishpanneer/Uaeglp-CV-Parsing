using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class ProfileFollowingsView
    {
        public List<PublicProfileView> FollowingsList { get; set; } = new List<PublicProfileView>();
        public List<PublicProfileView> PeopleMayKnowList { get; set; } = new List<PublicProfileView>();

        public int Take { get; set; }
        public int Page { get; set; }
        public int TotalCount { get; set; }
    }

    public class ProfileFollowersView
    {
        public List<PublicProfileView> FollowersList { get; set; } = new List<PublicProfileView>();

        public int Take { get; set; }
        public int Page { get; set; }
        public int TotalCount { get; set; }
    }

    public class FavoriteProfile
    {
        public List<ProfileView> Profiles { get; set; } = new List<ProfileView>();

        //public int Take { get; set; }
        //public int Page { get; set; }
        public int TotalCount { get; set; }
    }

    public class SearchPublicProfileView
    {
        public List<MyProfileView> Recommendations { get; set; } = new List<MyProfileView>();

       // public int Take { get; set; }
        //public int Page { get; set; }
        public int TotalCount { get; set; }
    }

    public class SearchTrackApplicationView
    {
        public List<TrackApplication> TrackApplication { get; set; } = new List<TrackApplication>();

        // public int Take { get; set; }
        //public int Page { get; set; }
        //public int TotalCount { get; set; }
    }
}
