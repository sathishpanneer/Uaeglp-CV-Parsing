using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Uaeglp.Contracts;
using Uaeglp.Services;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Tests
{
    [TestClass]
    public class TestAccountService
    {
        private readonly IAccountService _accountService;
        private readonly RandomStringGeneratorService _randomStringGeneratorService;
        private readonly IEmailService _emailService;
        private readonly IProfilePercentageCalculationService _calculationService;
        private readonly IApplicationProgressStatusService _applicationProgressStatusService;
        public TestAccountService()
        {
            var services = new ContainerResolver().Container;
            _accountService = (IAccountService)services.GetService(typeof(IAccountService));
            _randomStringGeneratorService = (RandomStringGeneratorService)services.GetService(typeof(RandomStringGeneratorService));
            _emailService = (IEmailService)services.GetService(typeof(IEmailService));
            _calculationService = (IProfilePercentageCalculationService)services.GetService(typeof(IProfilePercentageCalculationService));
            _applicationProgressStatusService = (IApplicationProgressStatusService)services.GetService(typeof(IApplicationProgressStatusService));
        }

        [TestMethod]
        public void TestSetNewPassword_ItShouldSetNewPassoword_Success()
        {


            var sss = -2;

            var ttt = (int)(uint)sss;


            var token = _randomStringGeneratorService.Generate(25);


            var yy = new SetNewPassword()
            {

            };
            var mm = _accountService.SetNewPassword(yy);

            Assert.IsNotNull(mm);
        }


        [TestMethod]
        public void ActivationMailCheck_ItShouldSetNewPassoword_Success()
        {

            var result = _emailService.SendActivationEmailAsync("karunakaran.ramu@sword-in.com", "karuna", LanguageType.EN).Result;

            // var vv = _emailService.SendActivationEmailAsync("karunakaran.ramu@sword-in.com", "karuna", LanguageType.EN).Result;
            var tt = _emailService.SendResetPasswordEmailAsync("karunakaran.ramu@sword-in.com", "karuna", LanguageType.AR).Result;
            //  var ss = _emailService.SendResetPasswordEmailAsync("karunakaran.ramu@sword-in.com", "karuna", LanguageType.EN).Result;

            Assert.AreNotEqual(result.OTP, 0);
        }

        [TestMethod]
        public void UserDiviceInfoAdd_ItShouldAddTheEntryIntoTable_Sucess()
        {

            var data = new UserDeviceInfoViewModel()
            {
                DeviceType = DeviceType.Android,
                DeviceId = "ff3blBjyT42G0gB8BuHlyZ:APA91bFQ4qZPWXiLPPWPKuL2dqzGEi-dIljSjOu5gOjLvLY0FA59g1IKsk3qq4pI5xHJDJrPyOEaXWGvtLmf1BvUtrhpJGxnEiIfc01IMB_RtsR8LALkbJoghhu96xVd2dhNlhfuCwXN",
                IsActive = true,
            
                UserId = 13600,
                CreatedOn = DateTime.Now
            };


            var mm = _accountService.AddOrUpdateDeviceInfoAsync(data).Result;

            Assert.IsNotNull(mm);
        }

        [TestMethod]
        public void PercentageCalucuation_IshouldCalculatePercentageOfUserAndUpdate_Success()
        {
            var yy = _calculationService.UpdateProfileCompletedPercentageAsync(13600).Result;
            Assert.IsNotNull(yy);
        }

        [TestMethod]
        public void PushNotificationTest_PushNotificationToCorrespondingDivice_success()
        {

            string response;

            try
            {
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4

                string serverKey = "AIzaSyDoBdqqw5MY0LB8tiwkXR2Vs-k52BVosKI"; // Something very long
                string senderId = "83200709865";
                string deviceId = "ff3blBjyT42G0gB8BuHlyZ:APA91bFQ4qZPWXiLPPWPKuL2dqzGEi-dIljSjOu5gOjLvLY0FA59g1IKsk3qq4pI5xHJDJrPyOEaXWGvtLmf1BvUtrhpJGxnEiIfc01IMB_RtsR8LALkbJoghhu96xVd2dhNlhfuCwXN"; // Also something very long, 
                                                                // got from android
                                                                //string deviceId = "//topics/all";             // Use this to notify all devices, 
                                                                // but App must be subscribed to 
                                                                // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        body = "Greetings",
                        title = "Augsburg",
                        sound = "Enabled"
                    },
                    //notification = new
                    //{
                    //    body = "Greetings",
                    //    title = "Augsburg",
                    //    sound = "Enabled"
                    //}
                };

                //var serializer = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(data);
                var byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add($"Authorization: key={serverKey}");
                tRequest.Headers.Add($"Sender: id={senderId}");
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            Assert.IsNotNull(response);

        }




        [TestMethod]
        public void ApplicationProgressStatus_validCalculation_Success()
        {
            var yy = _applicationProgressStatusService.CreateApplicationProgressAsync(4255,13617).Result;
            Assert.IsTrue(yy);
        }
    }
}
