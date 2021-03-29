using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using Uaeglp.Contracts;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.PushNotificationViewModel;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        //private readonly LangAppDbContext _appContext;
        private readonly AppSettings _appSettings;
        private readonly DBConnectionString _dbSettings;
        public PushNotificationService(AppDbContext appDbContext, IOptions<AppSettings> appSettings, IOptions<DBConnectionString> dbSettings)
        {
            _appDbContext = appDbContext;
            _appSettings = appSettings.Value;
            _dbSettings = dbSettings.Value;
            // _appContext = appContext;
        }

        public async Task<bool> SendPushNotificationAsync(NotificationView notification, string deviceId)
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

                //string serverKey = "AIzaSyDoBdqqw5MY0LB8tiwkXR2Vs-k52BVosKI"; // Something very long
                //string senderId = "83200709865";
                //string deviceId =
                //    "ff3blBjyT42G0gB8BuHlyZ:APA91bFQ4qZPWXiLPPWPKuL2dqzGEi-dIljSjOu5gOjLvLY0FA59g1IKsk3qq4pI5xHJDJrPyOEaXWGvtLmf1BvUtrhpJGxnEiIfc01IMB_RtsR8LALkbJoghhu96xVd2dhNlhfuCwXN"; // Also something very long, 
                // got from android
                //string deviceId = "//topics/all";             // Use this to notify all devices, 
                // but App must be subscribed to 
                // topic notification


                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var notificationLabel =  await GetNotificationLabel(notification);
                var notificationText = notification.UserNameEn?.Trim() + " " + notificationLabel?.Trim();

                var data = new PushNotificationModel()
                {
                    to =  deviceId,
                    data = notification,
                    notification = new FcmNotification()
                    {
                        body = notificationText,
                        title = "New Notification",
                        sound = "Enabled"
                    }
                };
                _logger.Info($"Notification {data.ToJsonString()}");


                //var serializer = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(data);
                var byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add($"Authorization: key={_appSettings.ServerKey}");
                tRequest.Headers.Add($"Sender: id={_appSettings.SenderId}");
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = await tRequest.GetRequestStreamAsync())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = await tRequest.GetResponseAsync())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                string sResponseFromServer = await tReader.ReadToEndAsync();
                                response = sResponseFromServer;

                                var responseData = JsonConvert.DeserializeObject<PushNotifyResponse>(response);

                                if (responseData.results[0].error == "NotRegistered" && responseData.success == 0)
                                {
                                    var dbConnectionString = _dbSettings.sqlConnection;
                                    using (var context = new AppDbContext(dbConnectionString))
                                    {
                                        var userDevice = await context.UserDeviceInfos.FirstOrDefaultAsync(k => k.DeviceId == deviceId);
                                        if (userDevice != null)
                                        {
                                            context.UserDeviceInfos.Remove(userDevice);
                                            await context.SaveChangesAsync();
                                        }

                                    }
                                       
                                }

                                _logger.Info(response);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;

                _logger.Error(ex);
                return false;
            }

            return true;

        }

        public async Task<string> GetNotificationLabel(NotificationView notification)
        {
            var dbConnectionString = _dbSettings.SqlLangConnection;

            using (var context = new LangAppDbContext(dbConnectionString))
            {
                var data =  await context.Languages_Label.Where(x => x.Labelname.Contains(notification.ActionID.ToString()) && x.Pagename == "notification" && x.Language_Code == "en").Select(x => x.Value).FirstOrDefaultAsync();
                return data;
            }


                
            
        }

        public async Task<bool> SendReminderPushNotificationAsync(string content, string deviceId, int daysLeft, DateTime? registrationEndDate, int applicationId)
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

                //string serverKey = "AIzaSyDoBdqqw5MY0LB8tiwkXR2Vs-k52BVosKI"; // Something very long
                //string senderId = "83200709865";
                //string deviceId =
                //    "ff3blBjyT42G0gB8BuHlyZ:APA91bFQ4qZPWXiLPPWPKuL2dqzGEi-dIljSjOu5gOjLvLY0FA59g1IKsk3qq4pI5xHJDJrPyOEaXWGvtLmf1BvUtrhpJGxnEiIfc01IMB_RtsR8LALkbJoghhu96xVd2dhNlhfuCwXN"; // Also something very long, 
                // got from android
                //string deviceId = "//topics/all";             // Use this to notify all devices, 
                // but App must be subscribed to 
                // topic notification


                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var data = new PushNotificationModel()
                {
                    to = deviceId,
                    notification = new FcmNotification()
                    {
                        body = applicationId != 3 ? "You have shown your interest in" + " " + content + ". Hurry UP! Only" + " " + daysLeft + " " + "days left for its registration to close" : "You have" + " " + content + " " + "at" + " " + registrationEndDate,
                        title = "New Notification",
                        sound = "Enabled"
                    }
                };
                _logger.Info($"Notification {data.ToJsonString()}");


                //var serializer = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(data);
                var byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add($"Authorization: key={_appSettings.ServerKey}");
                tRequest.Headers.Add($"Sender: id={_appSettings.SenderId}");
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = await tRequest.GetRequestStreamAsync())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = await tRequest.GetResponseAsync())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                string sResponseFromServer = await tReader.ReadToEndAsync();
                                response = sResponseFromServer;

                                var responseData = JsonConvert.DeserializeObject<PushNotifyResponse>(response);

                                if (responseData.results[0].error == "NotRegistered" && responseData.success == 0)
                                {
                                    var userDevice = await _appDbContext.UserDeviceInfos.FirstOrDefaultAsync(k => k.DeviceId == deviceId);
                                    if (userDevice != null)
                                    {
                                        _appDbContext.UserDeviceInfos.Remove(userDevice);
                                        await _appDbContext.SaveChangesAsync();
                                    }
                                }

                                _logger.Info(response);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;

                _logger.Error(ex);
                return false;
            }

            return true;

        }


        public async Task<bool> SendRecommendPushNotificationAsync(NotificationView notification, string content, string deviceId)
        {
            string response;

            try
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var data = new PushNotificationModel()
                {
                    to = deviceId,
                    data = notification,
                    notification = new FcmNotification()
                    {
                        body = content,
                        title = "New Notification",
                        sound = "Enabled"
                    }
                };
                _logger.Info($"Notification {data.ToJsonString()}");


                //var serializer = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(data);
                var byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add($"Authorization: key={_appSettings.ServerKey}");
                tRequest.Headers.Add($"Sender: id={_appSettings.SenderId}");
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = await tRequest.GetRequestStreamAsync())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = await tRequest.GetResponseAsync())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                string sResponseFromServer = await tReader.ReadToEndAsync();
                                response = sResponseFromServer;
                                var responseData = JsonConvert.DeserializeObject<PushNotifyResponse>(response);

                                if (responseData.results[0].error == "NotRegistered" && responseData.success == 0)
                                {
                                    var userDevice = await _appDbContext.UserDeviceInfos.FirstOrDefaultAsync(k => k.DeviceId == deviceId);
                                    if (userDevice != null)
                                    {
                                        _appDbContext.UserDeviceInfos.Remove(userDevice);
                                        await _appDbContext.SaveChangesAsync();
                                    }
                                }

                                _logger.Info(response);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;

                _logger.Error(ex);
                return false;
            }

            return true;

        }

        public async Task<bool> SendAdminPushNotificationAsync(NotificationView notification, string content, string deviceId)
        {
            string response;

            try
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var data = new PushNotificationModel()
                {
                    to = deviceId,
                    data = notification,
                    notification = new FcmNotification()
                    {
                        body = content,
                        title = "New Notification",
                        sound = "Enabled"
                    }
                };
                _logger.Info($"Notification {data.ToJsonString()}");


                //var serializer = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(data);
                var byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add($"Authorization: key={_appSettings.ServerKey}");
                tRequest.Headers.Add($"Sender: id={_appSettings.SenderId}");
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = await tRequest.GetRequestStreamAsync())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = await tRequest.GetResponseAsync())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                string sResponseFromServer = await tReader.ReadToEndAsync();
                                response = sResponseFromServer;

                                _logger.Info(response);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;

                _logger.Error(ex);
                return false;
            }

            return true;

        }



    }
}
