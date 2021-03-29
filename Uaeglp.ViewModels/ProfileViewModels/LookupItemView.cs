using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class LookupItemView
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        [JsonIgnore]
        public bool Manageable { get; set; }
        public int DisplayOrder { get; set; }
        [JsonIgnore]
        public string Value { get; set; }
        public int LookupId { get; set; }
        [JsonIgnore]
        public string Discriminator { get; set; }
        [JsonIgnore]
        public string SysName { get; set; }
    }
}
