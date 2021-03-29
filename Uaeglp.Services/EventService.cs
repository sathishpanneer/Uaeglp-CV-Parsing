using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Event;
using Microsoft.EntityFrameworkCore;
using Uaeglp.ViewModels.Enums;
using Uaeglp.Utilities;

namespace Uaeglp.Services
{
	public class EventService : IEventService
	{
		private readonly AppDbContext _appDbContext;
		private readonly MongoDbContext _mongoDbContext;
		private readonly IEncryptionManager _encryptor;
		private readonly IFileService _fileService;
		private const string BaseURL = "";

		public EventService(AppDbContext appContext, MongoDbContext mongoDbContext, IEncryptionManager encryption, IFileService fileService)
		{
			_appDbContext = appContext;
			_mongoDbContext = mongoDbContext;
			_encryptor = encryption;
			_fileService = fileService;
		}

		public async Task<IEventResponse<List<EventDayView>>>  GetEventDays(int batchID, LanguageType language = LanguageType.AR)
		{
			try
			{
				var eventDays = await _appDbContext.EventDays.Where(e => e.Event.BatchId == batchID).Select(d => new EventDayView
				{
					Date = d.Date,
					EndTime = d.EndTime,
					StartTime = d.StartTime,
					ID = d.Id,
					Event = new EventView()
					{
						ID = d.Event.Id,
						Latitude = d.Event.Latitude,
						Longitude = d.Event.Longitude,
						TextName = language == LanguageType.EN ? d.Event.TextEn : d.Event.TextAr,
						LocationName = language == LanguageType.EN ? d.Event.LocationEn : d.Event.LocationAr,
						DescriptionName = language == LanguageType.EN ? d.Event.DescriptionEn : d.Event.DescriptionAr,
						Text = new EnglishArabicView()
						{
							English = d.Event.TextEn,
							Arabic = d.Event.TextAr
						},
						Location = new EnglishArabicView()
						{
							English = d.Event.LocationEn,
							Arabic = d.Event.LocationAr
						},
						DaysCount = d.Event.EventDays.Count,
						MapAutocompleteView = new MapAutocompleteView()
						{
							Latitude = d.Event.Latitude,
							Longitude = d.Event.Longitude,
							Text = d.Event.MapText
						}
					}
				}).OrderBy(e => e.Date).ThenBy(e => e.StartTime).ToListAsync();

				return new EventResponse<List<EventDayView>>(eventDays);

			}
			catch (Exception e)
			{
				return new EventResponse<List<EventDayView>>(e);
			}
		}

		public async Task<IEventResponse<List<EventViewNew>>> GetEventsByMonth(int userId, int month, int year, LanguageType language = LanguageType.EN, bool forProfile = false, bool ispublic = false)
		{
			try
			{

				var eveents = await _appDbContext.Events.Include(x=>x.EventDays).Include(x=>x.ProfileEvents)
					      .Where(x => x.Id == x.EventDays.FirstOrDefault(a => a.StartTime.Month == month &&
					       a.StartTime.Year == year).EventId).ToListAsync();
				List<EventNew> UserEvents = new List<EventNew>();
				foreach (var item in eveents)
				{
					//if (item.BatchId == null)
					//{ }
					Application application = null;
					int batchid = (item.BatchId.HasValue) ? item.BatchId.Value : 0;
					if (batchid != 0)
					{


						application = await
						 _appDbContext.Applications.FirstOrDefaultAsync(k => k.ProfileId == userId && k.BatchId == batchid &&
						 k.StatusItemId == (int)ApplicationProgressStatus.Accepted);
					}

					if (item.BatchId == null || application != null)
					{
						foreach (var dayevent in item.EventDays)
						{
							EventNew userevent = new EventNew();
							userevent.StartDateTime = dayevent.StartTime;
							userevent.EndDateTime = dayevent.EndTime;
							userevent.Id = item.Id;
							userevent.BatchId = item.BatchId;
							if (item.BatchId != null)
							{
								userevent.IsPublic = false;
							}
							else
							{
								userevent.IsPublic = true;
							}
							userevent.TextEn = item.TextEn;
							userevent.TextAr = item.TextAr;
							userevent.DescriptionEn = item.DescriptionEn;
							userevent.DescriptionAr = item.DescriptionAr;
							userevent.LocationEn = item.LocationEn;
							userevent.LocationAr = item.LocationAr;
							userevent.Longitude = item.Longitude;
							userevent.Latitude = item.Latitude;
							userevent.MapText = item.MapText;
							userevent.Created = item.Created;
							userevent.CreatedBy = item.CreatedBy;
							userevent.Modified = item.Modified;
							userevent.ModifiedBy = item.ModifiedBy;

							//To Check MyEvent
							var usereventJoin = await _appDbContext.ProfileEvents.FirstOrDefaultAsync(e => e.EventId == item.Id && e.ProfileId == userId && e.EventStatusItemId == 69001);
							if (usereventJoin == null)
							{
								userevent.IsMyEvent = false;
							}
							else
							{
								userevent.IsMyEvent = true;
							}
							var attList = new List<attendee>();
							foreach (var pro in item.ProfileEvents.Where(x => x.EventStatusItemId == 69001).ToList())
							{

								var attendeeName = await _appDbContext.Users.Where(x => x.Id == pro.ProfileId).FirstOrDefaultAsync();

								if (attendeeName != null)
								{
									attendee at = new attendee();
									at.ProfileId = pro.ProfileId;
									at.NameEn = (attendeeName.NameEn != null) ? attendeeName.NameEn : "";
									at.NameAr = (attendeeName.NameAr != null) ? attendeeName.NameAr : "";

									var RprofileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == attendeeName.Id);
									if (RprofileDesignation != null)
									{
										at.TitleEn = RprofileDesignation.Title?.TitleEn;

										at.TitleAr = RprofileDesignation.Title?.TitleAr;
									}
									if (attendeeName.OriginalImageFileId != null)
									{
										at.ImageURL = ConstantUrlPath.ProfileImagePath + attendeeName.OriginalImageFileId;
									}
									attList.Add(at);
								}
							}
							userevent.attendees = attList;
							UserEvents.Add(userevent);
						}

					}
					

				}

				//var query =
				//	await (from e in _appDbContext.EventDays
				//		   where (e.StartTime.Month == month && e.StartTime.Year == year)
				//		   join d in _appDbContext.Events on e.EventId equals d.Id
				//		   select new
				//		   {
				//			   e.StartTime,
				//			   e.EndTime,
				//			   d.Id,
				//			   d.BatchId,
				//			   d.TextEn,
				//			   d.TextAr,
				//			   d.DescriptionEn,
				//			   d.DescriptionAr,
				//			   d.LocationEn,
				//			   d.LocationAr,
				//			   d.Longitude,
				//			   d.Latitude,
				//			   d.MapText,
				//			   d.Created,
				//			   d.CreatedBy,
				//			   d.Modified,
				//			   d.ModifiedBy
				//		   }).ToListAsync();
				//var events = new List<EventNew>();
				//foreach (var eventDay in query)
				//{
				//	EventNew en = new EventNew();
				//	en.StartDateTime = eventDay.StartTime;
				//	en.EndDateTime = eventDay.EndTime;
				//	en.Id = eventDay.Id;
				//	en.BatchId = eventDay.BatchId;
				//	if (eventDay.BatchId != null)
				//	{
				//		en.IsPublic = false;
				//	}
				//	else
				//	{
				//		en.IsPublic = true;
				//	}
				//	en.TextEn = eventDay.TextEn;
				//	en.TextAr = eventDay.TextAr;
				//	en.DescriptionEn = eventDay.DescriptionEn;
				//	en.DescriptionAr = eventDay.DescriptionAr;
				//	en.LocationEn = eventDay.LocationEn;
				//	en.LocationAr = eventDay.LocationAr;
				//	en.Longitude = eventDay.Longitude;
				//	en.Latitude = eventDay.Latitude;
				//	en.MapText = eventDay.MapText;
				//	en.Created = eventDay.Created;
				//	en.CreatedBy = eventDay.CreatedBy;
				//	en.Modified = eventDay.Modified;
				//	en.ModifiedBy = eventDay.ModifiedBy;

				//	//To Check MyEvent
				//	var userevent = await _appDbContext.ProfileEvents.FirstOrDefaultAsync(e => e.EventId == eventDay.Id && e.ProfileId == userId && e.EventStatusItemId == 69001);
				//	if (userevent == null)
				//	{
				//		en.IsMyEvent = false;
				//	}
				//	else
				//	{
				//		en.IsMyEvent = true;
				//	}
				//	// To get attendees
				//	var attendees =
				//	await (from e in _appDbContext.ProfileEvents
				//		   where ((e.EventId == eventDay.Id) && (e.EventStatusItemId == 69001))
				//		   join u in _appDbContext.Users on e.ProfileId equals u.Id
				//		   join p in _appDbContext.Profiles on u.Id equals p.Id
				//		   select new
				//		   {
				//			   p.FirstNameEn,
				//			   p.FirstNameAr,
				//			   p.LastNameEn,
				//			   p.LastNameAr,
				//			   u.TitleEn,
				//			   u.TitleAr,
				//			   u.OriginalImageFileId,
				//			   e.ProfileId
				//		   }).ToListAsync();
				//	var attList = new List<attendee>();
				//	if (attendees != null)
				//	{
				//		foreach (var item in attendees)
				//		{
				//			attendee at = new attendee();
				//			at.NameEn = item.FirstNameEn?.Trim() + " " + item.LastNameEn?.Trim();
				//			at.NameAr = item.FirstNameAr?.Trim() + " " + item.LastNameAr?.Trim();
				//			at.TitleEn = item.TitleEn;
				//			at.TitleAr = item.TitleAr;
				//			if (item.OriginalImageFileId != null)
				//			{
				//				at.ImageURL = ConstantUrlPath.ProfileImagePath + item.OriginalImageFileId;
				//			}
				//			at.ProfileId = item.ProfileId;

				//			attList.Add(at);
				//		}
				//	}
				//	en.attendees = attList;

				//	events.Add(en);
				//}
				return new EventResponse<List<EventViewNew>>(GetEventViewsNew(UserEvents, userId, language));
			}
			catch (Exception e)
			{
				return new EventResponse<List<EventViewNew>>(e);
			}
		}


