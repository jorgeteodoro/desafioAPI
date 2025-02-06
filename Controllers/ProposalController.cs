using DesafioAPISimulacao.API.Contract;
using DesafioAPISimulacao.Application;
using DesafioAPISimulacao.Application.Interfaces;
using DesafioAPISimulacao.Core.Models;
using DesafioAPISimulacao.Domain.Entities;
using DesafioAPISimulacao.Model;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace DesafioAPISimulacao.API.Controllers
{
    [ApiController]
    public class ProposalController : ServiceBaseController<ProposalEntity>
    {
        private readonly ProposalService _proposalService;
        private readonly PaymentFlowSummaryService _paymentFlowSummaryService;

        public ProposalController(IServiceBase<ProposalEntity> serviceBase,
                                  IServiceBase<PaymentFlowSummaryEntity> serviceBase1) : base(serviceBase)
        {
            _proposalService = (ProposalService)serviceBase;
            _paymentFlowSummaryService = (PaymentFlowSummaryService)serviceBase1;
        }

        [NonAction]
        public ObjectResult SetError(Exception e)
        {
            return StatusCode(500, e.Message);
        }

       
        [Route("/api/loans/simulate")]
        [HttpPost]
        public override async Task<ResultRequest> Insert([FromBody] ProposalEntity proposalEntity)
        {
            try
            {
                //applicar validações.
                int idProposal = await _proposalService.Insert(proposalEntity);
                //if (idProposal > 0)
                //{
                //double calculate = (proposalEntity.LoanAmmount * (proposalEntity.AnnualInterestRate / 12) * 
                //                    Math.Pow((proposalEntity.AnnualInterestRate / 12) + 1, (proposalEntity.NumberofMonths)) / 
                //                    (Math.Pow((proposalEntity.AnnualInterestRate / 12) + 1, proposalEntity.NumberofMonths)) - 1);


                // }
                PaymentModel paymentModel = calculateInterest(proposalEntity);

                int savedId = await _paymentFlowSummaryService.Insert(new PaymentFlowSummaryEntity
                {
                    IdProposal = idProposal,
                    TotalInterest = paymentModel.totalInterest,
                    MonthlyPayment = paymentModel.monthlyPayment,
                    TotalPayment = paymentModel.totalPayment
                });

                if (savedId > 0)
                    return new ResultRequest(true, paymentModel);


                return new ResultRequest(false, paymentModel);

            }
            catch (Exception ex)
            {
                return new ResultRequest(false, ex);
            }
        }
        [NonAction]
        private PaymentModel calculateInterest(ProposalEntity proposalEntity)
        {


            List<PaymentSchedule> paymentSchedules = new List<PaymentSchedule>();

            double interest = proposalEntity.AnnualInterestRate;
            int numberofMonths = proposalEntity.NumberofMonths;
            double loanAmmount = proposalEntity.LoanAmmount;

            double monthlyPayment = (loanAmmount * Math.Pow((interest / 12) + 1, (numberofMonths)) * interest / 12)
                                / (Math.Pow(interest / 12 + 1, (numberofMonths)) - 1);
            double totalPayment = monthlyPayment * numberofMonths;
            double totalInterest = totalPayment - loanAmmount;

            return new PaymentModel
            {
                monthlyPayment = monthlyPayment,
                totalInterest = totalInterest,
                totalPayment = totalPayment,
                paymentSchedule = calculateSchedule(numberofMonths, interest, monthlyPayment, totalPayment, totalInterest, proposalEntity.LoanAmmount)
            };
        }
        [NonAction]
        private List<PaymentSchedule> calculateSchedule(int numberofMonths, double interest, double monthlyPayment, double totalPayment, double totalInterest, double loanAmmount)
        {
            double totalPaymentMonthly = 0;

            List<PaymentSchedule> paymentSchedules = new List<PaymentSchedule>();
            double balance = loanAmmount;
            for (int parcela = 1; parcela <= numberofMonths; parcela++)
            {
                totalPaymentMonthly = loanAmmount * (Math.Pow((interest / 12) + 1, (numberofMonths)) * interest / 12)
                              / (Math.Pow((interest / 12 + 1), (numberofMonths)) - 1);

                balance = balance - (totalPaymentMonthly - (loanAmmount * (interest / 12)));

                paymentSchedules.Add(new PaymentSchedule()
                {
                    interest = (loanAmmount * (interest / 12)),
                    month = parcela,
                    balance =balance,
                    principal = (totalPaymentMonthly - (loanAmmount * (interest / 12)))
                });
            }

         



            return paymentSchedules;
        }


    }
}
