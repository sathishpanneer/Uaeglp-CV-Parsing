using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.AssessmentViewModels;

namespace Uaeglp.ViewModels
{
	public class ScaleView
	{
        public ScaleView()
        {
            this.SubScales = new List<SubScaleView>();
            this.QuestionItems = new List<QuestionItemView>();
            this.NarrativeLowDescription = new EnglishArabicView();
            this.NarrativeBelowAverageDescription = new EnglishArabicView();
            this.NarrativeAverageDescription = new EnglishArabicView();
            this.NarrativeAboveAverageDescription = new EnglishArabicView();
            this.NarrativeHighDescription = new EnglishArabicView();
        }

        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public EnglishArabicView Name { get; set; }

        public EnglishArabicView LowDescription { get; set; }

        public EnglishArabicView HighDescription { get; set; }

        public EnglishArabicView NarrativeLowDescription { get; set; }

        public EnglishArabicView NarrativeBelowAverageDescription { get; set; }

        public EnglishArabicView NarrativeAverageDescription { get; set; }

        public EnglishArabicView NarrativeAboveAverageDescription { get; set; }

        public EnglishArabicView NarrativeHighDescription { get; set; }

        public EnglishArabicView HighestJobDescirption { get; set; }

        public EnglishArabicView LowestJobDescirption { get; set; }

        public int AssessmentToolCategoryID { get; set; }

        public int FactorID { get; set; }

        public string FactroName { get; set; }

        public List<SubScaleView> SubScales { get; set; }

        public List<QuestionItemView> QuestionItems { get; set; }

        public Decimal? Mean { get; set; }

        public Decimal? StandardDeviation { get; set; }
    }
}