		public async Task<IEventResponse<List<EventView>>> GetEvents(int userId, LanguageType language = LanguageType.AR, bool forProfile = false, bool ispublic = false)
		{
			try
			{
				var events = await _appDbContext.Events.Where(e => forProfile ? ispublic ? e.BatchId == new int?() : e.BatchId == new int?() || e.Batch.Applications.Any(app => app.ProfileId == userId && (app.StatusItemId == (int?)59006 || app.StatusItemId == (int?)59009)) : true).ToListAsync();
				return new EventResponse<List<EventView>>(GetEventViews(events, language));
			}
			catch (Exception e)
			{
				return new EventResponse<List<EventView>>(e);
			}
		}

		public async Task<IEventResponse<List<EventView>>> GetEventsByBatches(int userId, LanguageType language = LanguageType.AR)
		{
			try
			{
				var events = await _appDbContext.Events.Where(e => e.BatchId != new int?() && e.Batch.Applications.Any(app => app.ProfileId == userId && (app.StatusItemId == (int?)59006 || app.StatusItemId == (int?)59009))).ToListAsync();
				return new EventResponse<List<EventView>>(GetEventViews(events, language));
			}
			catch (Exception e)
			{
				return new EventResponse<List<EventView>>(e);
			}
		}

		public async Task<IEventResponse<List<EventView>>> GetEventsForCalender(int userId, LanguageType language = LanguageType.AR)
		{
			try
			{
				var events = await _appDbContext.Events.Where(e => e.BatchId == new int?() || e.Batch.Applications.Any(app => app.ProfileId == userId && (app.StatusItemId == (int?)59006 || app.StatusItemId == (int?)59009))).ToListAsync();
				return new EventResponse<List<EventView>>(GetEventViews(events, language));
			}
			catch (Exception e)
			{
				return new EventResponse<List<EventView>>(e);
			}
		}

		public async Task<IEventResponse<List<EventView>>> GetAttendingEvents(LanguageType language = LanguageType.AR)
		{
			try
			{
				var events = await _appDbContext.Events.Where(b => b.ProfileEvents.Any(pe => pe.EventStatusItemId == 69001)).ToListAsync();
				return new EventResponse<List<EventView>>(GetEventViews(events, language));
			}
			catch (Exception e)
			{
				return new EventResponse<List<EventView>>(e);
			}
		}

		public async Task<IEventResponse<List<EventView>>> GetNotAttendingEvents(LanguageType language = LanguageType.AR)
		{
			try
			{
				var events = await _appDbContext.Events.Where(b => b.ProfileEvents.Any(pe => pe.EventStatusItemId == 69003)).ToListAsync();
				return new EventResponse<List<EventView>>(GetEventViews(events, language));
			}
			catch (Exception e)
			{
				return new EventResponse<List<EventView>>(e);
			}
		}

		public async Task<IEventResponse<List<EventView>>> GetMaBeEvents(LanguageType language = LanguageType.AR)
		{
			try
			{
				var events = await _appDbContext.Events.Where(b => b.ProfileEvents.Any(pe => pe.EventStatusItemId == 69002)).ToListAsync();
				return new EventResponse<List<EventView>>(GetEventViews(events, language));
			}
			catch (Exception e)
			{
				return new EventResponse<List<EventView>>(e);
			}
		}

		public async Task<IEventResponse<int>> SetEventDesicion(int UserID, int decisionID, int eventID)
		{
			try
			{
				var profileEvent = await _appDbContext.ProfileEvents.FirstOrDefaultAsync(m => m.EventId == eventID && m.ProfileId == UserID);
				if (profileEvent != null)
					profileEvent.EventStatusItemId = decisionID;
				else
					_appDbContext.ProfileEvents.Add(new ProfileEvent()
					{
						ProfileId = UserID,
						EventId = eventID,
						EventStatusItemId = decisionID
					});
				await _appDbContext.SaveChangesAsync();
				return new EventResponse<int>(decisionID);
			}
			catch (Exception e)
			{
				return new EventResponse<int>(e.Message,System.Net.HttpStatusCode.InternalServerError);
			}
		}

		public async Task<IEventResponse<MeetingRequestView>> GetMeetingRequest(int eventID)
		{
			try
			{
				var ev = await _appDbContext.Events.FirstOrDefaultAsync(x => x.Id == eventID);
				if (ev == null)
					throw new ArgumentException("Invalid  Event: " + (object)eventID);
				var request = new MeetingRequestView()
				{
					StartTime = ev.EventDays.FirstOrDefault().StartTime,
					EndTime = ev.EventDays.Max().EndTime,
					EventID = eventID
				};
				return new EventResponse<MeetingRequestView>(request);
			}
			catch (Exception e)
			{
				return new EventResponse<MeetingRequestView>(e);
			}
		}

		public async Task<IEventResponse<object>> UpdateEvent(EventView model)
		{
			try
			{
				var eventObj = await _appDbContext.Events.FirstOrDefaultAsync(x => x.Id == model.ID);
				if (eventObj == null)
					throw new ArgumentException("Invalid eventID: " + (object)model.ID);

				eventObj.LocationAr = model.Location.Arabic;
				eventObj.LocationEn = model.Location.English;
				eventObj.TextAr = model.Text.Arabic;
				eventObj.TextEn = model.Text.English;
				eventObj.DescriptionEn = model.Description.English;
				eventObj.DescriptionAr = model.Description.Arabic;
				eventObj.BatchId = model.BatchID;

				if (!string.IsNullOrEmpty(model.MapAutocompleteView.Longitude) && !string.IsNullOrEmpty(model.MapAutocompleteView.Latitude))
				{
					eventObj.Latitude = model.MapAutocompleteView.Latitude;
					eventObj.Longitude = model.MapAutocompleteView.Longitude;
					eventObj.MapText = model.MapAutocompleteView.Text;
				}
				else
				{
					eventObj.Latitude = null;
					eventObj.Longitude = null;
					eventObj.MapText = null;
				}
				List<int> updatedDayIDs = new List<int>();
				List<EventDay> eventDayList = new List<EventDay>();
				if (model.EventDays != null)
				{
					foreach (EventDayView item in model.EventDays)
					{
						if (item.ID == 0)
						{
							EventDay eventDay2 = new EventDay
							{
								Date = item.Date,
								StartTime = item.Date.AddHours(item.StartTime.Hour).AddMinutes(item.StartTime.Minute),
								EndTime = item.Date.AddHours(item.EndTime.Hour).AddMinutes(item.EndTime.Minute)
							};
							eventDayList.Add(eventDay2);
						}
						else
						{
							EventDay eventDay2 = eventObj.EventDays.FirstOrDefault(d => d.Id == item.ID);
							eventDay2.Date = item.Date;
							eventDay2.StartTime = item.Date.AddHours(item.StartTime.Hour).AddMinutes(item.StartTime.Minute);
							eventDay2.EndTime = item.Date.AddHours(item.EndTime.Hour).AddMinutes(item.EndTime.Minute);
							updatedDayIDs.Add(item.ID);
						}
					}
				}
				var list = eventObj.EventDays.Where(e => !updatedDayIDs.Contains(e.Id)).ToList();

				_appDbContext.EventDays.RemoveRange(list);
				foreach (EventDay eventDay in eventDayList)
					eventObj.EventDays.Add(eventDay);
				_appDbContext.SaveChanges();
				return new EventResponse<object>(null);

			}
			catch (Exception e)
			{
				return new EventResponse<object>(e);
			}
		}

