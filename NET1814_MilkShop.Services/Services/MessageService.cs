using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.MessageModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;

namespace NET1814_MilkShop.Services.Services;

public interface IMessageService
{
    // Task<ResponseModel> GetMessagesForUser(Guid userId);
    // Task<ResponseModel> GetMessageThread(Guid currentUser, Guid recipientId);
    Task<ResponseModel> CreateMessage(Guid senderId, CreateMessageModel createMessageModel);
}

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    // public async Task<ResponseModel> GetMessagesForUser(Guid userId)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async Task<ResponseModel> GetMessageThread(Guid userId, Guid recipientId)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<ResponseModel> CreateMessage(Guid senderId, CreateMessageModel createMessageModel)
    {
        var senderExist = await _userRepository.GetByIdAsync(senderId);
        if (senderExist == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Người gửi"));
        }

        var recipientExist = await _userRepository.GetByIdAsync(createMessageModel.RecipientId);
        if (recipientExist == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Người nhận"));
        }

        if (senderId == createMessageModel.RecipientId)
        {
            return ResponseModel.BadRequest("Bạn không thể gửi tin nhắn cho chính mình");
        }

        if (senderExist.RoleId == 3 && recipientExist.RoleId == 3)
        {
            return ResponseModel.BadRequest("Bạn chỉ có thể gửi tin nhắn cho nhân viên");
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
            return ResponseModel.Success(ResponseConstants.Create("tin nhắn", true), resp);
        }

        return ResponseModel.Error(ResponseConstants.Create("tin nhắn", false));
    }
}