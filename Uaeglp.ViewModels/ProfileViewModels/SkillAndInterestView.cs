using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class SkillAndInterestView
    {
        public int ProfileId { get; set; }
        public IList<LookupItemView> InterestedItems { get; set; }
        public IList<ProfileSkillView> ProfileSkillItems { get; set; }

        public List<LanguageItemView> LanguageKnown { get; set; }
    }
}
