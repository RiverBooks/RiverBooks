namespace RiverBooks.EmailSending;

internal interface ISendEmailsFromOutboxService
{
    Task CheckForAndSendEmailsAsync();
}
