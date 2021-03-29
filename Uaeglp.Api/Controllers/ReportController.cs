using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using Uaeglp.Contracts;

namespace Uaeglp.Api.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IAssessmentService _service;

        public ReportController(ILogger<ReportController> logger, IAssessmentService service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }


        public ActionResult Assessment(int ProfileAssessmentToolID)
        {
            var individualToolReportView = _service.ExportAssessmentReportPDF(ProfileAssessmentToolID).Result.Data;
            string.Format("--print-media-type --footer-spacing -10 --allow {0} --footer-html {0} --footer-spacing -10", this.Url.Action("Footer", "Home", new
            {
                area = ""
            }, "http"));
            var viewAsPdf = new ViewAsPdf(individualToolReportView.ViewPath, individualToolReportView)
            {
                PageMargins = new Margins(0, 0, 0, 0),
                CustomSwitches = "--debug-javascript --no-stop-slow-scripts --javascript-delay 1000",
                FileName = Uri.EscapeDataString(individualToolReportView.FileName + ".pdf").Replace(",", "%2C")
            };
            return viewAsPdf;
        }
    }
}