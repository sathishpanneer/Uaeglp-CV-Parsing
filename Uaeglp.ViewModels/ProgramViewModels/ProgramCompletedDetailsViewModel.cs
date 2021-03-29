using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class ProgramCompletedDetailsViewModel
    {
        public decimal ProfileCompletedPercentage { get; set; }
        public List<AssessmentDratail> ProgramPendingAssessmentDetails { get; set; }
        public List<DetailsViewModel> ProfileModuleDetails { get; set; }
        public List<DetailsViewModel> ProfileDocuments { get; set; }
        public DetailsViewModel ProfileImg { get; set; }
        public DetailsViewModel ProfileContactDetails { get; set; }
        public DetailsViewModel ProfilePersonalDetails { get; set; }   

        public ProgramView Program { get; set; }

        public bool isProgramOpen { get; set; }
    }

    public class DetailsViewModel
    {
        public string Module { get; set; }
        public int ModuleCount { get; set; }
        public bool Updatedrecently { get; set; }
        public string DocumentType { get; set; }
        public bool Available { get; set; }
    }

    public class AssessmentDratail
    {
        public string Title { get; set; }

        public bool? IsCompleted { get; set; }
    }
}
