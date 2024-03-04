﻿using Ardalis.Result;
using MediatR;
using MongoDB.Driver;
using RiverBooks.EmailSending.Contracts;

namespace RiverBooks.EmailSending.Integrations;

internal interface IQueueEmailsInOutboxService
{
   
    Task QueueEmailForSending(EmailOutboxEntity entity);
}

internal class MongoDbQueueEmailOutboxService : IQueueEmailsInOutboxService
{
    private readonly IMongoCollection<EmailOutboxEntity> _emailCollection;

    public MongoDbQueueEmailOutboxService(IMongoCollection<EmailOutboxEntity> emailCollection)
    {
        _emailCollection = emailCollection;
    }

    public async Task QueueEmailForSending(EmailOutboxEntity entity)
    {
        await _emailCollection.InsertOneAsync(entity);
    }
}

/*
* And so for this to work, we're going to need some type of an entity that represents the email message that we're
* going to send. So there's going to be two phases to this. Basically, when this handler fires, instead of calling
* into an email sender, it's going to actually queue up the message for sending in what is going to be out outbox.
*/
internal class QueueEmailInOutboxSendEmailCommandHandler : IRequestHandler<SendEmailCommand, Result<Guid>>
{
    private readonly IQueueEmailsInOutboxService _outboxService;

    public QueueEmailInOutboxSendEmailCommandHandler(IQueueEmailsInOutboxService outboxService)
    {
        _outboxService = outboxService;
    }

    public async Task<Result<Guid>> Handle(SendEmailCommand request, CancellationToken ct)
    {
        var newEntity = new EmailOutboxEntity
        {
            Body = request.Body,
            Subject = request.Subject,
            To = request.To,
            From = request.From
        };

        await _outboxService.QueueEmailForSending(newEntity);

        return newEntity.Id;
    }
}
