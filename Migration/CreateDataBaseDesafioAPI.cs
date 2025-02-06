using FluentMigrator;
using FluentMigrator.Builders;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DesafioAPISimulacao.MigrateDataBase
{
    [Migration(1)]
    public class CreateDataBaseDesafioAPI: Migration
    {

       
      
            public override void Up()
            {

            Create.Table("Proposal")
                .WithColumn("Id").AsInt32().PrimaryKey().NotNullable().Identity()
                .WithColumn("loanAmmount").AsDecimal(18, 2).NotNullable()
                .WithColumn("annualInterestRate").AsDecimal(18, 2).NotNullable()
                    .WithColumn("numberofMonths").AsInt32().NotNullable();

            Create.Table("PaymentFlowSummary")
               .WithColumn("Id").AsInt32().PrimaryKey().NotNullable().Identity()
               .WithColumn("monthlyPayment").AsDecimal(18, 2).NotNullable()
               .WithColumn("totalInterest").AsDecimal(18, 2).NotNullable()
               .WithColumn("totalInterest").AsDecimal(18, 2).NotNullable()
               .WithColumn("totalPayment").AsDecimal(18, 2).NotNullable()
               .WithColumn("totalPayment").AsInt32().NotNullable().ForeignKey("Proposal", "Id");
        }

            public override void Down()
            {
                Delete.Table("Proposal");
                Delete.Table("PaymentFlowSummary");
        }
      

    }
}
