using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class ProfileSectionsPercentageDistributionVM
    {
        public ProfileSectionsPercentageDistributionVM()
        {
            this.SectionsWithPercentage = new List<ProfileSectionsPercentageDistributionView>();
        }

        public List<ProfileSectionsPercentageDistributionView> SectionsWithPercentage { get; set; }

        public Decimal Percentage { get; set; }
    }
}
