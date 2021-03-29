using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NLog.LayoutRenderers.Wrappers;
using Uaeglp.Contracts;
using Uaeglp.Services;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Tests
{
    [TestClass]
    public class TestPushNotification
    {
        private readonly IAccountService _accountService;
        private readonly RandomStringGeneratorService _randomStringGeneratorService;
        private readonly IEmailService _emailService;
        private readonly IProfilePercentageCalculationService _calculationService;
        private readonly IPushNotificationService _pushNotificationService;
        public TestPushNotification()
        {
            var services = new ContainerResolver().Container;
            _accountService = (IAccountService)services.GetService(typeof(IAccountService));
            _randomStringGeneratorService = (RandomStringGeneratorService)services.GetService(typeof(RandomStringGeneratorService));
            _emailService = (IEmailService)services.GetService(typeof(IEmailService));
            _calculationService = (IProfilePercentageCalculationService)services.GetService(typeof(IProfilePercentageCalculationService));
            _pushNotificationService = (IPushNotificationService)services.GetService(typeof(IPushNotificationService));
        }

        [TestMethod]
        public void TestPushNotification_Success()
        {

   //         var str = @"	""Id"" : ""5e772b6e01af179ba4ce65f2"",
			//""ParentID"" : ""5e772b6e01af179ba4ce65f1"",
			//""ParentTypeID"" : 1,
			//""ActionID"" : 6,
			//""SenderID"" : 13617,
			//""Created"" : ""2020-03-22T14:40:06.534+05:30"",
			//""Modified"" : ""2020-03-22T14:40:06.534+05:30"",
			//""IsRead"" : false,
			//""GeneralNotification"" : false";

   //         var mmm = JsonConvert.DeserializeObject<NotificationView>(str);

            var notificatoinObj = new NotificationView()
            {
                Id = "5e7c42bc0bfd2e2dd070cfa5",
                ActionID =  ActionType.Comment,
                ParentTypeID = ParentType.Post,
                SenderID = 13600,
                ParentID = "5e7c42bc0bfd2e2dd070cfa5",
                IsRead = false

            };

            var deviceid =
                "ee4wtft4cEMWiIZ5ifeQ2h:APA91bEJMYiI9a_Y3IHRsI-iB4PzDMvQtcZoKTXV4cWJABgvh7T1sJgdHDW_x7J3dniAZASSBpRoDuuAg1YCjg9JdJ-JsO6T3-UiHGUm1E5EjC35sfNyXI_PIs1HDEJsrSAUawKQCM2W";

           var yy = _pushNotificationService.SendPushNotificationAsync(notificatoinObj,deviceid).Result;

            Assert.IsNotNull(yy);
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
                string deviceId = "dkjKmI7pT-qlOlsIvoBloN:APA91bF0Xkg-LhXqliBPgJnBqNN8OW50w1iGxuB8PdyS0bU_XlG-XbTjCuG0GaUzIN2B0-dIQAxd5P_q1AYhaZviqDk4AYVIGxRCwe3XDz6WekytQwSOGJ0hPTR1_Ma1MRmNkel6IcOI"; // Also something very long, 
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
                    data = new NotificationView()
                    {
                        Id = "5e7871b1436d36b2a0a8bd15",
                        ActionID = ActionType.Comment,
                        ParentTypeID = ParentType.Post,
                        SenderID = 13613,
                        ParentID = "5e787196436d36b2a0a8bd0f",
                        IsRead = false

                    },
                    notification = new
                    {
                        body = "Greetings",
                        title = "Augsburg",
                        sound = "Enabled"
                    }
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

       
    }
}
