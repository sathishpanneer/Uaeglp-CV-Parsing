using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class ProfileMembershipView
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public DateTime Date { get; set; }
        public string Role { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public DateTime Modified { get; set; }
        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public string ModifiedBy { get; set; }

        public OrganizationView Organization { get; set; }
    }
}
