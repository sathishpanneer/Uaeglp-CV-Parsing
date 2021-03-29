using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class AssignedAssessmentView
    {
        public AssignedAssessmentView()
        {
            this.Name = new EnglishArabicView();
        }

        public string IDEnc { get; set; }

        public EnglishArabicView Name { get; set; }

        public int ProfileID { get; set; }

        public string ProfileIDEnc { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public int AssessmentToolMatrixID { get; set; }

        public string AssessmentToolMatrixTitleAR { get; set; }

        public string AssessmentToolMatrixTitleEN { get; set; }

        public DateTime? AssessmentCompletionDatetime { get; internal set; }

        public int ID { get; set; }

        public bool ProfileStarted { get; set; }
    }
}
