﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    public partial class ClientClaim
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Type { get; set; }
        [Required]
        [StringLength(250)]
        public string Value { get; set; }
        [Column("Client_Id")]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        [InverseProperty("ClientClaims")]
        public virtual Client Client { get; set; }
    }
}