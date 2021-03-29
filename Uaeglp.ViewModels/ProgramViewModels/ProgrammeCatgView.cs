using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProgramViewModels
{
	public class ProgrammeCatgView
	{
        public ProgrammeCatgView()
        {
            this.Programmes = new List<ProgWithStatusView>();
        }

        public string Catg { get; set; }

        public List<ProgWithStatusView> Programmes { get; set; }
    }
}
