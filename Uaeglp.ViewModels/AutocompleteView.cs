using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class AutocompleteView
    {
        public AutocompleteView()
        {
        }

        public AutocompleteView(string text, string key)
        {
            this.Text = text;
            this.Key = key;
        }

        public string Text { get; set; }

        public string Key { get; set; }
    }
}
