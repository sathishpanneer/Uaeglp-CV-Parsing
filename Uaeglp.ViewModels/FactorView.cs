using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
	public class FactorView
	{
        public FactorView()
        {
            this.Scales = new List<ScaleView>();
        }

        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public EnglishArabicView Name { get; set; }

        public int AssessmentToolID { get; set; }

        public string AssessmentToolName { get; set; }

        public List<ScaleView> Scales { get; set; }

        public Decimal? Mean { get; set; }

        public Decimal? StandardDeviation { get; set; }
    }
}
