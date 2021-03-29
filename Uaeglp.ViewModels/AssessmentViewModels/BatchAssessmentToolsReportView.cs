using System.Collections.Generic;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class BatchAssessmentToolsReportView
    {
        public BatchAssessmentToolsReportView()
        {
            this.BatchAssessmentToolsProfiles = new ListingView<List<BatchAssessmentToolsProfilesPV>>();
            this.Counts = new BatchAssToolsReportCount();
            this.OrgDictionary = new Dictionary<int, string>();
        }

        public ListingView<List<BatchAssessmentToolsProfilesPV>> BatchAssessmentToolsProfiles { get; set; }

        public string BatchTitle { get; set; }

        public string ProgrameTitle { get; set; }

        public int ParticipantsCount { get; set; }

        public string BatchStartDate { get; set; }

        public string BatchEndDate { get; set; }

        public string DataProvider { get; set; }

        public Dictionary<int, string> OrgDictionary { get; set; }

        public BatchAssToolsReportCount Counts { get; set; }
    }
}
