using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Event
{
    public class EnglishArabicViewEvents
    {
        public EnglishArabicViewEvents()
        {
        }

        public EnglishArabicViewEvents(string english, string arabic)
        {
            this.English = english;
            this.Arabic = arabic;
        }

        public string English { get; set; }

        public string Arabic { get; set; }

    }
}
