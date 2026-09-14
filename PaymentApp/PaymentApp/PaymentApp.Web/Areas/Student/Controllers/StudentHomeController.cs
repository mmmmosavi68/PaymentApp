using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PaymentApp.Common;
using PaymentApp.Convertor;
using PaymentApp.Data.Contracts.File;
using PaymentApp.Data.Contracts.Students;
using PaymentApp.Entities.File;
using PaymentApp.Web.Areas.Student.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Web.Areas.Student.Controllers
{
    [Area("Student")]
    public class StudentHomeController : Controller
    {
        private readonly IMapper _mapper;

        private readonly IWebHostEnvironment _env;
        private readonly IUploadedFileRepository _uploadedFileRepository;
        private readonly SiteSettings _siteSettings;
        private readonly IStudentRepository _studentRepository;


        public StudentHomeController(IMapper mapper,
            IStudentRepository studentRepository,
            IWebHostEnvironment env,
            IOptions<SiteSettings> siteSettings,
            IUploadedFileRepository uploadedFileRepository)
        {
            _mapper = mapper;
            _env = env;
            _uploadedFileRepository = uploadedFileRepository;
            _siteSettings = siteSettings.Value;
        }

        // GET: /Student/StudentHome
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return Redirect("/SharedError/AccessDenied");
        }

        // GET: /Student/StudentHome/LoadForm?formId=1
        public async Task<IActionResult> LoadForm(int formId, CancellationToken cancellationToken)
        {
            
            return View("LoadForm");
        }
  
    }
}
