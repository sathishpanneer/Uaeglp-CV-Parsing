using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels.ProfileViewModels
{
   public class OrganizationView
    {

        public int Id { get; set; }
        [JsonIgnore]
        public int? OrganizationSectorTypeItemId { get; set; }
        [JsonIgnore]
        public int? OrganizationScaleItemId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        [JsonIgnore]
        public int? OrganizationTypeItemId { get; set; }
        [JsonIgnore]
        public bool IsFromTop200Qsrank { get; set; }
        [JsonIgnore]
        public bool IsUaepriority { get; set; }
        [JsonIgnore]
        public int? OrganizationIndustryItemId { get; set; }
        [JsonIgnore]
        public int? OrganizationEmirateLookupItemId { get; set; }
        public int? LogoId { get; set; }
        public string LogoURLPath => $@"/api/File/get-download-document/{LogoId}";
    }
}
