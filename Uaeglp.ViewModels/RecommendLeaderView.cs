using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class RecommendLeaderView
    {
        //public int? RecommendID { get; set; }
        public string FullName { get; set; }
        public string RecommendingText { get; set; }
        //public int? RecommendingAudioID { get; set; }
        //public int? RecommendingVideoID { get; set; }
        public string Occupation { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string LinkedinURL { get; set; }
        public string TwitterURL { get; set; }
        public string InstagramURL { get; set; }
        public int? ProfileID { get; set; }
        public IFormFile AudioFile { get; set; }
        public IFormFile VideoFile { get; set; }
        public List<int?> RecommendLeaderFit { get; set; }
        public string OthersText { get; set; }
        public int? SourceItemID { get; set; }
    }
}
