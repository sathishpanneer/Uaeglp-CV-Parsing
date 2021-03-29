using System;

namespace Uaeglp.ViewModels.ProfileViewModels
{
	public class ProfileCognitiveNarrativeDescriptionView
	{
        public Decimal Score { get; set; }

        public EnglishArabicView OverallScoreName { get; set; }

        public EnglishArabicView OverallScoreNarrativeDescription { get; set; }

        public EnglishArabicView NumericalScoreName { get; set; }

        public EnglishArabicView NumericalScoreNarrativeDescription { get; set; }

        public EnglishArabicView VerbalScoreName { get; set; }

        public EnglishArabicView VerbalScoreNarrativeDescription { get; set; }

        public EnglishArabicView AbstractScoreName { get; set; }

        public EnglishArabicView AbstractScoreNarrativeDescription { get; set; }
    }
}
