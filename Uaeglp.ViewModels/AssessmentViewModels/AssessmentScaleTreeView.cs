using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentScaleTreeView
	{
        public AssessmentScaleTreeView()
        {
            this.Subscales = new List<AssessmentSubscaleTreeView>();
        }

        public int ScaleID { get; set; }

        public string ScaleName { get; set; }

        public List<AssessmentSubscaleTreeView> Subscales { get; set; }
    }
}
