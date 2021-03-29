using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class ProfileLearningPreferenceView
    {

        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int LearningPreferenceItemId { get; set; }
        public int ItemOrder { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public DateTime Modified { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public string ModifiedBy { get; set; }
        public LookupItemView LearningPreference { get; set; }

    }
}
