namespace DesafioAPISimulacao.Model
{
    public class PaymentModel
    {
        public string monthlyPayment { get; set; }
        public string totalInterest { get; set; }
        public string totalPayment { get; set; }
        public List<PaymentSchedule> paymentSchedule { get; set; }

    }

    public class PaymentSchedule
    {
        public int month { get; set; }
        public string principal { get; set; }
        public string interest { get; set; }
        public string balance { get; set; }
    }
}