		public async Task<IEventResponse<EventView>> GetEvent(int eventID, bool cruchDays = false, LanguageType language = LanguageType.AR)
		{
			try
			{
				var ev = await _appDbContext.Events.FirstOrDefaultAsync(e => e.Id == eventID);
				var eventView = GetEventViews(new List<Event> { ev }, language).FirstOrDefault();

				if (cruchDays)
					this.CrunchDays(eventView);
				return new EventResponse<EventView>(eventView);
			}
			catch (Exception e)
			{
				return new EventResponse<EventView>(e);
			}			
		}

		public async Task<IEventResponse<int>> GetUserDecision(int userId, int eventID)
		{
			try
			{
				var profileEvents = await _appDbContext.ProfileEvents.FirstOrDefaultAsync(m => m.EventId == eventID && m.ProfileId == userId);
				if (profileEvents != null)
				{
					if (profileEvents.EventStatusItemId == 69001)
						return new EventResponse<int>(1);
					if (profileEvents.EventStatusItemId == 69002)
						return new EventResponse<int>(2);
					if (profileEvents.EventStatusItemId == 69003)
						return new EventResponse<int>(3);
				}
				return new EventResponse<int>(4);
			}
			catch (Exception e)
			{
				return new EventResponse<int>(e);
			}
		}

		public async Task<IEventResponse<int>> GetTotalCount()
		{
			try
			{
				return new EventResponse<int>(await _appDbContext.Events.CountAsync());
			}
			catch (Exception e)
			{
				return new EventResponse<int>(e);
			}
		}

		private void CancelMeetingRequests(IEnumerable<int> eventDayIDs)
		{
			var eventDays = _appDbContext.EventDays.Where(ed => eventDayIDs.Contains<int>(ed.Id) && ed.MeetingRequestExchangeId != default(string));
			foreach (EventDay eventDay in eventDays)
			{
				eventDay.MeetingRequestExchangeId = null;
			}
			_appDbContext.SaveChanges();
		}

		private void CrunchDays(EventView model)
		{
			if (model.EventDays == null || model.EventDays.Count == 0)
				return;
			var orderedEnumerable = model.EventDays.OrderBy(d => d.Date).ThenBy(d => d.StartTime);
			List<List<EventDayView>> eventDayViewListList = new List<List<EventDayView>>();
			List<EventDayView> source1 = new List<EventDayView>();
			foreach (EventDayView eventDayView1 in orderedEnumerable)
			{
				var eventDayView2 = source1.First();
				var dateTime = eventDayView2.Date;
				string str1 = dateTime.ToString("yyyy-MM");
				dateTime = eventDayView1.Date;
				string str2 = dateTime.ToString("yyyy-MM");
				if (str1 == str2)
				{
					dateTime = eventDayView2.StartTime;
					string str3 = dateTime.ToString("hh:mm tt");
					dateTime = eventDayView1.StartTime;
					string str4 = dateTime.ToString("hh:mm tt");
					if (str3 == str4)
					{
						dateTime = eventDayView2.EndTime;
						string str5 = dateTime.ToString("hh:mm tt");
						dateTime = eventDayView1.EndTime;
						string str6 = dateTime.ToString("hh:mm tt");
						if (str5 == str6)
						{
							source1.Add(eventDayView1);
							continue;
						}
					}
				}
				eventDayViewListList.Add(source1);
				source1 = new List<EventDayView>
				{
					eventDayView1
				};
			}
			if (!eventDayViewListList.Contains(source1))
				eventDayViewListList.Add(source1);
			List<EventDayView> eventDayViewList = new List<EventDayView>();
			foreach (List<EventDayView> source2 in eventDayViewListList)
			{
				EventDayView eventDayView1 = new EventDayView();
				EventDayView eventDayView2 = source2.First();
				eventDayView1.StartTime = eventDayView2.StartTime;
				eventDayView1.EndTime = eventDayView2.EndTime;
				eventDayView1.DateText = string.Join(", ", source2.Select(d => d.Date.Day)) + " " + eventDayView2.Date.ToString("MMMM yyyy");
				model.DaysCount += source2.Count;
				eventDayViewList.Add(eventDayView1);
			}
			model.EventDays = eventDayViewList;
		}

		private List<EventView> GetEventViews(List<Event> events, LanguageType language = LanguageType.AR)
		{
			var eventViews = new List<EventView>();
			foreach (var e in events)
			{
				if (e != null)
				{
					var eventView = new EventView
					{
						ID = e.Id,
						BatchID = e.BatchId,
						Latitude = e.Latitude,
						Longitude = e.Longitude,
						Location = new EnglishArabicView
						{

							Arabic = e.LocationAr,
							English = e.LocationEn,
						},
						TextName = language == LanguageType.AR ? e.TextAr : e.TextEn,
						DescriptionName = language == LanguageType.AR ? e.DescriptionAr : e.DescriptionEn,
						LocationName = language == LanguageType.AR ? e.LocationAr : e.LocationEn,
						Text = new EnglishArabicView
						{

							Arabic = e.TextAr,
							English = e.TextEn,
						},
						Description = new EnglishArabicView
						{

							Arabic = e.DescriptionAr,
							English = e.DescriptionEn,
						},
						MapText = e.MapText,
						DaysCount = e.EventDays.Count,
						MapAutocompleteView = new MapAutocompleteView()
						{
							Latitude = e.Latitude,
							Longitude = e.Longitude,
							Text = e.MapText
						},
						EventDays = e.EventDays.Select(d => new EventDayView()
						{
							ID = d.Id,
							EventID = d.EventId,
							Date = d.Date,
							StartTime = d.StartTime,
							EndTime = d.EndTime,
							EventTitle = language == LanguageType.AR ? e.TextEn : e.TextAr,
							IsMeetingRequestCreated = !string.IsNullOrEmpty(d.MeetingRequestExchangeId)
						}).ToList()
					};

					eventViews.Add(eventView);
				}
			}
			return eventViews;
		}

		private List<EventViewNew> GetEventViewsNew(List<EventNew> events, int userId, LanguageType language = LanguageType.AR)
		{
			var eventViews = new List<EventViewNew>();
			foreach (var e in events)
			{
				if (e != null)
				{
					var eventView = new EventViewNew
					{
						IsMyEvent = e.IsMyEvent,
						IsPublic = e.IsPublic,
						StartDateTime = e.StartDateTime,
						EndDateTime = e.EndDateTime,
						ID = e.Id,
						BatchID = e.BatchId,
						Location = new EnglishArabicViewEvents
						{

							Arabic = e.LocationAr,
							English = e.LocationEn,
						},
						Text = new EnglishArabicViewEvents
						{

							Arabic = e.TextAr,
							English = e.TextEn,
						},
						Description = new EnglishArabicViewEvents
						{

							Arabic = e.DescriptionAr,
							English = e.DescriptionEn,
						},
						MapAutocompleteView = new MapAutocompleteView()
						{
							Latitude = e.Latitude,
							Longitude = e.Longitude,
							Text = e.MapText
						},
						attendees = e.attendees.Select(d => new attendee()
						{
							NameEn = d.NameEn,
							NameAr = d.NameAr,
							TitleEn = d.TitleEn,
							TitleAr = d.TitleAr,
							ImageURL = d.ImageURL,
							ProfileId = d.ProfileId
						}).ToList()
					};

					eventViews.Add(eventView);
				}
			}

			foreach(var item in eventViews)
			{
				var reminder =  _appDbContext.Reminders.Where(x => x.UserID == userId && x.ActivityId == item.ID && x.ApplicationId == 3).FirstOrDefault();
				item.isReminderSet = reminder != null ? true : false;
			}
			return eventViews;
		}
		//		public async Task Create(EventView model)
		//		{
		//			var eventObj = new Event();
		//			if (!string.IsNullOrEmpty(model.MapAutocompleteView.Longitude) && !string.IsNullOrEmpty(model.MapAutocompleteView.Latitude))
		//			{
		//				eventObj.Latitude = model.MapAutocompleteView.Latitude;
		//				eventObj.MapText = model.MapAutocompleteView.Text;
		//				eventObj.Longitude = model.MapAutocompleteView.Longitude;
		//			}
		//			eventObj.LocationAr = model.Location.Arabic;
		//			eventObj.LocationEn = model.Location.English;
		//			eventObj.TextAr = model.Text.Arabic;
		//			eventObj.TextEn = model.Text.English;
		//			eventObj.BatchId = model.BatchID;
		//			eventObj.DescriptionEn = model.Description.English;
		//			eventObj.DescriptionAr = model.Description.Arabic;
		//			if (model.EventDays != null)
		//				eventObj.EventDays = (ICollection<EventDay>)model.EventDays.Select(e =>
		//				{
		//					EventDay eventDay = new EventDay
		//					{
		//						Date = e.Date
		//					};
		//					DateTime dateTime1 = e.Date;
		//					dateTime1 = dateTime1.AddHours((double)e.StartTime.Hour);
		//					eventDay.StartTime = dateTime1.AddMinutes((double)e.StartTime.Minute);
		//					DateTime dateTime2 = e.Date;
		//					dateTime2 = dateTime2.AddHours((double)e.EndTime.Hour);
		//					eventDay.EndTime = dateTime2.AddMinutes((double)e.EndTime.Minute);
		//					return eventDay;
		//				}).ToList();
		//			_appDbContext.Events.Add(eventObj);
		//			await _appDbContext.SaveChangesAsync();
		//			NotificationView notificationView = new NotificationView()
		//			{
		//				ParentID = eventObj.Id.ToString(),
		//				ParentTypeID = ViewModels.Enums.ParentType.Event,
		//				GeneralNotification = true,
		//				ActionID = ViewModels.Enums.ActionType.AddNewItem,
		//				Modified = DateTime.Now
		//			};
		//			notificationView.GeneralNotification = true;
		//			int? batchId = eventObj.BatchId;
		//			if (batchId.HasValue)
		//			{
		//				batchId = eventObj.BatchId;
		//				int num = 0;
		//				if (!(batchId.GetValueOrDefault() == num & batchId.HasValue))
		//				{
		//					Batch batch = _appDbContext.Batches.FirstOrDefaultAsync(b => (int?)b.Id == eventObj.BatchId);
		//					CreateForMultiProfilesAsync(notificationView, ParentType.Event, batch != null ? batch.Applications.Select(p => p.ProfileId).ToList() : null);
		//					return;
		//				}
		//			}
		//			CreateForMultiProfilesAsync(notificationView, ParentType.Event, null);
		//		}

