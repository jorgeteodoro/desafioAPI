namespace DesafioAPISimulacao.Model
{
    public class PaymentModel
    {
        public double monthlyPayment { get; set; }
        public double totalInterest { get; set; }
        public double totalPayment { get; set; }
        public List<PaymentSchedule> paymentSchedule { get; set; }

    }

    public class PaymentSchedule
    {
        public int month { get; set; }
        public double principal { get; set; }
        public double interest { get; set; }
        public double balance { get; set; }
    }
}
