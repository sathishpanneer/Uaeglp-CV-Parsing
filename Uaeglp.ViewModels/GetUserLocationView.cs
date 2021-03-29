using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class GetUserLocationView
    {
        public HideLocationView HideLocationDetails { get; set; }
        public List<NearByUsers> NearByUsers { get; set; }
    }
}