		//		public void CreateOrUpdateMeetingRequests(IEnumerable<int> eventDayIDs)
		//		{
		//			var eventDays = _appDbContext.EventDays.Where(ed => eventDayIDs.Contains(ed.Id));
		//			foreach (var eventDay in eventDays.GroupBy(ed => ed.EventId))
		//			{
		//				Event eventObj = eventDay.First().Event;
		//				List<int> batchUsers = this.GetBatchUsers(eventObj);
		//				StringBuilder sb = new StringBuilder();
		//				this.AddSection(ref sb, "Description", eventObj.DescriptionEN, false);
		//				this.AddSection(ref sb, "Location", eventObj.LocationEN, false);
		//				this.AddSection(ref sb, "الوصف", eventObj.DescriptionAR, true);
		//				this.AddSection(ref sb, "المكان", eventObj.LocationAR, true);
		//				if (!string.IsNullOrEmpty(eventObj.Longitude) && !string.IsNullOrEmpty(eventObj.Latitude))
		//				{
		//					MapAutocompleteView mapAutocompleteView = new MapAutocompleteView()
		//					{
		//						Longitude = eventObj.Longitude,
		//						Latitude = eventObj.Latitude
		//					};
		//					sb.AppendLine("<br /><a href=\"" + mapAutocompleteView.GoogleMapsURL + "\">Google Maps</a><br />");
		//				}
		//				string str1 = sb.ToString();
		//				foreach (var ev in eventDay)
		//				{
		//					MeetingRequestView meetingRequestModel1 = new MeetingRequestView();
		//					meetingRequestModel1.Start = ev.StartTime;
		//					meetingRequestModel1.End = ev.EndTime;
		//					meetingRequestModel1.Subject = string.Join(" | ", eventObj.TextEn, ev.Date.ToString("dd-MM-yyyy"));
		//					meetingRequestModel1.Body = str1;
		//					MeetingRequestView meetingRequestModel2 = meetingRequestModel1;
		//					string str2;
		//					if (eventObj.LocationEn == null || eventObj.LocationAr == null)
		//						str2 = "";
		//					else
		//						str2 = string.Join(" | ", eventObj.LocationEn, eventObj.LocationAr);
		//					meetingRequestModel2.Location = str2;
		//					MeetingRequestView model = meetingRequestModel1;
		//					if (string.IsNullOrEmpty(ev.MeetingRequestExchangeId))
		//					{
		//						ev.MeetingRequestExchangeId = CreateMeetingRequest(model, "EWSProvider", batchUsers, null, new int?());
		//					}
		//					else
		//					{
		//						model.ExchangeId = ev.MeetingRequestExchangeId;
		//						UpdateMeetingRequest(model, "EWSProvider", batchUsers, null);
		//					}
		//				}
		//			}
		//			_appDbContext.SaveChanges();
		//		}

		//		private string CreateMeetingRequest(MeetingRequestView model, string emailProviderName, IEnumerable<int> userIDs = null,
		//			IEnumerable<string> directEmails = null, int? templateID = null)
		//		{
		//			if (userIDs == null && directEmails == null && (model.To == null || model.To.Count == 0))
		//				return string.Empty;
		//			Dictionary<string, string> keyMap = new Dictionary<string, string>();
		//			if (model.MessageID == Guid.Empty)
		//				model.MessageID = Guid.NewGuid();
		//			var recipients = GetRecipients(templateID.GetValueOrDefault(), userIDs, keyMap);
		//			this.GetRecipients(templateID.GetValueOrDefault(), recipients, directEmails, keyMap);
		//			this.GetRecipients(templateID.GetValueOrDefault(), recipients, model.To, keyMap);
		//			var providerEmail = this.GetProvider_Email(emailProviderName);
		//			model.To.Clear();
		//			foreach (Recipient recipient in recipients.Recipients)
		//				model.To.Add(recipient.Email);
		//			return providerEmail.CreateMeetingRequest(model);
		//		}

		//		private string GetUnsubscribeUrl(int templateID)
		//		{
		//			return BaseURL + "/MessageCenter/Unsubscribe/" + _encryptor.Encrypt("templateid=" + (object)templateID);
		//		}

		//		private RecipientsCollection GetRecipients(int templateID, IEnumerable<int> userIDs, Dictionary<string, string> keyMap)
		//		{
		//			RecipientsCollection recipientsCollection = new RecipientsCollection();
		//			if (userIDs == null || userIDs.Count<int>() == 0)
		//				return recipientsCollection;

		//			var templateUnsubscribes = _appDbContext.TemplateUnsubscribes.Where(x => userIDs.Any(y => y == x.UserId) && x.TemplateId == templateID);
		//			var recipient = _appDbContext.UserInfos.Where(x => userIDs.Any(y => y == x.UserId)).Select(r => new Recipient
		//			{
		//				NameEN = r.User.NameEn,
		//				NameAR = r.User.NameAr,
		//				Email = r.Email,
		//				Mobile = r.Mobile
		//			});

		//			string unsubscribeUrl = this.GetUnsubscribeUrl(templateID);

		//			foreach (var r in recipient)
		//			{
		//				r.KeyMap = new Dictionary<string, string>(keyMap);
		//				DateTime now = DateTime.Now;
		//				r.AddKeyToMap("NameEN", r.NameEN);
		//				r.AddKeyToMap("NameAR", r.NameAR);
		//				r.AddKeyToMap("Mobile", r.Mobile);
		//				r.AddKeyToMap("Email", r.Email);
		//				r.AddKeyToMap("Date", now.ToString("dd-MM-yyyy"));
		//				r.AddKeyToMap("Time", now.ToString("hh:mm tt"));
		//				r.AddKeyToMap("DateTime", now.ToString("dd-MM-yyyy hh:mm tt"));
		//				r.AddKeyToMap("UnsubscribeUrl", unsubscribeUrl);
		//				r.AddKeyToMap("BaseURL", BaseURL);
		//				recipientsCollection.Recipients.Add(r);
		//				if (r.LangKey == "en")
		//					recipientsCollection.IsEnglish = true;
		//				if (r.LangKey == "ar")
		//					recipientsCollection.IsArabic = true;
		//			}

		//			return recipientsCollection;
		//		}


		//private RecipientsCollection GetRecipients(int templateID, RecipientsCollection collection,
		//	IEnumerable<string> directEmails,
		//	Dictionary<string, string> keyMap, LanguageType language = LanguageType.AR)
		//{
		//	if (collection == null)
		//		collection = new RecipientsCollection();
		//	if (directEmails == null || directEmails.Count<string>() == 0)
		//		return collection;
		//	string unsubscribeUrl = this.GetUnsubscribeUrl(templateID);
		//	foreach (string directEmail in directEmails)
		//	{
		//		Recipient recipient = new Recipient();
		//		recipient.KeyMap = new Dictionary<string, string>((IDictionary<string, string>)keyMap);
		//		DateTime now = DateTime.Now;
		//		recipient.AddKeyToMap("Email", directEmail);
		//		recipient.AddKeyToMap("Date", now.ToString("dd-MM-yyyy"));
		//		recipient.AddKeyToMap("Time", now.ToString("hh:mm tt"));
		//		recipient.AddKeyToMap("DateTime", now.ToString("dd-MM-yyyy hh:mm tt"));
		//		recipient.AddKeyToMap("UnsubscribeUrl", unsubscribeUrl);
		//		recipient.AddKeyToMap("BaseURL", BaseURL);
		//		if (!collection.Recipients.Any())
		//			recipient.LangKey = language;
		//		else
		//			collection.IsEnglish = true;
		//		collection.Recipients.Add(recipient);
		//	}
		//	return collection;
		//}


