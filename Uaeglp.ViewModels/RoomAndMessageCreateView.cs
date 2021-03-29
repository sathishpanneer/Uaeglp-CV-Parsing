using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class RoomAndMessageCreateView
    {
        public int userId { get; set; }
        public string RoomTitle { get; set; }
        public string MembersIDs { get; set; }
        public string MessageText { get; set; }
        public IFormFile DocumentData { get; set; }
        public IFormFile ImageData { get; set; }
    }
    public class UpdateRommView
    {
        public int userId { get; set; }
        public string roomID { get; set; }
        public string RoomTitle { get; set; }
    }
    public class AddRoomView
    {
        public int userId { get; set; }
        public string RoomTitle { get; set; }
        public List<int> MembersIDs { get; set; }
    }
}
