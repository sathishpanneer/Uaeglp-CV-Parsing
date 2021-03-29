using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class OptionViewModel
    {
        public int Id { get; set; }
        public string TextEn { get; set; }
        public string TextAr { get; set; }
        public int? Value { get; set; }
        public int? QuestionId { get; set; }
    }
}
