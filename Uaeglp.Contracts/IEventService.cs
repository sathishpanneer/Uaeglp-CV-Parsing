using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.Event;

namespace Uaeglp.Contracts
{
	public interface IEventService
	{
		Task<IEventResponse<EventView>> GetEvent(int eventID, bool crunchDays = false, LanguageType language = LanguageType.AR);
		Task<IEventResponse<List<EventDayView>>> GetEventDays(int batchID, LanguageType language = LanguageType.AR);
		Task<IEventResponse<List<EventView>>> GetEvents(int userId, LanguageType language = LanguageType.AR, bool forProfile = false, bool ispublic = false);
		Task<IEventResponse<List<EventViewNew>>> GetEventsByMonth(int userId, int month, int year, LanguageType language = LanguageType.AR, bool forProfile = false, bool ispublic = false);
		Task<IEventResponse<List<EventView>>> GetEventsByBatches(int userId, LanguageType language = LanguageType.AR);
		Task<IEventResponse<List<EventView>>> GetEventsForCalender(int userId, LanguageType language = LanguageType.AR);
		Task<IEventResponse<List<EventView>>> GetAttendingEvents(LanguageType language = LanguageType.AR);
		Task<IEventResponse<List<EventView>>> GetNotAttendingEvents(LanguageType language = LanguageType.AR);
		Task<IEventResponse<List<EventView>>> GetMaBeEvents(LanguageType language = LanguageType.AR);
		Task<IEventResponse<int>> SetEventDesicion(int UserID, int decisionID, int eventID);
		Task<IEventResponse<MeetingRequestView>> GetMeetingRequest(int eventID);
		Task<IEventResponse<object>> UpdateEvent(EventView model);
		Task<IEventResponse<int>> GetUserDecision(int userId, int eventID);
		Task<IEventResponse<int>> GetTotalCount();
	}
}
