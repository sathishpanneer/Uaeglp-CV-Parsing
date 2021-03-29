using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentToolCategoryView
	{
        public int AssessmentToolID { get; set; }

        public int AssessmentToolCategoryID { get; set; }

        public string AssessmentToolName { get; set; }

        public Decimal AssessmentToolWeigth { get; set; }

        public string AssessmentToolImageID { get; set; }
        public string AssessmentToolMobImageID { get; set; }

        public Decimal ProfileScore { get; set; }

        public Decimal ProfileWeigthedScore { get; set; }
    }
}
