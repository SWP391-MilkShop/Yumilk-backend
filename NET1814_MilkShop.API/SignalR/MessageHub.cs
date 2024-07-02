using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models.MessageModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.SignalR;

[Authorize(AuthenticationSchemes = "Access")]
public class MessageHub : Hub
{
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MessageHub(IMessageRepository messageRepository, ILogger logger, IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _logger = logger;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.Information("User connected to message hub");
        var httpContext = Context.GetHttpContext();
        var currentUser = Guid.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value);
        var otherUser = Guid.Parse(httpContext.Request.Query["otherUser"].ToString());
        var groupName = GetGroupName(currentUser, otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var messages = await _messageRepository.GetMessageThread(currentUser, otherUser);
        await Clients.Groups(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageModel createMessageModel)
    {
        var httpContext = Context.GetHttpContext();
        var senderId = (httpContext!.Items["UserId"] as Guid?)!.Value;
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
            var group = GetGroupName(senderId, createMessageModel.RecipientId);
            await Clients.Groups(group).SendAsync("NewMessage", resp);
        }
    }

    private string GetGroupName(Guid currentUser, Guid otherUser)
    {
        var stringCompare = string.CompareOrdinal(currentUser.ToString(), otherUser.ToString()) < 0;
        return stringCompare ? $"{currentUser}-{otherUser}" : $"{otherUser}-{currentUser}";
    }
}