		//private void AddSection(ref StringBuilder sb, string label, string content, bool isArabic = false)
		//{
		//	if (string.IsNullOrEmpty(content))
		//		return;
		//	string str = isArabic ? "style=\"direction:rtl;text-align:right;\"" : "";
		//	sb.AppendLine("<h3 " + str + ">" + label + ":</h3>");
		//	sb.AppendLine("<div " + str + ">" + content.Replace("\r\n", "<br />") + "</div>");
		//	sb.AppendLine("<br />");
		//}

		//private List<int> GetBatchUsers(Event eventObj)
		//{
		//	return eventObj.Batch.Applications.Where(a =>
		//	{
		//		int? statusItemId1 = a.StatusItemId;
		//		int num1 = 59006;
		//		if (statusItemId1.GetValueOrDefault() == num1 & statusItemId1.HasValue)
		//			return true;
		//		int? statusItemId2 = a.StatusItemId;
		//		int num2 = 59009;
		//		return statusItemId2.GetValueOrDefault() == num2 & statusItemId2.HasValue;
		//	}).Select(a => a.ProfileId).ToList();
		//}

		//public void CancelMeetingRequests(IEnumerable<int> eventDayIDs, string cancellationMessage = null)
		//{
		//	IDbSet<EventDay> eventDayDal = _appDbContext.EventDays;
		//	Expression<Func<EventDay, bool>> predicate = (Expression<Func<EventDay, bool>>)(ed => eventDayIDs.Contains<int>(ed.ID) && ed.MeetingRequestExchangeId != default(string));
		//	foreach (EventDay eventDay in (IEnumerable<EventDay>)eventDayDal.Where<EventDay>(predicate))
		//	{
		//		this._notificationsLogic.DeleteMeetingRequest(eventDay.MeetingRequestExchangeId, "EWSProvider", cancellationMessage);
		//		eventDay.MeetingRequestExchangeId = null;
		//	}
		//	_appDbContext.SaveChanges();
		//}

		//public void Update(EventView model)
		//{
		//	Event eventObj = _appDbContext.Events.Find((object)model.ID);
		//	if (eventObj == null)
		//		throw new ArgumentException("Invalid eventID: " + (object)model.ID);
		//	this._transactionFactory.ExecuteTransaction((Action)(() =>
		//	{
		//		eventObj.LocationAR = model.Location.Arabic;
		//		eventObj.LocationEN = model.Location.English;
		//		eventObj.TextAR = model.Text.Arabic;
		//		eventObj.TextEN = model.Text.English;
		//		eventObj.DescriptionEN = model.Description.English;
		//		eventObj.DescriptionAR = model.Description.Arabic;
		//		eventObj.BatchID = model.BatchID;
		//		if (!string.IsNullOrEmpty(model.MapAutocompleteView.Longitude) && !string.IsNullOrEmpty(model.MapAutocompleteView.Latitude))
		//		{
		//			eventObj.Latitude = model.MapAutocompleteView.Latitude;
		//			eventObj.Longitude = model.MapAutocompleteView.Longitude;
		//			eventObj.MapText = model.MapAutocompleteView.Text;
		//		}
		//		else
		//		{
		//			eventObj.Latitude = null;
		//			eventObj.Longitude = null;
		//			eventObj.MapText = null;
		//		}
		//		List<int> updatedDayIDs = new List<int>();
		//		List<EventDay> eventDayList = new List<EventDay>();
		//		if (model.EventDays != null)
		//		{
		//			foreach (EventDayView eventDay1 in model.EventDays)
		//			{
		//				EventDayView item = eventDay1;
		//				DateTime dateTime1;
		//				DateTime dateTime2;
		//				if (item.ID == 0)
		//				{
		//					EventDay eventDay2 = new EventDay();
		//					eventDay2.Date = item.Date;
		//					EventDay eventDay3 = eventDay2;
		//					dateTime1 = item.Date;
		//					ref DateTime local1 = ref dateTime1;
		//					dateTime2 = item.StartTime;
		//					double hour1 = (double)dateTime2.Hour;
		//					dateTime1 = local1.AddHours(hour1);
		//					ref DateTime local2 = ref dateTime1;
		//					dateTime2 = item.StartTime;
		//					double minute1 = (double)dateTime2.Minute;
		//					DateTime dateTime3 = local2.AddMinutes(minute1);
		//					eventDay3.StartTime = dateTime3;
		//					EventDay eventDay4 = eventDay2;
		//					dateTime1 = item.Date;
		//					ref DateTime local3 = ref dateTime1;
		//					dateTime2 = item.EndTime;
		//					double hour2 = (double)dateTime2.Hour;
		//					dateTime1 = local3.AddHours(hour2);
		//					ref DateTime local4 = ref dateTime1;
		//					dateTime2 = item.EndTime;
		//					double minute2 = (double)dateTime2.Minute;
		//					DateTime dateTime4 = local4.AddMinutes(minute2);
		//					eventDay4.EndTime = dateTime4;
		//					eventDayList.Add(eventDay2);
		//				}
		//				else
		//				{
		//					EventDay eventDay2 = eventObj.EventDays.FirstOrDefault<EventDay>((Func<EventDay, bool>)(d => d.ID == item.ID));
		//					eventDay2.Date = item.Date;
		//					dateTime1 = item.Date;
		//					ref DateTime local1 = ref dateTime1;
		//					dateTime2 = item.StartTime;
		//					double hour1 = (double)dateTime2.Hour;
		//					dateTime1 = local1.AddHours(hour1);
		//					ref DateTime local2 = ref dateTime1;
		//					dateTime2 = item.StartTime;
		//					double minute1 = (double)dateTime2.Minute;
		//					eventDay2.StartTime = local2.AddMinutes(minute1);
		//					dateTime1 = item.Date;
		//					ref DateTime local3 = ref dateTime1;
		//					dateTime2 = item.EndTime;
		//					double hour2 = (double)dateTime2.Hour;
		//					dateTime1 = local3.AddHours(hour2);
		//					ref DateTime local4 = ref dateTime1;
		//					dateTime2 = item.EndTime;
		//					double minute2 = (double)dateTime2.Minute;
		//					eventDay2.EndTime = local4.AddMinutes(minute2);
		//					updatedDayIDs.Add(item.ID);
		//				}
		//			}
		//		}
		//		List<EventDay> list = eventObj.EventDays.Where<EventDay>((Func<EventDay, bool>)(e => !updatedDayIDs.Contains(e.ID))).ToList<EventDay>();
		//		this.CancelMeetingRequests((IEnumerable<int>)eventObj.EventDays.Where<EventDay>((Func<EventDay, bool>)(e => !string.IsNullOrEmpty(e.MeetingRequestExchangeId))).Select<EventDay, int>((Func<EventDay, int>)(e => e.ID)).ToArray<int>(), null);
		//		_appDbContext.EventDays.RemoveRange<EventDay>((IEnumerable<EventDay>)list);
		//		foreach (EventDay eventDay in eventDayList)
		//			eventObj.EventDays.Add(eventDay);
		//		_appDbContext.SaveChanges();
		//	}), TransactionScopeOption.Required, IsolationLevel.ReadCommitted, 16);
		//}

		//public bool Delete(int id, string cancellationMessage = null)
		//{
		//	Event entity = _appDbContext.Events.Find((object)id);
		//	if (entity == null)
		//		throw new ArgumentException("Invalid eventID: " + (object)id);
		//	List<int> list1 = entity.EventDays.Select<EventDay, int>((Func<EventDay, int>)(d => d.ID)).ToList<int>();
		//	List<int> list2 = entity.Profile_Events.Select<Profile_Events, int>((Func<Profile_Events, int>)(d => d.ID)).ToList<int>();
		//	try
		//	{
		//		if (list1.Count > 0)
		//		{
		//			foreach (int num in list1)
		//				_appDbContext.EventDays.Remove(_appDbContext.EventDays.Find((object)num));
		//		}
		//		if (list2.Count > 0)
		//		{
		//			foreach (int num in list2)
		//				this._profile_EventsDAl.Remove(this._profile_EventsDAl.Find((object)num));
		//		}
		//		_appDbContext.Events.Remove(entity);
		//		_appDbContext.SaveChanges();
		//		this.CancelMeetingRequests((IEnumerable<int>)list1, cancellationMessage);
		//		return true;
		//	}
		//	catch (Exception ex)
		//	{
		//		this._logger.Error("Tried to delete event", ex);
		//		return false;
		//	}
		//}

