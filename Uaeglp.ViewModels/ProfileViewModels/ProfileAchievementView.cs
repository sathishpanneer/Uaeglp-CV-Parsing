using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class ProfileAchievementView:BaseProfileView
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int? OrgnizationId { get; set; }
        public string OrganizationName { get; set; }
        public string Role { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public DateTime Modified { get; set; }
        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public string ModifiedBy { get; set; }
        public int ProfileId { get; set; }
        public string ProjectTitleAndEvent { get; set; }
        public int VerbItemId { get; set; }
        public int? ImpactItemId { get; set; }
        public int? AwardItemId { get; set; }
        public int? MedalItemId { get; set; }
        public int? ReachedItemId { get; set; }

        public virtual LookupItemView AwardItem { get; set; }
        public virtual LookupItemView ImpactItem { get; set; }
        public virtual LookupItemView MedalItem { get; set; }
        public virtual OrganizationView Organization { get; set; }
        public virtual LookupItemView ReachedItem { get; set; }
        public virtual LookupItemView VerbItem { get; set; }
      //  public virtual ICollection<ApplicationAchievement> ApplicationAchievements { get; set; }
    }
}
