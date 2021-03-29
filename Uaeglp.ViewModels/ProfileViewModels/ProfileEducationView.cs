using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class ProfileEducationView : BaseProfileView
    {


        public int Id { get; set; }
        public int ProfileId { get; set; }
        public bool IsStudied { get; set; }
        public int LevelOfEducation => (FieldOfStudyId == null || Title.Equals("Title", StringComparison.InvariantCultureIgnoreCase)) ? 0 : 1;
        public int DegreeLookupItemId { get; set; }
        
        public string Title { get; set; }
        public string FieldOfStudyString { get; set; }
        public int? FieldOfStudyId { get; set; }
        public ProfileEducationFieldOfStudyView FieldOfStudy { get; set; }
        public LookupItemView DegreeItem { get; set; }
        public int? EmirateItemId { get; set; }
        public LookupItemView EmirateItem { get; set; }
        public string OrganizationName { get; set; }
        public int OrganizationId { get; set; }

        public OrganizationView Organization { get; set; }

        public int CountryId { get; set; }
        public CountryView Country { get; set; }
        public string Year { get; set; }
    }
}
