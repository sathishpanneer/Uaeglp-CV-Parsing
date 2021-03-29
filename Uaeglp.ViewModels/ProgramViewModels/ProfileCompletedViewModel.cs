using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class ProfileCompletedViewModel
    {
        public int Education { get; set; }
        public int WorkExperiences{ get; set; }
        public int Trainings { get; set; }
        public int Achievements { get; set; }
       // public int Membership { get; set; }
        public bool IsCandidateInfoCompleted { get; set; }
        public bool IsAllDocumentUploaded { get; set; }
        public bool IsProgramCompleted { get; set; }
        public bool IsAssessmentCompleted { get; set; }
        public ProgramView Program { get; set; }

        public decimal ProfileCompletedPercentage { get; set; }
    }
}
