using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IpgSettings _settings;   // نه SiteSettings

        public PaymentController(
                 IExamRepository examRepository,
                 IExamPaymentRequestRepository paymentRepository,
                 IIpgService ipgService,
                 IOptions<SiteSettings> settings)
        {
            _examRepository = examRepository;
            _paymentRepository = paymentRepository;
            _ipgService = ipgService;
            _settings = settings.Value.IpgSettings;   // <-- این خط عوض بشه
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var exams = _examRepository.TableNoTracking
                .Where(e => e.IsActive)
                .Select(e => new ExamSelectViewModel { ExamId = e.Id, Title = e.Title, Amount = e.Amount })
                .ToList();
            return View(exams);
        }

        [HttpGet]
        public async Task<IActionResult> Register(int examId, CancellationToken cancellationToken)
        {
            var exam = await _examRepository.GetByIdAsync(cancellationToken, examId);
            if (exam is null) return NotFound();
            return View(new RegistrationFormViewModel { ExamId = examId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationFormViewModel form, CancellationToken cancellationToken)
        {
            var exam = await _examRepository.GetByIdAsync(cancellationToken, form.ExamId);
            if (exam is null) return NotFound();
            if (!ModelState.IsValid) return View(form);

            var billNumber = Guid.NewGuid().ToString("N")[..25];


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

            try
            {
                var token = await _ipgService.GetTokenAsync();

                var cbiResult = await _ipgService.GetCbiAsync(new CbiRequest
                {
                    UnitCode = _settings.UnitCode,
                    SourceCode = _settings.SourceCode,
                    NationalCode = form.NationalCode,
                    Deposit = exam.Amount
                }, token);

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
                                            Iban = cbiResult.Sheba!,        // <-- دیگه از cbiResult میاد
                                            Amount = exam.Amount,
                                            PurchaseID = cbiResult.Cbi!      // <-- دیگه از cbiResult میاد
                                        }
                                    }
                    }
                }, token);

                payment.SepToken = prepareResult.Token;
                payment.IdPay = prepareResult.Pay_id;
                payment.PayUrl = prepareResult.Payurls;
                await _paymentRepository.SaveChangesAsync(cancellationToken);

                return Redirect(prepareResult.Payurls!);
            }
            catch (Exception ex)
            {
                payment.Status = PaymentRequestStatus.Failed;
                await _paymentRepository.SaveChangesAsync(cancellationToken);
                ModelState.AddModelError("", "خطا در اتصال به درگاه پرداخت. لطفا دوباره تلاش کنید.");
                return View(form);
            }
        }

        [HttpPost]
        [Route("payment/callback")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Callback([FromForm] IpgCallbackModel model, CancellationToken cancellationToken)
        {
            var payment = _paymentRepository.TableNoTracking
                .FirstOrDefault(p => p.BillNumber == model.BillNumber);

            if (payment is not null)
            {
                payment.ResNum = model.Resnum;
                payment.ReferenceCode = model.Referencecode;
                payment.TraceNo = model.TraceNo;
                payment.VerifyDetail = model.VerifyDetail;
                payment.Status = model.Status == 1 ? PaymentRequestStatus.Success : PaymentRequestStatus.Failed;
                _paymentRepository.Update(payment);
                await _paymentRepository.SaveChangesAsync(cancellationToken);
            }

            return Ok();
        }
    }
}
