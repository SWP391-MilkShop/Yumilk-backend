using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models.MessageModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.SignalR;

[Authorize(AuthenticationSchemes = "Access")]
public class MessageHub : Hub
{
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageService _messageService;

    public MessageHub(IMessageRepository messageRepository, ILogger logger, IUserRepository userRepository,
        IUnitOfWork unitOfWork, IMessageService messageService)
    {
        _messageRepository = messageRepository;
        _logger = logger;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _messageService = messageService;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.Information("User connected to message hub");
        var httpContext = Context.GetHttpContext();
        var currentUser = Guid.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value);
        var otherUser = Guid.Parse(httpContext.Request.Query["otherUser"].ToString());
        var isAdmin = await _userRepository.GetByIdAsync(otherUser);
        if (isAdmin.RoleId == (int)RoleId.CUSTOMER)
        {
            throw new HubException("Bạn chỉ có thể gửi tin nhắn cho nhân viên");
        }

        var groupName = GetGroupName(currentUser, otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        var messages = await _messageService.GetMessageThread(currentUser, otherUser);
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageModel createMessageModel)
    {
        var httpContext = Context.GetHttpContext();
        var senderId = Guid.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value);
        var senderExist = await _userRepository.GetByIdAsync(senderId);
        if (senderExist == null)
        {
            throw new HubException("Người gửi không tồn tại");
        }

        var recipientExist = await _userRepository.GetByIdAsync(createMessageModel.RecipientId);
        if (recipientExist == null)
        {
            throw new HubException("Người nhận không tồn tại");
        }

        if (senderId == createMessageModel.RecipientId)
        {
            throw new HubException("Bạn không thể gửi tin nhắn cho chính mình");
        }

        if (senderExist.RoleId == 3 && recipientExist.RoleId == 3)
        {
            throw new HubException("Bạn chỉ có thể gửi tin nhắn cho nhân viên");
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            SenderName = senderExist.FirstName + " " + senderExist.LastName,
            RecipientId = createMessageModel.RecipientId,
            RecipientName = recipientExist.FirstName + " " + recipientExist.LastName,
            Content = createMessageModel.Content,
        };
        _messageRepository.Add(message);
        var groupName = GetGroupName(senderId, createMessageModel.RecipientId);
        var group = await _messageRepository.GetMessageGroup(groupName);
        if (group!.Connections.Any(x => x.UserId == createMessageModel.RecipientId))
        {
            message.DateRead = DateTime.Now;
        }

        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            var resp = new MessageModel
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = message.SenderName,
                RecipientId = message.RecipientId,
                RecipientName = message.RecipientName,
                Content = message.Content,
                CreatedAt = message.CreatedAt
            };

            await Clients.Groups(groupName).SendAsync("NewMessage", resp);
        }

        throw new HubException("Đã xảy ra lỗi trong quá trình gửi tin nhắn");
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);
        var connection = new Connection
        {
            ConnectionId = Context.ConnectionId,
            UserId = Guid.Parse(Context.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value)
        };
        if (group == null)
        {
            group = new Group
            {
                Name = groupName,
            };
            _messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return group;
        }

        throw new HubException("Tham gia cuộc trò chuyện thất bại");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group!.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _messageRepository.RemoveConnection(connection!);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            return group;
        }

        throw new HubException("Rời khỏi cuộc trò chuyện thất bại");
    }

    private string GetGroupName(Guid currentUser, Guid otherUser)
    {
        var stringCompare = string.CompareOrdinal(currentUser.ToString(), otherUser.ToString()) < 0;
        return stringCompare ? $"{currentUser}-{otherUser}" : $"{otherUser}-{currentUser}";
    }
}