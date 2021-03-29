using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class BatchAssessmentToolsProfilesPV
	{
        public BatchAssessmentToolsProfilesPV()
        {
            this.Profile = new AssessmentProfileView();
            this.AssessmentTools = new List<AssessmentToolCategoryView>();
        }

        public AssessmentProfileView Profile { get; set; }

        public List<AssessmentToolCategoryView> AssessmentTools { get; set; }

        public string AssessmentToolsJason { get; set; }
    }
}
