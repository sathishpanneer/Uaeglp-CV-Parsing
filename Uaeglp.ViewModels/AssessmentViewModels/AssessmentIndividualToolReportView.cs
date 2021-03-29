using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentIndividualToolReportView
	{
        public AssessmentIndividualToolReportView()
        {
            this.profileScaleScoreReportsView = new List<ProfileScaleScoreReportView>();
            this.profileScaleNarrativeDescriptionsView = new List<ProfileScaleNarrativeDescriptionView>();
            this.ProfileAnswerQuestionGroups = new List<ProfileAnswerQuestionGroupView>();
            this.profileCognitiveNarrativeDescriptionView = new ProfileCognitiveNarrativeDescriptionView();
            this.profileHighestJobNarrativeDescriptionsView = new List<ProfileScaleNarrativeDescriptionView>();
            this.profileLowestJobNarrativeDescriptionsView = new List<ProfileScaleNarrativeDescriptionView>();
        }

        public int AssessmentToolID { get; set; }

        public int AssessmnetToolCategoryID { get; set; }

        public Decimal OverallScore { get; set; }

        public int PercentileScore { get; set; }

        public EnglishArabicView AssessmentToolName { get; set; }

        public string CompletedOn { get; set; }

        public EnglishArabicView ProfileName { get; set; }

        public EnglishArabicView OverAllPercentileDescription { get; set; }

        public EnglishArabicView OverAllDescription { get; set; }

        public List<ProfileScaleScoreReportView> profileScaleScoreReportsView { get; set; }

        public List<ProfileScaleNarrativeDescriptionView> profileScaleNarrativeDescriptionsView { get; set; }

        public List<ProfileScaleNarrativeDescriptionView> profileHighestJobNarrativeDescriptionsView { get; set; }

        public List<ProfileScaleNarrativeDescriptionView> profileLowestJobNarrativeDescriptionsView { get; set; }

        public List<ProfileAnswerQuestionGroupView> ProfileAnswerQuestionGroups { get; set; }

        public ProfileCognitiveNarrativeDescriptionView profileCognitiveNarrativeDescriptionView { get; set; }

        public List<ProfilePillarScoreReportView> profilePillarScoreReportView { get; set; }

        public string ViewPath { get; set; }

        public string FileName { get; set; }
    }
}
