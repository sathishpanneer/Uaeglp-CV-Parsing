using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class ProgramAnswerViewModel
    {
        public int BatchId { get; set; }

        public int ProfileId { get; set; }

        public List<ApplicationAnswerViewModel> Answers { get; set; } = new List<ApplicationAnswerViewModel>();
    }
}
