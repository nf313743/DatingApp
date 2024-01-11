using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public sealed class MessagesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var userName = User.GetUserName();

        if (userName.Equals(createMessageDto.RecipientUserName, StringComparison.OrdinalIgnoreCase))
            return BadRequest("You cannot send messages to yourself");

        var sender = (await _userRepository.GetUserByUserName(userName))!;
        var recipient = await _userRepository.GetUserByUserName(createMessageDto.RecipientUserName);

        if (recipient is null)
            return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUserName = sender.UserName!,
            RecipientUserName = recipient.UserName!,
            Content = createMessageDto.Content
        };

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAll())
            return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.UserName = User.GetUserName();

        var messages = await _messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUserName = User.GetUserName();

        return Ok(await _messageRepository.GetMessageThread(currentUserName, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var userName = User.GetUserName();

        var message = await _messageRepository.GetMessage(id);

        if (message is null)
            return NotFound();

        if (message.SenderUserName != userName && message.RecipientUserName != userName)
            return Unauthorized();

        if (message.SenderUserName == userName)
            message.SenderDeleted = true;

        if (message.RecipientUserName == userName)
            message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted)
            _messageRepository.DeleteMessage(message);

        if (await _messageRepository.SaveAll())
            return Ok();

        return BadRequest();
    }
}