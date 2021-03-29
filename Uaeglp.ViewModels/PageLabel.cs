using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class PageLabel
    {
        public Guid ID { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
        public List<PageLabelRes> Data { get; set; }
    }

    public class PageLabelRes
    {
        public string Page_Name { get; set; }
        public string Page_Display_Name { get; set; }
        public string Label_Name { get; set; }
        public List<LanguageData> LanguageData { get; set; }
    }

    public class LanguageData
    {
        public string value { get; set; }
        public string language { get; set; }
    }

    public class PageLabelReq
    {
        public string page_name { get; set; }
        public DateTime FromDate { get; set; }
    }

    public partial class Page
    {
        public string Name { get; set; }

        public string DispName { get; set; }

        public List<Label> Labels { get; set; }
    }

    public partial class PageNew
    {
        public Dictionary<string, string> PageKeyValue { get; set; }
    }

    public partial class Label
    {
        public string Text { get; set; }

        public List<Values> Values { get; set; }
    }

    public partial class Values
    {
        public string En { get; set; }

        public string Ar { get; set; }
    }
    public class LabelNames
    {
        public string PageName { get; set; }
        public string PageDisplayName { get; set; }
        public string LabelName { get; set; }
        public string English { get; set; }
        public string Arabic { get; set; }
    }
    public class PageNames
    {
        public string PageName { get; set; }
    }
}
