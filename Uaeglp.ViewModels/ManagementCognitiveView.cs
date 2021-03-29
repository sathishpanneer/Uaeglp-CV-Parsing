namespace Uaeglp.ViewModels
{
	public class ManagementCognitiveView
	{
        public int OverallScore { get; set; }

        public int NumericalScore { get; set; }

        public int VerbalScore { get; set; }

        public int AbstractScore { get; set; }

        public int TimeTakenScore { get; set; }

        public EnglishArabicView OverallScoreNarrativeDescription { get; set; }

        public EnglishArabicView NumericalScoreNarrativeDescription { get; set; }

        public EnglishArabicView VerbalScoreNarrativeDescription { get; set; }

        public EnglishArabicView AbstractScoreNarrativeDescription { get; set; }

        public EnglishArabicView TimetakenScoreNarrativeDescription { get; set; }
    }
}
