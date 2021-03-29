using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.Meetup
{
    public class GroupPagedView
    {
        public GroupPagedView()
        {
            this.Groups = new List<GroupView>();
        }

        public int Count { get; set; }

        public List<GroupView> Groups { get; set; }

        public PagingView pagingView { get; set; }
    }
}
