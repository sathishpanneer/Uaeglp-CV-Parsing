using System.Collections.Generic;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class AssessmentToolReportViewModel
    {
        public int CompletedCount { get; set; }

        public int InProgressCount { get; set; }

        public List<AssessmentToolReportView> AssessmentToolReport { get; set; }

        public int AssessmeentToolID { get; set; }

        public int TypeID { get; set; }

        public string AssessmeentToolIDEncrypted { get; set; }

        public string ProfileIDEncrypted { get; set; }

        public int ProfileID { get; set; }
    }
}
