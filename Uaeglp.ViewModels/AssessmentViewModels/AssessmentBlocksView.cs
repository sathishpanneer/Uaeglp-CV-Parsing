using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentBlocksView
	{
        public List<AssessmentBlockView> AssessmentBlocks { get; set; }

        public string AssessmentToolName { get; set; }

        public string AssessmentToolIDEncrypted { get; set; }

        public int SubAssessmentID { get; set; }

        public string SubAssessmentIDEncrypted { get; set; }

        public string SubAssessmentName { get; set; }

        public int ModelCount { get; set; }

        public PagingView PagingDTO { get; set; }
    }
}
