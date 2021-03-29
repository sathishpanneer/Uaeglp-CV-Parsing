using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class TimeDiffereceView
    {
        public TimeDiffereceView()
        {
        }

        public TimeDiffereceView(int Months, int Days)
        {
            this.MonthsLeft = Months;
            this.DaysLeft = Days;
        }

        public int YearsLeft { get; set; }

        public int DaysLeft { get; set; }

        public int MonthsLeft { get; set; }
    }
}
