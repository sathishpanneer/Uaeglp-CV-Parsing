using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ActivityViewModels
{
    public class InitiativeProfileViewModel
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int InitiativeId { get; set; }
        public int StatusItemId { get; set; }
        public string Comment { get; set; }
        public int? ParticipationReferenceID { get; set; }
        public int? StatusID { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
