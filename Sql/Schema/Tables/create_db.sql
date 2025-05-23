USE [master]
GO

USE [CollectionMgr]
GO

CREATE USER [CollectMgrUser] FOR LOGIN [CollectMgrUser] WITH DEFAULT_SCHEMA=[dbo]
GO

ALTER ROLE [db_owner] ADD MEMBER [CollectMgrUser]
GO

/****** Object:  Table [dbo].[Acct_Accounts]    Script Date: 5/23/2025 12:38:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Acct_Accounts](
	[AccountID] [uniqueidentifier] NOT NULL,
	[EmailAddress] [nvarchar](500) NOT NULL,
	[PWHash] [char](60) NOT NULL,
	[AccountVerified] [bit] NULL,
	[AuthorizationToken] [uniqueidentifier] NULL,
 CONSTRAINT [PK_tblAccounts] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_Email] UNIQUE NONCLUSTERED 
(
	[EmailAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Collect_Cards]    Script Date: 5/23/2025 12:38:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Collect_Cards](
	[CardID] [uniqueidentifier] NOT NULL,
	[CollectionID] [uniqueidentifier] NOT NULL,
	[ScryfallCardID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Collect_Cards] PRIMARY KEY CLUSTERED 
(
	[CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Collect_Collections]    Script Date: 5/23/2025 12:38:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Collect_Collections](
	[CollectionID] [uniqueidentifier] NOT NULL,
	[AccountID] [uniqueidentifier] NOT NULL,
	[CollectionName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Collect_Collections] PRIMARY KEY CLUSTERED 
(
	[CollectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Acct_Accounts] ADD  DEFAULT ((0)) FOR [AccountVerified]
GO
ALTER TABLE [dbo].[Collect_Cards]  WITH CHECK ADD  CONSTRAINT [FK_Cards_Collection_CollectionID] FOREIGN KEY([CollectionID])
REFERENCES [dbo].[Collect_Collections] ([CollectionID])
GO
ALTER TABLE [dbo].[Collect_Cards] CHECK CONSTRAINT [FK_Cards_Collection_CollectionID]
GO
ALTER TABLE [dbo].[Collect_Collections]  WITH CHECK ADD  CONSTRAINT [FK_Collections_Accounts_AccountID] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Acct_Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[Collect_Collections] CHECK CONSTRAINT [FK_Collections_Accounts_AccountID]
GO
USE [master]
GO
ALTER DATABASE [CollectionMgr] SET  READ_WRITE 
GO
