using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public ReportsController(ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;

        }

        // GET: ReportsSender
        [HttpGet]
        public async Task<ActionResult<List<ReportShortViewModel>>> Index()
        {
            //можно заменить на include позже
            List<Report> reports = await _context.Reports.ToListAsync();
            List<ReportShortViewModel> models = new List<ReportShortViewModel>(reports.Count);
            ReportDescription reportDescription;
            foreach (Report report in reports){
                ReportShortViewModel reportView = new ReportShortViewModel();
                reportDescription = await _context.ReportDescription.FindAsync(report.ReportDescription_Id);
                reportView.Id = report.Id;
                reportView.SenderName = report.SenderName;
                reportView.ReportName = report.ReportName;
                reportView.DateCreated = report.DateCreated;
                reportView.Description =  reportDescription.Description;
                reportView.Status = report.Status;
                models.Add(reportView);
            }
            
            return models;
            
                //return View(await _context.Report.ToListAsync());
        }
        

        // GET: ReportsSender/Details/5
        [Route("datails")]
        public async Task<ActionResult<Report>> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return report;
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ReportFullViewModel>> ViewReport(int id)
        {
            //можно заменить на include позже
            //var report = _context.Report.Include(p => p.).Where(p => p.BlogPostId == id).FirstOrDefault();
            var report = await _context.Reports.FindAsync(id);

            if (report != null)
            {
                var reportTextContent = await _context.ReportTextContents.FindAsync(report.TextContent_Id);
                var reportDescription = await _context.ReportDescription.FindAsync(report.ReportDescription_Id);
                
                ReportFullViewModel model = new ReportFullViewModel {
                    Id = id,
                    SenderName = report.SenderName,
                    ReportName = report.ReportName,
                    TextContent = reportTextContent.TextContent,
                    DateCreated = report.DateCreated,
                    DateSent = report.DateSent,
                    DateReceived = report.DateReceived,
                    Description = reportDescription.Description,
                    Status = report.Status 
                };
                if (report.Filename != null)
                {
                    string path = _appEnvironment.WebRootPath + "/files/" + report.Filename;
                    if (System.IO.File.Exists(path))
                    {
                        model.Filename = "/files/" + report.Filename;
                    }

                }
                return model;
            }
            return NotFound();
        }

        [Route("reportexists")]
        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}