		//public EventView Get(int eventID, bool cruchDays = false)
		//{
		//	var data = _appDbContext.Events.Where(e => e.ID == eventID)).Select(e => new
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		LocationEN = e.LocationEN,
		//		LocationAR = e.LocationAR,
		//		Longitude = e.Longitude,
		//		TextAR = e.TextAR,
		//		TextEN = e.TextEN,
		//		MapText = e.MapText,
		//		EventDays = e.EventDays,
		//		DescriptionEN = e.DescriptionEN,
		//		DescriptionAR = e.DescriptionAR,
		//		BatchID = e.BatchId
		//	}).FirstOrDefaultAsync();
		//	EventView model = new EventView()
		//	{
		//		ID = data.ID,
		//		Latitude = data.Latitude,
		//		Longitude = data.Longitude,
		//		BatchID = data.BatchID,
		//		MapText = data.MapText,
		//		Text = new EnglishArabicView()
		//		{
		//			English = data.TextEN,
		//			Arabic = data.TextAR
		//		},
		//		Location = new EnglishArabicView()
		//		{
		//			English = data.LocationEN,
		//			Arabic = data.LocationAR
		//		},
		//		Description = new EnglishArabicView()
		//		{
		//			English = data.DescriptionEN,
		//			Arabic = data.DescriptionAR
		//		},
		//		DaysCount = data.EventDays.Count,
		//		DescriptionName = language == LanguageType.EN ? data.DescriptionEN,
		//		data.DescriptionAR),
		//                TextName = language == LanguageType.EN ? data.TextEN, data.TextAR),
		//                LocationName = language == LanguageType.EN ? data.LocationEN, data.LocationAR),
		//                MapAutocompleteView = new MapAutocompleteView()
		//				{
		//					Latitude = data.Latitude,
		//					Longitude = data.Longitude,
		//					Text = data.MapText
		//				},
		//                EventDays = data.EventDays.Select<EventDay, EventDayView>((Func<EventDay, EventDayView>)(d => new EventDayView()
		//				{
		//					Date = d.Date,
		//					EndTime = d.EndTime,
		//					StartTime = d.StartTime,
		//					ID = d.ID,
		//					IsMeetingRequestCreated = !string.IsNullOrEmpty(d.MeetingRequestExchangeId)
		//				})).ToList<EventDayView>()

		//			};
		//model.MapAutocompleteView = new MapAutocompleteView()
		//{
		//	Latitude = model.Latitude,
		//                Longitude = model.Longitude,
		//                Text = model.MapText

		//			};
		//            if (cruchDays)
		//				this.CrunchDays(model);
		//            return model;

		//		}

		//public EventViewModel GetList(
		//  PagingView pagingView,
		//  int batchID,
		//  bool cruchDays = true,
		//  string SearchText = null)
		//{
		//	IEnumerable<EventView> source = _appDbContext.Events.Where(e => e.BatchId == (int?)batchID)).Select(e => new
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		LocationEN = e.LocationEN,
		//		LocationAR = e.LocationAR,
		//		Longitude = e.Longitude,
		//		TextAR = e.TextAR,
		//		TextEN = e.TextEN,
		//		EventDays = e.EventDays,
		//		DescriptionAR = e.DescriptionAR,
		//		DescriptionEN = e.DescriptionEN
		//	}).AsEnumerable().Select(e => new EventView()
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		Longitude = e.Longitude,
		//		TextName = language == LanguageType.EN ? e.TextEN,
		//		e.TextAR),
		//                LocationName = language == LanguageType.EN ? e.LocationEN, e.LocationAR),
		//                DescriptionName = language == LanguageType.EN ? e.DescriptionEN, e.DescriptionAR),
		//                Text = new EnglishArabicView()
		//				{
		//					English = e.TextEN,
		//					Arabic = e.TextAR
		//				},
		//                Location = new EnglishArabicView()
		//				{
		//					English = e.LocationEN,
		//					Arabic = e.LocationAR
		//				},
		//                DaysCount = e.EventDays.Count,
		//                EventDays = e.EventDays.Select<EventDay, EventDayView>((Func<EventDay, EventDayView>)(d => new EventDayView()
		//				{
		//					Date = d.Date,
		//					EndTime = d.EndTime,
		//					StartTime = d.StartTime,
		//					ID = d.ID
		//				})).ToList<EventDayView>()

		//			}).AsEnumerable<EventView>();
		//            pagingView.TotalCount = source.Count<EventView>();
		//            IEnumerable<EventView> list = (IEnumerable<EventView>)source.Skip<EventView>(pagingView.Skip).Take<EventView>(pagingView.Take).ToList<EventView>();

		//			this.CrunchDays(list);
		//            return new EventViewModel()
		//{
		//	List = new List<EventView>(list),
		//                pagingView = pagingView

		//			};
		//        }

		//        public EventViewModel GetList(
		//		  PagingView pagingView,
		//		  string SearchText = null,
		//		  bool forProfile = false,
		//		  bool ispublic = false)
		//{
		//	// ISSUE: object of a compiler-generated type is created
		//	// ISSUE: variable of a compiler-generated type
		//	EventLogic.\u003C\u003Ec__DisplayClass22_0 cDisplayClass220 = new EventLogic.\u003C\u003Ec__DisplayClass22_0();
		//	// ISSUE: reference to a compiler-generated field
		//	cDisplayClass220.forProfile = forProfile;
		//	// ISSUE: reference to a compiler-generated field
		//	cDisplayClass220.ispublic = ispublic;
		//	// ISSUE: reference to a compiler-generated field
		//	cDisplayClass220.\u003C\u003E4__this = this;
		//	// ISSUE: reference to a compiler-generated field
		//	// ISSUE: reference to a compiler-generated field
		//	// ISSUE: reference to a compiler-generated method
		//	IEnumerable<EventView> source = _appDbContext.Events.Where(e => cDisplayClass220.forProfile ? (cDisplayClass220.ispublic ? e.BatchId == new int?() : e.BatchId == new int?() || e.Batch.Applications.Any(app => app.ProfileId == userId && (app.StatusItemId == (int?)59006 || app.StatusItemId == (int?)59009)))) : true)).Select(e => new
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		LocationEN = e.LocationEN,
		//		LocationAR = e.LocationAR,
		//		Longitude = e.Longitude,
		//		TextAR = e.TextAR,
		//		TextEN = e.TextEN,
		//		EventDays = e.EventDays,
		//		DescriptionAR = e.DescriptionAR,
		//		DescriptionEN = e.DescriptionEN,
		//		MapText = e.MapText
		//	}).AsEnumerable().Select(new Func<\u003C\u003Ef__AnonymousType132<int, string, string, string, string, string, string, ICollection<EventDay>, string, string, string>, EventView>(cDisplayClass220.\u003CGetList\u003Eb__2)).AsEnumerable<EventView>();
		//	pagingView.TotalCount = source.Count<EventView>();
		//	IEnumerable<EventView> list = (IEnumerable<EventView>)source.Skip<EventView>(pagingView.Skip).Take<EventView>(pagingView.Take).ToList<EventView>();
		//	return new EventViewModel()
		//	{
		//		List = new List<EventView>(list),
		//		pagingView = pagingView
		//	};
		//}

		//public EventViewModel GetEventsByFilterType(PagingView pagingView, int FilterTypeID)
		//{
		//	EventViewModel eventViewModel1 = new EventViewModel();
		//	EventViewModel eventViewModel2;
		//	switch (FilterTypeID)
		//	{
		//		case 1:
		//			eventViewModel2 = this.GetList(pagingView, null, true, true);
		//			eventViewModel2.ActionName = "GetForCalendar";
		//			break;
		//		case 2:
		//			eventViewModel2 = this.GetListByBatches(pagingView, null);
		//			eventViewModel2.ActionName = "GetBatchEventsForCalendar";
		//			break;
		//		case 3:
		//			eventViewModel2 = this.GetAttendingEvents(pagingView, null);
		//			eventViewModel2.ActionName = "GetAttendingEventsForCalendar";
		//			break;
		//		case 4:
		//			eventViewModel2 = this.GetNotAttendingEvents(pagingView, null);
		//			eventViewModel2.ActionName = "GetNotAttendingEventsForCalendar";
		//			break;
		//		case 5:
		//			eventViewModel2 = this.GetMaBeEvents(pagingView, null);
		//			eventViewModel2.ActionName = "GetMayBeEventsForCalendar";
		//			break;
		//		default:
		//			eventViewModel2 = this.GetList(pagingView, null, false, false);
		//			eventViewModel2.ActionName = "GetForCalendar";
		//			break;
		//	}
		//	return eventViewModel2;
		//}

		//public EventView GetListForCalender(int userId, string SearchText = null, LanguageType language = LanguageType.AR)
		//{
		//	var pageView = new PagingView
		//	{
		//		TotalCount = 50
		//	};

