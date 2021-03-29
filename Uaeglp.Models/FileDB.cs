using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models
{

    [Table("File")]
    public class FileDB
    {
        [Key]
        [Column("ID")]
        public Guid Id { get; set; }
        
        public byte[] Bytes { get; set; }
    }
}
