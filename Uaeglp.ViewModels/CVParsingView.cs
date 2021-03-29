using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class CVParsingView
    {
        public IFormFile Resume { get; set; }
        public int userId { get; set; }
    }
}
