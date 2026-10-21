using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentApp.Common;
using PaymentApp.Data.Contracts.Payment;
using PaymentApp.Entities.Payment;
using PaymentApp.Services.Ipg;
using PaymentApp.Services.Ipg.Models;
using PaymentApp.Web.Areas.Payment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApp.Web.Areas.Payment.Controllers
{
    [AllowAnonymous]
    [Area("Payment")]
    public class PaymentController : Controller
    {
        private readonly IExamRepository _examRepository;
        private readonly IExamPaymentRequestRepository _paymentRepository;
        private readonly IIpgService _ipgService;
        private readonly IpgSettings _settings;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IExamRepository examRepository,
            IExamPaymentRequestRepository paymentRepository,
            IIpgService ipgService,
            IOptions<SiteSettings> settings,
            ILogger<PaymentController> logger)
        {
            _examRepository = examRepository;
            _paymentRepository = paymentRepository;
            _ipgService = ipgService;
            _settings = settings.Value.IpgSettings;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var exams = _examRepository.TableNoTracking
                .Where(e => e.IsActive)
                .Select(e => new ExamSelectViewModel { ExamId = e.Id, Title = e.Title, Amount = e.Amount })
                .ToList();

            _logger.LogInformation("Payment index loaded. ActiveExams={Count}, Ip={Ip}",
                exams.Count, HttpContext.Connection.RemoteIpAddress);

            return View(exams);
        }

        [HttpGet]
        public async Task<IActionResult> Register(int examId, CancellationToken cancellationToken)
        {
            var exam = await _examRepository.GetByIdAsync(cancellationToken, examId);
            if (exam is null)
            {
                _logger.LogWarning("Register form requested for unknown exam. ExamId={ExamId}, Ip={Ip}",
                    examId, HttpContext.Connection.RemoteIpAddress);
                return NotFound();
            }

            _logger.LogInformation("Register form opened. ExamId={ExamId}", examId);
            return View(new RegistrationFormViewModel { ExamId = examId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationFormViewModel form, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Register submitted. ExamId={ExamId}, NationalCode={NationalCode}, Ip={Ip}",
                form.ExamId, (form.NationalCode), HttpContext.Connection.RemoteIpAddress);

            var exam = await _examRepository.GetByIdAsync(cancellationToken, form.ExamId);
            if (exam is null)
            {
                _logger.LogWarning("Register rejected: exam not found. ExamId={ExamId}", form.ExamId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // فقط نام فیلدهای نامعتبر، نه مقدارشون
                var invalidFields = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => x.Key);
                _logger.LogWarning("Register validation failed. ExamId={ExamId}, InvalidFields={Fields}",
                    form.ExamId, string.Join(",", invalidFields));
                return View(form);
            }

            var billNumber = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + Random.Shared.Next(100000, 999999);

            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["BillNumber"] = billNumber });

            var payment = new ExamPaymentRequest
            {
                ExamId = exam.Id,
                FullName = form.FullName,
                NationalCode = form.NationalCode,
                Mobile = form.Mobile,
                FatherName = form.FatherName,
                Email = form.Email,
                BillNumber = billNumber,
                Status = PaymentRequestStatus.Pending
            };
            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _paymentRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment request saved as Pending. PaymentId={PaymentId}, ExamId={ExamId}, Amount={Amount}",
                payment.Id, exam.Id, exam.Amount);

            try
            {
                var token = await _ipgService.GetTokenAsync();
                _logger.LogInformation("IPG API token received.");

                var cbiResult = await _ipgService.GetCbiAsync(new CbiRequest
                {
                    UnitCode = _settings.UnitCode,
                    SourceCode = _settings.SourceCode,
                    NationalCode = form.NationalCode,
                    Deposit = exam.Amount
                }, token);
                _logger.LogInformation("CBI received. Cbi={Cbi}", cbiResult.Cbi);

                var prepareResult = await _ipgService.PreparePaymentAsync(new IpgPreparePaymentRequest
                {
                    Psp = "SEP",
                    Nationalcode = form.NationalCode,
                    BillNumber = billNumber,
                    RedirectUrl = _settings.RedirectUrl,
                    Depositparams = new DepositParams
                    {
                        Amount = exam.Amount,
                        Ibans = new List<IbanItem>
                        {
                            new()
                            {
                                Iban = cbiResult.Sheba!,
                                Amount = exam.Amount,
                                PurchaseID = cbiResult.Cbi!
                            }
                        }
                    }
                }, token);

                payment.SepToken = prepareResult.Token;
                payment.IdPay = prepareResult.Pay_id;
                payment.PayUrl = prepareResult.Payurls;
                await _paymentRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Payment link created, redirecting user to gateway. IdPay={IdPay}", prepareResult.Pay_id);

                return Redirect(prepareResult.Payurls!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment initiation failed. PaymentId={PaymentId}", payment.Id);

                payment.Status = PaymentRequestStatus.Failed;
                await _paymentRepository.SaveChangesAsync(cancellationToken);

                ModelState.AddModelError("", "خطا در اتصال به درگاه پرداخت. لطفا دوباره تلاش کنید.");
                return View(form);
            }
        }


        [HttpPost]
        [Route("Payment/Callback")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Callback([FromForm] IpgCallbackModel model, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["BillNumber"] = model.BillNumber ?? "-" });
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation(
                "Callback received. Status={Status}, ResNum={ResNum}, ReferenceCode={ReferenceCode}, TraceNo={TraceNo}, Ip={Ip}",
                model.Status, model.Resnum, model.Referencecode, model.TraceNo, remoteIp);
            try
            {
                var payment = await _paymentRepository.Table
                    .FirstOrDefaultAsync(p => p.BillNumber == model.BillNumber, cancellationToken);
                if (payment is null)
                {
                    _logger.LogWarning("Callback for unknown BillNumber. Ip={Ip}", remoteIp);
                    return Ok();
                }
                if (payment.Status != PaymentRequestStatus.Pending)
                {
                    _logger.LogWarning(
                        "Callback ignored: payment already finalized. PaymentId={PaymentId}, CurrentStatus={CurrentStatus}, IncomingStatus={IncomingStatus}, Ip={Ip}",
                        payment.Id, payment.Status, model.Status, remoteIp);
                    return Ok(); 
                }

                payment.ResNum = model.Resnum;
                payment.ReferenceCode = model.Referencecode;
                payment.TraceNo = model.TraceNo;
                payment.VerifyDetail = model.VerifyDetail;
                payment.Status = model.Status == 1 ? PaymentRequestStatus.Success : PaymentRequestStatus.Failed;
                await _paymentRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Callback saved. PaymentId={PaymentId}, FinalStatus={FinalStatus}, Ip={Ip}",
                    payment.Id, payment.Status, remoteIp);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Callback processing failed. Status={Status}, ResNum={ResNum}, ReferenceCode={ReferenceCode}, TraceNo={TraceNo}",
                    model.Status, model.Resnum, model.Referencecode, model.TraceNo);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("Payment/Callback")]
        public async Task<IActionResult> CallbackResult(string billNumber, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(billNumber))
                return NotFound();

            var payment = await _paymentRepository.TableNoTracking
                .Include(p => p.Exam)
                .FirstOrDefaultAsync(p => p.BillNumber == billNumber, cancellationToken);

            if (payment is null)
            {
                _logger.LogWarning("Callback page requested for unknown BillNumber={BillNumber}", billNumber);
                return NotFound();
            }


            //call    ...


            var vm = new PaymentResultViewModel
            {
                ExamTitle = payment.Exam.Title,
                Amount = payment.Exam.Amount,
                ResNum = payment.ResNum,
                ReferenceCode = payment.ReferenceCode,
                Status = payment.Status,
                PdfUrl = !string.IsNullOrEmpty(payment.ResNum)
         ? $"{_settings.BaseUrl.TrimEnd('/')}/payverification/pdf/?resnum={payment.ResNum}"
         : null
            };

            return View("CallbackResult", vm);
        }
    }
}