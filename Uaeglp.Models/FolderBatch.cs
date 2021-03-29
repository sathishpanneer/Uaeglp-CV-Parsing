﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("FolderBatch")]
    public partial class FolderBatch
    {
        [Key]
        [Column("Folder_ID")]
        public int FolderId { get; set; }
        [Key]
        [Column("Batch_ID")]
        public int BatchId { get; set; }

        [ForeignKey(nameof(BatchId))]
        [InverseProperty("FolderBatches")]
        public virtual Batch Batch { get; set; }
        [ForeignKey(nameof(FolderId))]
        [InverseProperty("FolderBatches")]
        public virtual Folder Folder { get; set; }
    }
}