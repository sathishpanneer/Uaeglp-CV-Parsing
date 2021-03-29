using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using Uaeglp.Contracts;

namespace Uaeglp.Web.Controllers
{
    public class TestModel
    {
        public string Name { get; set; }
    }

    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IAssessmentService _service;

        public ReportController(ILogger<ReportController> logger, IAssessmentService service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult Test()
        {
            ViewData["Message"] = "Your application description page.";
            var model = new TestModel { Name = "Giorgio" };

            return new ViewAsPdf(model, ViewData)
            {
                PageMargins = new Margins(0, 0, 0, 0),
                CustomSwitches = "--debug-javascript --no-stop-slow-scripts --javascript-delay 1000",
                FileName = Uri.EscapeDataString("test.pdf").Replace(",", "%2C")
            };

            //return new ViewAsPdf(model, ViewData);
        }

        //[HttpGet("file", Name = "GetFile")]
        //public async Task<IActionResult> GetFile()
        //{
        //    var mymodel = Db.SomeCallToGetModel();
        //    return Request.CreatePdfResponse(
        //        "~/Views/Home/Simple.cshtml",
        //        model: mymodel, filename: "simple.pdf");
        //}

        public ActionResult Assessment(int profileAssessmentToolID, string language = "AR")
        {
            var individualToolReportView = _service.ExportAssessmentReportPDF(profileAssessmentToolID, language).Result.Data;

            var viewName = individualToolReportView.ViewPath.Replace(".cshtml", String.Empty);
            var viewAsPdf = new ViewAsPdf(viewName, individualToolReportView)
            {
                PageMargins = new Margins(0, 0, 0, 0),
                CustomSwitches = "--debug-javascript --no-stop-slow-scripts --javascript-delay 1000",
                FileName = Uri.EscapeDataString(individualToolReportView.FileName + ".pdf").Replace(",", "%2C")
            };
            return viewAsPdf;
        }

        public FileResult ExportGroupAssessmrntToolsReport(int GroupID, string language = "AR")
        {
            language = language.ToLower();
            var fileDto = _service.ExportGroupAssessmentToolsReport(GroupID, language);
            return File(fileDto.Bytes, "application/octet-stream", fileDto.NameWithExtension);
        }
    }
}