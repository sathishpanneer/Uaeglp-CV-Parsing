using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Contracts;
using Uaeglp.Services;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Tests
{
    [TestClass]
    public class TestProfileAssessmentService
    {
        private readonly IProfileAssessmentService _assessmentService;
        private readonly IProgramService _programService;
        public TestProfileAssessmentService()
        {
            var services = new ContainerResolver().Container;
            _assessmentService = (IProfileAssessmentService)services.GetService(typeof(IProfileAssessmentService));
            _programService  = (IProgramService)services.GetService(typeof(IProgramService));
        }

        [TestMethod]
       public void TestAddOrUpdateProfileEducation_ItShouldSetAddProfileEducation_Success()
        {
            var model  = new ProfileEducationView()
            {
                ProfileId = 15996,
                OrganizationName = "New Test Org",
                IsStudied = true,
                CountryId = 10,
                DegreeLookupItemId = 56001,
                OrganizationId = 10,
                Year = "2019",
                FieldOfStudyString = "Field of study 1"
            };

            var data = _assessmentService.AddOrUpdateProfileEducationAsync(model).Result;
            Assert.IsNotNull(data);
        }


       [TestMethod]
       public void TestDeleteProfileEducation_ItShouldSetAddProfileEducation_Success()
       {





            var mm = _programService.GetCompletedProgramAsync(15922).Result;

           var id = 8858;
           var data = _assessmentService.DeleteProfileEducationAsync(id).Result;
           Assert.IsNotNull(data);
       }

        [TestMethod]
       public void TestAddOrUpdateProfileWorkExperience_ItShouldSetAddOrUpdateProfileWorkExperience_Success()
        {
            var model = new ProfileWorkExperienceView()
            {
                ProfileId = 13594,
                OrganizationName = "New Test Org",
                JobTitle = "New Job",
                DateFrom = DateTime.Now.AddYears(-20),
                CountryId = 10,
                IndustryId = 10
            };

            var data = _assessmentService.AddOrUpdateProfileWorkExperienceAsync(model).Result;

            Assert.IsNotNull(data);
        }


       [TestMethod]
       public void TestAddProfileSkills_ItShouldSetAddProfileSkills_Success()
       {
           var model = new SkillAndInterestView()
           {
               ProfileId = 13594,
               InterestedItems = new List<LookupItemView>()
               {
                   new LookupItemView()
                   {
                       Id = 74001
                   },
                   new LookupItemView()
                   {
                       Id = 74002
                   }
               },
               ProfileSkillItems = new List<ProfileSkillView>()
               {
                   new ProfileSkillView()
                   {
                       Id = 4,
                       Name = "New 1",
                   },
                   new ProfileSkillView()
                   {
                       Id=5,
                       Name = "New 2",
                   },
                   new ProfileSkillView()
                   {
                       Id = 7,
                       Name = "New 3",
                   }
               }
           };

           var data = _assessmentService.AddOrUpdateSkillsAsync(model).Result;

           Assert.IsNotNull(data);
       }

       

    }
}
