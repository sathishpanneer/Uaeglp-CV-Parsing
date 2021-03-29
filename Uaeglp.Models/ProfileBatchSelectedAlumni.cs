using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models
{
    [Table("ProfileBatchSelectedAlumni", Schema = "dbo")]
    public class ProfileBatchSelectedAlumni
    {
        [Key]
        public int BatchID { get; set; }
        [Key]
        public int ProfileID { get; set; }
    }
}
