using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public sealed class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages
                    .OrderByDescending(x => x.MessageSent)
                    .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.RecipientUserName == messageParams.UserName && !x.RecipientDeleted),
            "Outbox" => query.Where(x => x.SenderUserName == messageParams.UserName && !x.SenderDeleted),
            _ => query.Where(x => x.RecipientUserName == messageParams.UserName && !x.RecipientDeleted && x.DateRead == null)
        };

        var message = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.Create(
            message,
            messageParams.PageNumber,
            messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        var messages = await _context.Messages
             .Include(x => x.Sender)
                 .ThenInclude(x => x.Photos)
             .Include(x => x.Recipient)
                 .ThenInclude(x => x.Photos)
             .Where(
                 x => x.RecipientUserName == currentUserName && !x.RecipientDeleted
                     && x.SenderUserName == recipientUserName
                     ||
                    x.RecipientUserName == recipientUserName && !x.SenderDeleted
                     && x.SenderUserName == currentUserName)
             .OrderBy(x => x.MessageSent)
             .ToListAsync();

        var unreadMessage = messages
            .Where(x => x.DateRead == null && x.RecipientUserName == currentUserName)
            .ToList();

        if (unreadMessage.Any())
        {
            foreach (var message in unreadMessage)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAll()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}