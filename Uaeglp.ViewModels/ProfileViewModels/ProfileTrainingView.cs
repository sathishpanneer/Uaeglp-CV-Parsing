using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class ProfileTrainingView : BaseProfileView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public DateTime Date { get; set; }
        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public string ModifiedBy { get; set; }
        public int ProfileId { get; set; }
        public int HaveCertificate { get; set; }
        public OrganizationView Organization { get; set; }
    }
}
