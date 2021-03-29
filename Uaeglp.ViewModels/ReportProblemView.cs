using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class ReportProblemView
    {
        public int UserID { get; set; }
        public string ReportDescription { get; set; }
        //public int ReportFileID { get; set; }
        public IFormFile ReportFile { get; set; }
    }
}
