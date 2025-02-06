USE [Desafio_API]
GO
/****** Object:  Table [dbo].[PaymentFlowSummary]    Script Date: 06/02/2025 14:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentFlowSummary](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[monthlyPayment] [decimal](18, 2) NOT NULL,
	[totalInterest] [decimal](18, 2) NOT NULL,
	[totalPayment] [decimal](18, 2) NOT NULL,
	[IdProposal] [int] NOT NULL,
 CONSTRAINT [PK_PaymentFlowSummary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Proposal]    Script Date: 06/02/2025 14:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Proposal](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[loanAmmount] [decimal](18, 2) NOT NULL,
	[annualInterestRate] [decimal](18, 2) NOT NULL,
	[numberofMonths] [int] NOT NULL,
 CONSTRAINT [PK_Proposal] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PaymentFlowSummary]  WITH CHECK ADD FOREIGN KEY([IdProposal])
REFERENCES [dbo].[Proposal] ([Id])
GO