		//	var source = _appDbContext.Events.Where(e => e.BatchId == new int?() || e.Batch.Applications.Any(app => app.ProfileId == userId && (app.StatusItemId == (int?)59006 || app.StatusItemId == (int?)59009)))
		//		.Select(e => new EventView()
		//		{
		//			ID = e.Id,
		//			Latitude = e.Latitude,
		//			Longitude = e.Longitude,
		//			TextName = language == LanguageType.EN ? e.TextEN : e.TextAR,
		//			LocationName = language == LanguageType.EN ? e.LocationEN : e.LocationAR,
		//			DescriptionName = language == LanguageType.EN ? e.DescriptionEN : e.DescriptionAR,
		//			Text = new EnglishArabicView()
		//			{
		//				English = e.TextEN,
		//				Arabic = e.TextAR
		//			},
		//			Location = new EnglishArabicView()
		//			{
		//				English = e.LocationEN,
		//				Arabic = e.LocationAR
		//			},
		//			DaysCount = e.EventDays.Count,
		//			EventDays = e.EventDays.Select<EventDay, EventDayView>((Func<EventDay, EventDayView>)(d => new EventDayView()
		//			{
		//				Date = d.Date,
		//				EndTime = d.EndTime,
		//				StartTime = d.StartTime,
		//				EventID = e.ID,
		//				ID = d.ID,
		//				EventTitle = language == LanguageType.EN ? e.TextEN,
		//				e.TextAR)

		//		}).ToList()

		//			}).AsEnumerable();
		//pagingView.TotalCount = source.Count<EventView>();
		//            IEnumerable<EventView> list = (IEnumerable<EventView>)source.Skip<EventView>(pagingView.Skip).Take<EventView>(pagingView.Take).ToList<EventView>();
		//            return new EventViewModel()
		//{
		//	List = new List<EventView>(list),
		//                pagingView = pagingView

		//			};
		//        }

		//        public EventViewModel GetListByBatches(PagingView pagingView, string SearchText = null)
		//{
		//	IEnumerable<EventView> source = _appDbContext.Events.Where(e => e.BatchId != new int?() && e.Batch.Applications.Any(app => app.ProfileId == userId && (app.StatusItemId == (int?)59006 || app.StatusItemId == (int?)59009))).Select(e => new
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		LocationEN = e.LocationEN,
		//		LocationAR = e.LocationAR,
		//		Longitude = e.Longitude,
		//		TextAR = e.TextAR,
		//		TextEN = e.TextEN,
		//		EventDays = e.EventDays,
		//		DescriptionAR = e.DescriptionAR,
		//		DescriptionEN = e.DescriptionEN
		//	}).AsEnumerable().Select(e => new EventView()
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		Longitude = e.Longitude,
		//		TextName = language == LanguageType.EN ? e.TextEN,
		//		e.TextAR),
		//		LocationName = language == LanguageType.EN ? e.LocationEN, e.LocationAR),
		//                DescriptionName = language == LanguageType.EN ? e.DescriptionEN, e.DescriptionAR),
		//                Text = new EnglishArabicView()
		//				{
		//					English = e.TextEN,
		//					Arabic = e.TextAR
		//				},
		//                Location = new EnglishArabicView()
		//				{
		//					English = e.LocationEN,
		//					Arabic = e.LocationAR
		//				},
		//                DaysCount = e.EventDays.Count,
		//                EventDays = e.EventDays.Select<EventDay, EventDayView>((Func<EventDay, EventDayView>)(d => new EventDayView()
		//				{
		//					Date = d.Date,
		//					EndTime = d.EndTime,
		//					StartTime = d.StartTime,
		//					EventID = e.ID,
		//					ID = d.ID,
		//					EventTitle = language == LanguageType.EN ? e.TextEN,
		//					e.TextAR)

		//				})).ToList<EventDayView>()
		//            }).AsEnumerable<EventView>();
		//            pagingView.TotalCount = source.Count<EventView>();
		//            IEnumerable<EventView> list = (IEnumerable<EventView>)source.Skip<EventView>(pagingView.Skip).Take<EventView>(pagingView.Take).ToList<EventView>();
		//            return new EventViewModel()
		//{
		//	List = new List<EventView>(list),
		//                pagingView = pagingView

		//			};
		//        }

		//        public EventViewModel GetAttendingEvents(PagingView pagingView, string SearchText = null)
		//{
		//	IEnumerable<EventView> source = _appDbContext.Events.Where(b => b.Profile_Events.Any<Profile_Events>((Func<Profile_Events, bool>)(pe => pe.EventStatusItemID == 69001)))).Select(e => new
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		LocationEN = e.LocationEN,
		//		LocationAR = e.LocationAR,
		//		Longitude = e.Longitude,
		//		TextAR = e.TextAR,
		//		TextEN = e.TextEN,
		//		EventDays = e.EventDays,
		//		DescriptionAR = e.DescriptionAR,
		//		DescriptionEN = e.DescriptionEN
		//	}).AsEnumerable().Select(e => new EventView()
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		Longitude = e.Longitude,
		//		TextName = language == LanguageType.EN ? e.TextEN,
		//		e.TextAR),
		//                LocationName = language == LanguageType.EN ? e.LocationEN, e.LocationAR),
		//                DescriptionName = language == LanguageType.EN ? e.DescriptionEN, e.DescriptionAR),
		//                Text = new EnglishArabicView()
		//				{
		//					English = e.TextEN,
		//					Arabic = e.TextAR
		//				},
		//                Location = new EnglishArabicView()
		//				{
		//					English = e.LocationEN,
		//					Arabic = e.LocationAR
		//				},
		//                DaysCount = e.EventDays.Count,
		//                EventDays = e.EventDays.Select<EventDay, EventDayView>((Func<EventDay, EventDayView>)(d => new EventDayView()
		//				{
		//					Date = d.Date,
		//					EndTime = d.EndTime,
		//					StartTime = d.StartTime,
		//					EventID = e.ID,
		//					ID = d.ID,
		//					EventTitle = language == LanguageType.EN ? e.TextEN,
		//					e.TextAR)

		//				})).ToList<EventDayView>()
		//            }).AsEnumerable<EventView>();
		//            pagingView.TotalCount = source.Count<EventView>();
		//            IEnumerable<EventView> list = (IEnumerable<EventView>)source.Skip<EventView>(pagingView.Skip).Take<EventView>(pagingView.Take).ToList<EventView>();
		//            return new EventViewModel()
		//{
		//	List = new List<EventView>(list),
		//                pagingView = pagingView

		//			};
		//        }

		//        public EventViewModel GetNotAttendingEvents(
		//		  PagingView pagingView,
		//		  string SearchText = null)
		//{
		//	IEnumerable<EventView> source = _appDbContext.Events.Where(b => b.Profile_Events.Any<Profile_Events>((Func<Profile_Events, bool>)(pe => pe.EventStatusItemID == 69003)))).Select(e => new
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		LocationEN = e.LocationEN,
		//		LocationAR = e.LocationAR,
		//		Longitude = e.Longitude,
		//		TextAR = e.TextAR,
		//		TextEN = e.TextEN,
		//		EventDays = e.EventDays,
		//		DescriptionAR = e.DescriptionAR,
		//		DescriptionEN = e.DescriptionEN
		//	}).AsEnumerable().Select(e => new EventView()
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		Longitude = e.Longitude,
		//		TextName = language == LanguageType.EN ? e.TextEN,
		//		e.TextAR),
		//                LocationName = language == LanguageType.EN ? e.LocationEN, e.LocationAR),
		//                DescriptionName = language == LanguageType.EN ? e.DescriptionEN, e.DescriptionAR),
		//                Text = new EnglishArabicView()
		//				{
		//					English = e.TextEN,
		//					Arabic = e.TextAR
		//				},
		//                Location = new EnglishArabicView()
		//				{
		//					English = e.LocationEN,
		//					Arabic = e.LocationAR
		//				},
		//                DaysCount = e.EventDays.Count,
		//                EventDays = e.EventDays.Select<EventDay, EventDayView>((Func<EventDay, EventDayView>)(d => new EventDayView()
		//				{
		//					Date = d.Date,
		//					EndTime = d.EndTime,
		//					StartTime = d.StartTime,
		//					EventID = e.ID,
		//					ID = d.ID,
		//					EventTitle = language == LanguageType.EN ? e.TextEN,
		//					e.TextAR)

		//				})).ToList<EventDayView>()
		//            }).AsEnumerable<EventView>();
		//            pagingView.TotalCount = source.Count<EventView>();
		//            IEnumerable<EventView> list = (IEnumerable<EventView>)source.Skip<EventView>(pagingView.Skip).Take<EventView>(pagingView.Take).ToList<EventView>();
		//            return new EventViewModel()
		//{
		//	List = new List<EventView>(list),
		//                pagingView = pagingView

		//			};
		//        }

