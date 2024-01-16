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

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return (await _context.Connections.FindAsync(connectionId))!;
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await _context.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(y => y.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message?> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await _context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
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
        var query = _context.Messages
             .Where(
                 x => x.RecipientUserName == recipientUserName && !x.RecipientDeleted
                     && x.SenderUserName == currentUserName
                     ||
                    x.RecipientUserName == currentUserName && !x.SenderDeleted
                     && x.SenderUserName == recipientUserName)
             .OrderBy(x => x.MessageSent)
             .AsQueryable();

        var unreadMessage = query
            .Where(x => x.DateRead == null && x.RecipientUserName == currentUserName)
            .ToList();

        if (unreadMessage.Any())
        {
            foreach (var message in unreadMessage)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        var dtos = await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();

        return dtos;
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }
}