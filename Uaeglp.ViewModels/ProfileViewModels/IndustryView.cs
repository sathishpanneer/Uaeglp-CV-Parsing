using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class IndustryView
    {
        public int Id { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public DateTime Modified { get; set; }
        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public string ModifiedBy { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
    }
}
