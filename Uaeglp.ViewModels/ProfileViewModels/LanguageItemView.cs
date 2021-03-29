using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class LanguageItemView
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int LanguageItemId { get; set; }
        public int ProficiencyItemId { get; set; }
        public LookupItemView LanguageItem { get; set; } 
        public LookupItemView ProficiencyItem { get; set; } 

    }
}
