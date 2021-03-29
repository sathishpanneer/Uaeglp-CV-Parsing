using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Uaeglp.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using Uaeglp.Utilities;
using AutoMapper;
using Uaeglp.Services.Communication;
using Uaeglp.Contracts.Communication;

namespace Uaeglp.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IEmailService _emailService;
        private readonly AppDbContext _appDbContext;
        private readonly IPushNotificationService _pushNotificationService;
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IMapper _mapper;
        private readonly IUserIPAddress _userIPAddress;

        public ReminderService(IEmailService emailService, AppDbContext appDbContext, IPushNotificationService pushNotificationService, IMapper mapper, IUserIPAddress userIPAddress)
        {
            _emailService = emailService;
            _appDbContext = appDbContext;
            _pushNotificationService = pushNotificationService;
            _mapper = mapper;
            _userIPAddress = userIPAddress;
        }


        public async Task<IReminderResponse> SetReminder(ReminderView view)
        {

            //var id = ObjectId.GenerateNewId();
            var registrationEndDate = new DateTime?();
            var title = "";
            if (view.ApplicationId == 1)
            {
                var batch = await _appDbContext.Batches.Where(x => x.Id == view.ActivityID).FirstOrDefaultAsync();
                registrationEndDate = batch.DateRegTo;
                title = await _appDbContext.Programmes.Where(x => x.Id == batch.ProgrammeId).Select(x => x.TitleEn).FirstOrDefaultAsync();
            } else if( view.ApplicationId == 2)
            {

               var activity = await _appDbContext.Initiatives.Where(x => x.Id == view.ActivityID).FirstOrDefaultAsync();
               registrationEndDate = activity.RegistrationEndDate;
               title = await _appDbContext.Initiatives.Where(x => x.Id == view.ActivityID).Select(x => x.TitleEn).FirstOrDefaultAsync();

            }
            else if (view.ApplicationId == 3)
            {
                var events = await _appDbContext.EventDays.Where(x => x.EventId == view.ActivityID).FirstOrDefaultAsync();
                registrationEndDate = events.EndTime;
                title = await _appDbContext.Events.Where(x => x.Id == view.ActivityID).Select(x => x.TextEn).FirstOrDefaultAsync();
            }

            var currentDate = DateTime.Now;

            var days = ((registrationEndDate - currentDate) / 3).Value.Days;

            var reminderSendDate = registrationEndDate - TimeSpan.FromDays(days);
            if (days == 0 || reminderSendDate < DateTime.Now)
            {
                reminderSendDate = DateTime.Now;
            }
            try
            {

                var reminder = await _appDbContext.Reminders.Where(x => x.UserID == view.UserID && x.ApplicationId == view.ApplicationId && x.ActivityId == view.ActivityID).FirstOrDefaultAsync();

                if(reminder == null)
                {
                    reminder = new Reminder()
                    {
                        UserID = view.UserID,
                        ApplicationId = view.ApplicationId,
                        ActivityId = view.ActivityID,
                        RegistrationEndDate = registrationEndDate,
                        ReminderSendDate = reminderSendDate,
                        isReminderSent = false,
                        Name = title
                    };

                    await _appDbContext.Reminders.AddAsync(reminder);
                    await _appDbContext.SaveChangesAsync();

                }

                var data = _mapper.Map<ReminderViewModel>(reminder);
                return new ReminderResponse(data);

            }

            catch(Exception ex)
            {
                throw ex;
            }
        
         
        }


        public async Task<IReminderResponse> RemoveReminder(int userId, int applicationId, int activityId)
        {
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId : {userId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

            try
            {
                var reminderData = await _appDbContext.Reminders.Where(x => x.UserID == userId && x.ActivityId == activityId && x.ApplicationId == applicationId).FirstOrDefaultAsync();

                var data = false;
                if (reminderData != null)
                {
                    _appDbContext.Reminders.Remove(reminderData);
                    await _appDbContext.SaveChangesAsync();

                    data = true;
                }

                return new ReminderResponse(data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
            
        }

        public async Task<List<Reminder>> SendReminder()
            {

            //var reminderList = await _appDbContext.Reminders.ToListAsync();

            //foreach(var item in reminderList)
            //{
            //    if(item.isReminderSent != true)
            //    {
            //        var registrationEndDate = new DateTime?();
            //        if (item.ApplicationId == 1)
            //        {
            //            registrationEndDate = await _appDbContext.Batches.Where(x => x.ProgrammeId == item.ActivityId).OrderByDescending(x => x.DateRegTo).Select(x => x.DateRegTo).FirstOrDefaultAsync();

            //        }
            //        else if (item.ApplicationId == 2)
            //        {
            //            //var activity = await _appDbContext.InitiativeProfiles.Where(x => x.InitiativeId == item.ActivityId && x.ProfileId == item.UserID).FirstOrDefaultAsync();
            //            registrationEndDate = await _appDbContext.Initiatives.Where(x => x.Id == item.ActivityId).Select(x => x.RegistrationEndDate).FirstOrDefaultAsync();
            //        }

            //        var currentDate = DateTime.Now;

            //        var days = ((registrationEndDate - currentDate) / 3).Value.Days;


            //        var reminderSendDate = registrationEndDate - TimeSpan.FromDays(days);
            //        if (days == 0 || reminderSendDate < DateTime.Now)
            //        {
            //            reminderSendDate = DateTime.Now;
            //        }
            //        item.RegistrationEndDate = registrationEndDate;
            //        item.ReminderSendDate = reminderSendDate;

            //        await _appDbContext.SaveChangesAsync();
            //    }
                

            //}

            var data =  await _appDbContext.Reminders.ToListAsync();

            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  data : {data} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    var userId = item.UserID;
                    var registrationEndDate = item.RegistrationEndDate;
                    var reminderDate = item.ReminderSendDate;
                    //var content = item.Name;
                    

                    //var currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    //var reminderSendDate = DateTime.Parse(reminderDate.ToString("yyyy-MM-dd"));

                    var currentDate = DateTime.Now.Date;
                    var reminderSendDate = reminderDate.Value.Date;

                    var daysLeft = (registrationEndDate - reminderSendDate).Value.Days;

                    logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  RegistrationEndDate : {reminderSendDate} CurrentDate: {currentDate}");

                    if (reminderSendDate == currentDate && item.isReminderSent != true)
                    {
                        var content = "";
                        if (item.ApplicationId == 1)
                        {
                            var batch = await _appDbContext.Batches.Where(x => x.Id == item.ActivityId).FirstOrDefaultAsync();
                            content = await _appDbContext.Programmes.Where(x => x.Id == batch.ProgrammeId).Select(x => x.TitleEn).FirstOrDefaultAsync();
                        }
                        else if (item.ApplicationId == 2)
                        {
                            content = await _appDbContext.Initiatives.Where(x => x.Id == item.ActivityId).Select(x => x.TitleEn).FirstOrDefaultAsync();
                        }
                        else if (item.ApplicationId == 3)
                        {
                            content = await _appDbContext.Events.Where(x => x.Id == item.ActivityId).Select(x => x.TextEn).FirstOrDefaultAsync();
                        }


                        var email = await _appDbContext.UserInfos.Where(k => k.UserId == userId).Select(k => k.Email).FirstOrDefaultAsync();
                        var firstName = await _appDbContext.Profiles.Where(k => k.Id == userId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                        var lastName = await _appDbContext.Profiles.Where(k => k.Id == userId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                        var userName = firstName + " " + lastName;
                        var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
                        logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Sending reminder mail to : {email}");

                        if(item.ApplicationId !=3)
                        await _emailService.SendReminderEmailAsync(email, content, userName, daysLeft, registrationEndDate);

                        if(deviceIds.Count > 0) 
                        { 
                            foreach (var deviceId in deviceIds)
                                {
                                    logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Sending reminder push notification to User : {userId} and Device ID: {deviceId} ");
                                    await _pushNotificationService.SendReminderPushNotificationAsync(content, deviceId, daysLeft, registrationEndDate, item.ApplicationId);
                                }
                                logger.Info("Notification sent");
                        }
                        item.isReminderSent = true;
                        await _appDbContext.SaveChangesAsync();


                    }
                }

            }

            return data;

            

          }
    }
}
