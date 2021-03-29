using System;
using System.Threading;

namespace Uaeglp.ViewModels
{
	public class EnglishArabicView
	{
        public EnglishArabicView()
        {
        }

        public EnglishArabicView(string english, string arabic)
        {
            this.English = english;
            this.Arabic = arabic;
        }

        public string English { get; set; }

        public string Arabic { get; set; }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(this.Arabic) || string.IsNullOrEmpty(this.English);
            }
        }

        public bool IsAnyValueAvailable
        {
            get
            {
                return !string.IsNullOrEmpty(this.Arabic) || !string.IsNullOrEmpty(this.English);
            }
        }

        public bool IsArabicEmpty
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.Arabic);
            }
        }

        public bool IsEnglishEmpty
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.English);
            }
        }

        public string ExtraParams { get; set; }

        public string Current
        {
            get
            {
                return this.Get();
            }
        }

        public string Current_FailOver
        {
            get
            {
                return this.Get_FailOver();
            }
        }

        public string Get()
        {
            return Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "ar" ? this.Arabic : this.English;
        }

        public string Get_FailOver()
        {
            return Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "ar" ? (string.IsNullOrEmpty(this.Arabic) ? this.English : this.Arabic) : (string.IsNullOrEmpty(this.English) ? this.Arabic : this.English);
        }

        public string GetPreferred(string preferredLanguageKey)
        {
            if (string.IsNullOrEmpty(preferredLanguageKey))
                throw new ArgumentNullException("preferredLanguageKey cannot be empty or null");
            return preferredLanguageKey == "ar" ? this.Arabic : this.English;
        }

        public string GetPreferred_FailOver(string preferredLanguageKey)
        {
            if (string.IsNullOrEmpty(preferredLanguageKey))
                throw new ArgumentNullException("preferredLanguageKey cannot be empty or null");
            return preferredLanguageKey == "ar" ? (string.IsNullOrEmpty(this.Arabic) ? this.English : this.Arabic) : (string.IsNullOrEmpty(this.English) ? this.Arabic : this.English);
        }

        public override string ToString()
        {
            return this.Current;
        }
    }
}
