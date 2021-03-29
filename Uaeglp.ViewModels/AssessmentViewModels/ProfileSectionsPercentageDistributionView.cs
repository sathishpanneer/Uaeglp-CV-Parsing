using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class ProfileSectionsPercentageDistributionView
    {
        public ProfileSectionPercentage Section { get; set; }

        public Decimal Percentage { get; set; }

        public bool Done { get; set; }
    }
}
