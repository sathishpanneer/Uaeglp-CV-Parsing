using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels
{
   public class UserDeviceInfoViewModel : Result
    {


        [Required]
        public int UserId { get; set; }

        [Required]
        public string DeviceId { get; set; }

        [Required]
        public DeviceType DeviceType { get; set; }


        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime? Modified { get; set; }
    }
}
