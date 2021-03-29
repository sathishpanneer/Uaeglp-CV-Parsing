using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class UserLocationModelView
    {
        public int ID { get; set; }
        public int ProfileID { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool isHideLocation { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
