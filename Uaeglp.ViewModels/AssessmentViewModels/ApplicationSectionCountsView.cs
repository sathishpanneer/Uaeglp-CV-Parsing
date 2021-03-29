using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class ApplicationSectionCountsView
    {
        public int EducationCount { get; set; }

        public int WorkExperienceCount { get; set; }

        public int MembershipCount { get; set; }

        public bool CandidateInformationCompleted { get; set; }

        public bool AssessmentToolDone { get; set; }

        public int TrainingCount { get; set; }

        public int AchievementCount { get; set; }

        public int ProgramDetailsCount { get; set; }

        public int AttachmentCount { get; set; }
    }
}
