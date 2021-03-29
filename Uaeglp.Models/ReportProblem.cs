using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models
{
    [Table("ReportProblem", Schema = "app")]
    public class ReportProblem
    {
        [Key]
        public int ReportID { get; set; }
        public int UserID { get; set; }
        public string ReportDescription { get; set; }
        public int ReportFileID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        //[ForeignKey(nameof(ReportFileID))]
        //[InverseProperty(nameof(File.ReportProblems))]
        //public virtual File ReportFile { get; set; }
    }
}
