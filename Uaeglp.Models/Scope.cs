﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    public partial class Scope
    {
        public Scope()
        {
            ScopeClaims = new HashSet<ScopeClaim>();
            ScopeSecrets = new HashSet<ScopeSecret>();
        }

        [Key]
        public int Id { get; set; }
        public bool Enabled { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string DisplayName { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public int Type { get; set; }
        public bool IncludeAllClaimsForUser { get; set; }
        [StringLength(200)]
        public string ClaimsRule { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public bool AllowUnrestrictedIntrospection { get; set; }

        [InverseProperty(nameof(ScopeClaim.Scope))]
        public virtual ICollection<ScopeClaim> ScopeClaims { get; set; }
        [InverseProperty(nameof(ScopeSecret.Scope))]
        public virtual ICollection<ScopeSecret> ScopeSecrets { get; set; }
    }
}