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
    public class TestProfileService
    {
        private readonly IProfileService _profileService;
        private readonly IEncryptionManager _encryption;
        public TestProfileService()
        {
            var services = new ContainerResolver().Container;
            _profileService =(IProfileService)services.GetService(typeof(IProfileService));
            _encryption = (IEncryptionManager)services.GetService(typeof(IEncryptionManager));
        }



       [TestMethod]
       public void GetPersonalInfo_ItShouldReturnPersonalInfoDetails_Success()
       {
           var userId = 4;

            var mm = _profileService.GetPersonalInfoAsync(userId).Result;

            Assert.IsNotNull(mm);
       }

       [TestMethod]
       public void UpdatePersonalInfo_ItShouldUpdatePersonalInfoDetails_Success()
       {
           var userId =new PersonalInfoView()
           {
               UserId = 13600,
               BirthDate = new DateTime(1978,08,03),
               GenderItemId = 51002,
               Email = "Meshal_Alhammad@Dummy.com",
               PhoneNumber = "+97511111222",
               LanguageKnown = new List<LanguageItemView>
               {
                   new LanguageItemView()
                   {
                       ProfileId = 15985,
                       LanguageItemId = 98001,
                       ProficiencyItemId = 99001
                   },
                   new LanguageItemView()                   {

                       ProfileId = 15985,
                       LanguageItemId = 98002,
                       ProficiencyItemId = 99002
                   }
               }

           };

           var mm = _profileService.UpdatePersonalInfoAsync(userId).Result;

           Assert.IsTrue(mm.Success);
       }


       [TestMethod]
       public void GetProfileName_ItShouldReturnPersonalInfoName_Success()
       {
           var userId = 4;

           var mm = _profileService.GetProfileNameAsync(userId).Result;

           Assert.IsNotNull(mm);
       }

       [TestMethod]
       public void UpdateProfileName_ItShouldUpdateProfileName_Success()
       {
           var userId = new ProfileNameView()
           {
              FirstNameEN = "Meshal",
              SecondNameEN = "Alhammadi",
              ThirdNameEN = "Alhammadi",
              LastNameEN = "Alhammadi",
              FirstNameAR = "مشعل",
              SecondNameAR = "الحمادي",
              ThirdNameAR = "الحمادي",
              LastNameAR = "الحمادي",
              UserId = 4
           };

           var mm = _profileService.UpdateProfileNameAsync(userId).Result;

           Assert.IsTrue(mm.Success);
       }


       [TestMethod]
       public void GetContactDetail_ItShouldReturnContactDetails_Success()
       {
           var userId = 4;

           var mm = _profileService.GetContactDetailsAsync(userId).Result;

           Assert.IsNotNull(mm);
       }

       [TestMethod]
       public void GetLanguages_ItShouldReturnAllLanguages_Success()
       {

           var id = "5BYlw097lln1LPt8dllNzzK28YjSLlkLj18_5aTHaKQ";

           var dd = _encryption.Decrypt(id);


           var mm = _profileService.GetLanguagesAndProficiencyAsync().Result;

           Assert.IsNotNull(mm);
       }

       [TestMethod]
       public void GetMyProfile_ItShouldReturnProfileInformation_Success()
       {

           var yy = _profileService.GetAllUploadedDocumentsAsync(15859).Result;

           var mm = _profileService.GetMyProfileAsync(15921).Result;

           Assert.IsNotNull(mm);
       }

    }
}