		//        public EventViewModel GetMaBeEvents(PagingView pagingView, string SearchText = null)
		//{
		//	IEnumerable<EventView> source = _appDbContext.Events.Where(b => b.Profile_Events.Any<Profile_Events>((Func<Profile_Events, bool>)(pe => pe.EventStatusItemID == 69002)))).Select(e => new
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		LocationEN = e.LocationEN,
		//		LocationAR = e.LocationAR,
		//		Longitude = e.Longitude,
		//		TextAR = e.TextAR,
		//		TextEN = e.TextEN,
		//		EventDays = e.EventDays,
		//		DescriptionAR = e.DescriptionAR,
		//		DescriptionEN = e.DescriptionEN
		//	}).AsEnumerable().Select(e => new EventView()
		//	{
		//		ID = e.ID,
		//		Latitude = e.Latitude,
		//		Longitude = e.Longitude,
		//		TextName = language == LanguageType.EN ? e.TextEN,
		//		e.TextAR),
		//                LocationName = language == LanguageType.EN ? e.LocationEN, e.LocationAR),
		//                DescriptionName = language == LanguageType.EN ? e.DescriptionEN, e.DescriptionAR),
		//                Text = new EnglishArabicView()
		//				{
		//					English = e.TextEN,
		//					Arabic = e.TextAR
		//				},
		//                Location = new EnglishArabicView()
		//				{
		//					English = e.LocationEN,
		//					Arabic = e.LocationAR
		//				},
		//                DaysCount = e.EventDays.Count,
		//                EventDays = e.EventDays.Select<EventDay, EventDayView>((Func<EventDay, EventDayView>)(d => new EventDayView()
		//				{
		//					Date = d.Date,
		//					EndTime = d.EndTime,
		//					StartTime = d.StartTime,
		//					EventID = e.ID,
		//					ID = d.ID,
		//					EventTitle = language == LanguageType.EN ? e.TextEN,
		//					e.TextAR)

		//				})).ToList<EventDayView>()
		//            }).AsEnumerable<EventView>();
		//            pagingView.TotalCount = source.Count<EventView>();
		//            IEnumerable<EventView> list = (IEnumerable<EventView>)source.Skip<EventView>(pagingView.Skip).Take<EventView>(pagingView.Take).ToList<EventView>();
		//            return new EventViewModel()
		//{
		//	List = new List<EventView>(list),
		//                pagingView = pagingView

		//			};
		//        }

		//        public void SetEventDesicion(int decisionID, int eventID)
		//{
		//	if (this._profile_EventsDAl.Any<Profile_Events>((Expression<Func<Profile_Events, bool>>)(m => m.EventID == eventID && m.ProfileID == userId)))
		//		this._profile_EventsDAl.Where<Profile_Events>((Expression<Func<Profile_Events, bool>>)(m => m.EventID == eventID && m.ProfileID == userId)).FirstOrDefault<Profile_Events>().EventStatusItemID = decisionID;
		//	else
		//		this._profile_EventsDAl.Add(new Profile_Events()
		//		{
		//			ProfileID = userId,
		//			EventID = eventID,
		//			EventStatusItemID = decisionID
		//		});
		//	_appDbContext.SaveChanges();
		//}

		//public int GetUserDecision(int eventID)
		//{
		//	Profile_Events profileEvents = this._profile_EventsDAl.FirstOrDefault<Profile_Events>((Expression<Func<Profile_Events, bool>>)(m => m.EventID == eventID && m.ProfileID == userId));
		//	if (profileEvents != null)
		//	{
		//		if (profileEvents.EventStatusItemID == 69001)
		//			return 1;
		//		if (profileEvents.EventStatusItemID == 69002)
		//			return 2;
		//		if (profileEvents.EventStatusItemID == 69003)
		//			return 3;
		//	}
		//	return 4;
		//}

		//public List<EventDayView> GetEventDaysList(int batchID)
		//{
		//	return _appDbContext.EventDays.Where(e => e.Event.BatchID == (int?)batchID)).Select(e => new
		//	{
		//		ID = e.ID,
		//		Event = e.Event,
		//		EventID = e.EventID,
		//		EndTime = e.EndTime,
		//		Date = e.Date,
		//		StartTime = e.StartTime
		//	}).AsEnumerable().Select(d => new EventDayView()
		//	{
		//		Date = d.Date,
		//		EndTime = d.EndTime,
		//		StartTime = d.StartTime,
		//		ID = d.ID,
		//		Event = new EventView()
		//		{
		//			ID = d.Event.ID,
		//			Latitude = d.Event.Latitude,
		//			Longitude = d.Event.Longitude,
		//			TextName = language == LanguageType.EN ? d.Event.TextEN,
		//			d.Event.TextAR),
		//                    LocationName = language == LanguageType.EN ? d.Event.LocationEN, d.Event.LocationAR),
		//                    DescriptionName = language == LanguageType.EN ? d.Event.DescriptionEN, d.Event.DescriptionAR),
		//                    Text = new EnglishArabicView()
		//					{
		//						English = d.Event.TextEN,
		//						Arabic = d.Event.TextAR
		//					},
		//                    Location = new EnglishArabicView()
		//					{
		//						English = d.Event.LocationEN,
		//						Arabic = d.Event.LocationAR
		//					},
		//                    DaysCount = d.Event.EventDays.Count,
		//                    MapAutocompleteView = new MapAutocompleteView()
		//					{
		//						Latitude = d.Event.Latitude,
		//						Longitude = d.Event.Longitude,
		//						Text = d.Event.MapText
		//					}

		//				}
		//            }).OrderBy<EventDayView, DateTime>((Func<EventDayView, DateTime>)(e => e.Date)).ThenBy<EventDayView, DateTime>((Func<EventDayView, DateTime>)(e => e.StartTime)).ToList<EventDayView>();
		//        }




		//        private void CreateForMultiProfilesAsync(NotificationView notificationView, ParentType ParentType, List<int> profilesIDs = null)
		//{
		//	List<int> intList;
		//	if (profilesIDs == null)
		//	{
		//		intList = _appDbContext.UserInfos.Where(x => x.IsActive == true).Select(u => u.Id).ToList();
		//	}
		//	else
		//		intList = profilesIDs;
		//	List<int> profiles = intList;
		//	CreateForMultiProfiles(notificationView, (ParentType?)ParentType, profiles);
		//}

		//private void CreateForMultiProfiles(NotificationView notificationView, ParentType? ParentType, List<int> profilesIDs = null)
		//{
		//	NotificationGenericObject notificationGenericObject1 = _mongoDbContext.NotificationGenericObjects.Find(e => e.UserID == profilesIDs.OrderByDescending(p => p).FirstOrDefaultAsync()).FirstOrDefaultAsync();
		//	if (notificationGenericObject1 != null && notificationGenericObject1.Notifications.Any(n => n.ParentID == notificationView.ParentID && n.ParentTypeID == (int)notificationView.ParentTypeID))
		//		return;
		//	foreach (int profilesId in profilesIDs)
		//	{
		//		try
		//		{
		//			NotificationGenericObject notificationGenericObject2 = _mongoDbContext.NotificationGenericObjects.Find(e => e.UserID == profilesId).FirstOrDefaultAsync();
		//			if (notificationGenericObject2 != null)
		//			{
		//				if (notificationGenericObject2.Notifications.Any(n => n.ParentID == notificationView.ParentID && n.ParentTypeID == (int)notificationView.ParentTypeID))
		//					continue;
		//			}
		//			notificationView.OwnerID = profilesId;
		//			this.CreateAsyc(notificationView);
		//		}
		//		catch (Exception ex)
		//		{
		//			throw ex;
		//		}
		//	}
		//}

		//private void CreateAsyc(NotificationView notificationView)
		//{
		//	NotificationGenericObject document = _mongoDbContext.NotificationGenericObjects.Find(e => e.UserID == notificationView.OwnerID).FirstOrDefaultAsync();
		//	if (document == null)
		//	{
		//		_mongoDbContext.NotificationGenericObjects.InsertOne(new NotificationGenericObject()
		//		{
		//			ID = ObjectId.GenerateNewId(),
		//			UserID = notificationView.OwnerID,
		//			UnseenNotificationCounter = 1,
		//			Notifications = new List<Notification>()
		//		  {
		//			new Notification()
		//			{
		//			  Modified = DateTime.Now,
		//			  Created = DateTime.Now,
		//			  SenderID = notificationView.SenderID,
		//			  ID = ObjectId.GenerateNewId(),
		//			  ActionID = (int) notificationView.ActionID,
		//			  ParentTypeID = (int) notificationView.ParentTypeID,
		//			  ParentID = notificationView.ParentID.ToString(),
		//			  IsRead = false,
		//			  GeneralNotification = notificationView.GeneralNotification
		//			}
		//		  }
		//		});
		//	}
		//	else
		//	{
		//		document.UnseenNotificationCounter = ++document.UnseenNotificationCounter;
		//		document.Notifications.Add(new Notification()
		//		{
		//			Modified = DateTime.Now,
		//			Created = DateTime.Now,
		//			SenderID = notificationView.SenderID,
		//			ID = ObjectId.GenerateNewId(),
		//			ActionID = (int)notificationView.ActionID,
		//			ParentTypeID = (int)notificationView.ParentTypeID,
		//			ParentID = notificationView.ParentID.ToString(),
		//			IsRead = false,
		//			GeneralNotification = notificationView.GeneralNotification
		//		});
		//		_mongoDbContext.NotificationGenericObjects.ReplaceOne(e => e.UserID == notificationView.OwnerID, document);
		//	}
		//}
	}
}
