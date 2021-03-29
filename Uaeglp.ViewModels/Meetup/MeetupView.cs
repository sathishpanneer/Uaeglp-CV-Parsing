using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.Event;

namespace Uaeglp.ViewModels.Meetup
{
	public class MeetupView
	{
        public MeetupView()
        {
            this.Agendas = new List<AgendaView>();
            this.MapAutocompleteView = new MapAutocompleteView();
        }

        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public MapAutocompleteView MapAutocompleteView { get; set; }

        public string Descriotion { get; set; }



        public bool IsOwner { get; set; }

        public MeetupLangView UserName { get; set; }

        public DateTime Date { get; set; }

        public List<AgendaView> Agendas { get; set; }

        public int? ProfileDesicionID { get; set; }
        public MeetupLangView ProfileDesicionItem { get; set; }
        public List<ParticipantsView> ParticipantsList { get; set; }

        public int ProfileImageId { get; set; }

        public string ProfileIDEncrypted { get; set; }

        public int GroupID { get; set; }

        public bool IsPublished { get; set; }

        public bool IsAdmin { get; set; }

        public int StatusItemID { get; set; }

        public MeetupLangView StatusItem { get; set; }

        public string CreatedBY { get; set; }

        public int NumberOfGoing { get; set; }

        public string RejectionComment { get; set; }

        public bool DisplayAdminDecision { get; set; }

        public bool DisplayProfileDecision { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public int? MeetupPictureId { get; set; }
        public string MeetupPictureURL
        {
            get
            {
                if (MeetupPictureId != null)
                {
                    return ConstantUrlPath.DocumentDownloadPath + MeetupPictureId;
                }

                return null;
            }
        }
    }
}
