using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.UserLocationViewModels
{
    public class UserLocationResponseView
    {
        public List<string> origin_addresses { get; set; }
        public List<string> destination_addresses { get; set; }
        public List<Elements> rows { get; set; }
        public string status { get; set; }
        public string error_message { get; set; }
    }

    public class Elements
    {
        public List<Distance> elements { get; set; }
        
    }

    public class Distance
    {
        public Value distance { get; set; }
        public Value duration { get; set; }
        public string status { get; set; }
    }

    public class Value
    {
        public string text { get; set; }
        public string value { get; set; }
    }
}
