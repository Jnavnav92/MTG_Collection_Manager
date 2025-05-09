
use [CollectionMgr]
GO

CREATE OR ALTER PROCEDURE dbo.proc_CreateAccount
@_iEmailAddress NVARCHAR(500),
@_iPWhash CHAR(60),
@_oQueryResult BIT OUTPUT, --0 error, 1 success
@_oQueryMessage NVARCHAR(4000) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (select 1 from Acct_Accounts WHERE EmailAddress = @_iEmailAddress)
	BEGIN
		SELECT @_oQueryResult = 0, @_oQueryMessage = 'Email Already Exists'
	END
	ELSE
	BEGIN
		INSERT INTO dbo.Acct_Accounts
		(AccountID, EmailAddress, PWHash)
		VALUES
		(NEWID(), @_iEmailAddress, @_iPWhash)

		SELECT @_oQueryResult = 1, @_oQueryMessage = 'Successfully Created Account!'
	END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[proc_Get_LoginAccountRecord]
@_iEmailAddress NVARCHAR(500)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (select 1 from Acct_Accounts WHERE EmailAddress = @_iEmailAddress)
	BEGIN
		select EmailAddress as 'EmailAddress', 
		PWHash as 'PasswordHash',
		'True' as 'QueryResult', 
		'Retrieved Login Record' as 'QueryMessage'
		from Acct_Accounts with (nolock)
		where EmailAddress = @_iEmailAddress

	END
	ELSE
	BEGIN
		select 'False' as 'QueryResult', 
		'No Login Record' as 'QueryMessage'
	END
END
GO
