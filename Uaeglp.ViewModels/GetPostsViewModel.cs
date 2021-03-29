using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels
{
    public class GetPostsViewModel
    {

      public int UserId { get; set; } 
      public List<int> FilterType { get; set; }
      public int Skip { get; set; } = 0;
      public int Limit { get; set; } = 5;
      public SortType SortBy { get; set; } = SortType.Desc;

    }
}
