using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class CVSaveProfileView
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string LinkedInURL { get; set; }
        public string TwitterURL { get; set; }
        public string Bio { get; set; }
        public CountryDetails Country { get; set; }
        public List<LanguageList> Language { get; set; }
        public string[] Skill { get; set; }
        public List<EducationList> Education { get; set; }
        public List<WorkExperienceList> WorkExperience { get; set; }
        
    }

    public class LanguageList
    {
        public int LanguageID { get; set; }
        public int LanguageItemId { get; set; }
        public int ProficiencyItemId { get; set; }
    }

    public class EducationList
    {
        public int EducationID { get; set; }
        public string SchoolType { get; set; }
        public string SchoolName { get; set; }
        public int DegreeTypeID { get; set; }
        public string FieldOfStudy { get; set; }
        public int CountryID { get; set; }
        public bool isStudied { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
    }

    public class WorkExperienceList
    {
        public int WorkExperienceID { get; set; }
        public string companyName { get; set; }
        public string jobTitle { get; set; }
        public int industryID { get; set; }
        public int countryID { get; set; }
        public string nextPosition { get; set; }
        public string jobDescription { get; set; }
        public bool isSomeoneReportToYou { get; set; }
        public bool isYouReportToSomeone { get; set; }
        public string managerJobTitle { get; set; }
        public int? managerJobTitleID { get; set; }
        public string jobCategory { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }


    }
}
