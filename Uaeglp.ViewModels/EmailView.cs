using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class EmailView
    {
        public Guid ID { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
        public int OTP { get; set; }

    }
}
