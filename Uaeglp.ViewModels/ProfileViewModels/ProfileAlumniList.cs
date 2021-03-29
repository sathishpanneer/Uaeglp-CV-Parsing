using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class ProfileAlumniList
    {
        public int BatchNumber { get; set; }
        public int BatchYear { get; set; }
        public string ProgramTitleEn { get; set; }
        public string ProgramTitleAr { get; set; }
        public int ProgramCategoryID { get; set; }
        public string ProgramCategoryEn { get; set; }
        public string ProgramCategoryAr { get; set; }
        public string StatusEn { get; set; }
        public string StatusAr { get; set; }
    }
}
