using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class LeadershipPointSystemView
    {
        public int ProfileId { get; set; }
        public int CurrentPoints { get; set; }

        public int RemainingPoints
        {
            get
            {
                return (from point in Badges.Select(k => k.MinimumPoints).OrderBy(k => k) where point > CurrentPoints select point - CurrentPoints).FirstOrDefault();
            }
        }
        public int NextTargetPoints
        {
            get
            {
                return (from point in Badges.Select(k => k.MinimumPoints).OrderBy(k => k) where point > CurrentPoints select point ).FirstOrDefault();
            }
        }
        public List<BadgeView> Badges { get; set; } = new List<BadgeView>();
        public string ProfileImageUrl => $@"/api/File/get-profileImage/{ProfileId}";
        public List<CriteriaView> CriteriaList { get; set; }


    }
}
