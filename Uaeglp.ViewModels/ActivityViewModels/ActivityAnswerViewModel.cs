using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.ViewModels.ActivityViewModels
{
    public class ActivityAnswerViewModel
    {
        public int ActivityId { get; set; }

        public int ProfileId { get; set; }

        public List<ApplicationAnswerViewModel> Answers { get; set; } = new List<ApplicationAnswerViewModel>();
    }
}
