using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models
{
    [Table("UserCustomNotification", Schema = "app")]
    public class CustomNotification
    {
        [Key]
        public int ID { get; set; }
        public int ProfileID { get; set; }
        public int CategoryID { get; set; }
        public bool isEnabled { get; set; }

    }
}
