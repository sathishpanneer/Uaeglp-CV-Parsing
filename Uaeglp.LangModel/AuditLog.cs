﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Uaeglp.LangModel
{
    public partial class AuditLog
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string Ipaddress { get; set; }
        public string Activity { get; set; }
        public DateTime? Loggeddate { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; }
    }
}