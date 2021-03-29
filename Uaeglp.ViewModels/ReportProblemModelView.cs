using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class ReportProblemModelView
    {
        public int ReportID { get; set; }
        public int UserID { get; set; }
        public string ReportDescription { get; set; }
        public int ReportFileID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
