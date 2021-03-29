using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels.ProfileViewModels
{
     public class ProfileWorkExperienceView : BaseProfileView
    {

        public int Id { get; set; }

        public int ProfileId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string OrganizationName { get; set; }

        public int OrganizationId { get; set; }

        public int IndustryId { get; set; }
        public IndustryView Industry { get; set; }

        public int CountryId { get; set; }
        public CountryView Country { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public DateTime Modified { get; set; }
        [JsonIgnore]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        public string ModifiedBy { get; set; }

        public string JobTitle { get; set; }

        public int? EmirateItemId { get; set; }
        public LookupItemView EmirateItem { get; set; }
 
        public int? FieldOfworkId { get; set; }
        public string FieldOfWorkString { get; set; }

        public WorkFieldView FieldOfWork { get; set; }
  
        public int? TitleId { get; set; }

        public int? LineManagerTitleId { get; set; }

        public string LineManagerTitle { get; set; }
        public bool IsYouReportToSomeone { get; set; }
        public bool IsSomeoneReportToYou { get; set; }
        public string NextPosition { get; set; }
        public string JobDescription { get; set; }

        public ProfileWorkExperienceJobTitleView ExperienceJobTitleView { get; set; }
        public ProfileWorkExperienceJobTitleView LineManagerTitleView { get; set; }

        public OrganizationView Organization { get; set; }
    }
}
