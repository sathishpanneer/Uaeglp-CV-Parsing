using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Models
{
    public class RecommendProfileView
    {
        public  string RecommendViewProfileURL { get; set; }
        public List<PublicProfileView> RecommendMatchProfileList { get; set; }
        //public PublicProfileView RecommendedBy { get; set; }

    }
}
