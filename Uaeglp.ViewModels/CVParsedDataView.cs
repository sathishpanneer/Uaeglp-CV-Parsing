using Sovren.External;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace Uaeglp.ViewModels
{
    public class CVParsedDataView
    {
        public int UserId { get; set; }
        public string CVName { get; set; }
        public string CVFileType { get; set; }
        public DateTime CVParsingUpdatedDate { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string LinkedInURL { get; set; }
        public string TwitterURL { get; set; }
        public CountryDetails Country { get; set; }
        public List<EducationDetails> Education { get; set; }
        public List<WorkExperienceDetails> WorkExperience { get; set; }
        public List<string> Skill { get; set; }
        public List<LanguageDetails> Language { get; set; }
        public string Bio { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public string CustomMessage { get; set; }
    }

    public class LanguageDetails
    {
        public string LanguageCode { get; set; }
        public int LanguageItemId { get; set; }
        public string LanguageName { get; set; }
        public int ProficiencyItemId { get; set; }
        public string ProficiencyName { get; set; }
    }

    public class CountryDetails
    {
        public string countryCode { get; set; }
        public string countryName { get; set; }
    }

    public class EducationDetails
    {
        public string SchoolType { get; set; }
        public string SchoolName { get; set; }
        public int DegreeTypeID { get; set; }
        public string DegreeName { get; set; }
        public string DegreeType { get; set; }
        public string FieldOfStudy { get; set; }
        public int CountryID { get; set; }
        public bool isStudied { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
    }

    public class WorkExperienceDetails
    {
        public string companyName { get; set; }
        public string jobTitle { get; set; }
        public int CountryID { get; set; }
        public string description { get; set; }
        public string jobCategory { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int industryID { get; set; }
        public string nextPosition { get; set; }
        public bool isSomeoneReportToYou { get; set; }
        public bool isYouReportToSomeone { get; set; }
        public string managerJobTitle { get; set; }
        public int? managerJobTitleID { get; set; }


    }
}
