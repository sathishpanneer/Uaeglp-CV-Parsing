// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Uaeglp.LangModel
{
    public partial class TempFeedbackstates
    {
        public int Id { get; set; }
        public int? FeedbackId { get; set; }
        public int? Centerid { get; set; }
        public int? Questionid { get; set; }
        public string Screen { get; set; }
        public int? Totalscreencount { get; set; }
        public int? Currentscreenindex { get; set; }
        public int? Mandatoryscreen { get; set; }
        public string Selectedanswers { get; set; }
        public string Comments { get; set; }
        public string Employee { get; set; }
    }
}