using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class CompletedProgramView
    {
        public LookupItemView ProgramTypeLookup { get; set; }
        public ProgramView Program { get; set; }
        public int BatchNumber { get; set; }
        public int BatchYear { get; set; }
        public int StatusItemId { get; set; }
        public int ProgramTypeId => ProgramTypeLookup?.Id ?? 0;
        public string Role => StatusItemId
             == (int)ApplicationProgressStatus.Accepted ? "Participant" : "Graduated";
    }
}
