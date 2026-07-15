
using ChatAppWithDeafStudents.API.Controllers.Messages;
using ChatAppWithDeafStudents.API.Functions.User;
using ChatAppWithDeafStudents.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppWithDeafStudents.API.Functions.Message
{
    /// <summary>
    /// Implements business logic for message processing, 
    /// including retrieval of latest messages and history, 
    /// as well as message persistence.
    /// </summary>
    public class MessageFunction : IMessageFunction
    {

        private readonly ApplicationDbContext _context;
        private readonly IUserFunction _userFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFunction"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="userFunction">The user service dependency.</param>
        public MessageFunction(ApplicationDbContext context, IUserFunction userFunction)
        {
            _context = context;
            _userFunction = userFunction;
        }

        /// <summary>
        /// Retrieves the most recent message from each chat the specified user is a member of.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation,
        /// containing a collection of <see cref="LastestMessages"/>.</returns>
        public async Task<IEnumerable<LastestMessages>> GetLastestMessages(Guid userId)
        {
            var userChats = await _context.ChatMembers
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => x.ChatId)
                .ToListAsync();

            var result = new List<LastestMessages>();

            foreach (var chatid in userChats)
            {
                var lastMessage = await _context.Messages
                    .AsNoTracking()
                    .Where(x => x.ChatId == chatid)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lastMessage != null)
                {
                    result.Add(new LastestMessages
                    {
                        Id = lastMessage.Id,
                        ChatId = chatid,
                        UserId = lastMessage.SenderId,
                        Content = lastMessage.Content,
                        SendDateTime = lastMessage.CreatedAt,
                        IsRead = lastMessage.IsRead
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves the complete message history for a specified chat.
        /// </summary>
        /// <param name="senderId">The identifier of the user requesting the history.</param>
        /// <param name="chatId">The unique identifier of the chat.</param>
        /// <returns>A task representing the asynchronous operation,
        /// containing a collection of <see cref="Messages"/>.</returns>
        public async Task<IEnumerable<Messages>> GetMessages(Guid senderId, Guid chatId)
        {

            var messages = await _context.Messages
                    .Where(x => x.ChatId == chatId)
                    .Include(x => x.Sender)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();

            return messages.Select(x => new Messages
            {
                Id = x.Id,
                ChatId = x.ChatId,
                SenderId = x.SenderId,
                Content = x.Content,
                IsVoiceToText = x.IsVoiceToText,
                CreatedAt = x.CreatedAt,
                IsRead = x.IsRead,
                Sender = x.Sender
            });
        }

        /// <summary>
        /// Retrieves paginated message history for a specified chat with efficient query execution.
        /// </summary>
        /// <param name="senderId">The identifier of the user requesting the history.</param>
        /// <param name="chatId">The unique identifier of the chat.</param>
        /// <param name="pageNumber">The page number for pagination (1-based). Default is 1.</param>
        /// <param name="pageSize">The number of messages per page. Default is 50. Maximum is 200.</param>
        /// <returns>A task representing the asynchronous operation,
        /// containing a <see cref="PaginatedResponse{Messages}"/>.</returns>
        public async Task<PaginatedResponse<Messages>> GetMessagesPaginated(Guid senderId, Guid chatId, int pageNumber = 1, int pageSize = 50)
        {
            // Validate pagination parameters
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 200) pageSize = 200; // Max page size limit

            var skip = (pageNumber - 1) * pageSize;

            // Get total count
            var totalCount = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ChatId == chatId)
                .CountAsync();

            // Get paginated messages (most recent last for natural chat order)
            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ChatId == chatId)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Reverse to display oldest first
            messages.Reverse();

            var result = new PaginatedResponse<Messages>
            {
                Items = messages.Select(x => new Messages
                {
                    Id = x.Id,
                    ChatId = x.ChatId,
                    SenderId = x.SenderId,
                    Content = x.Content,
                    IsVoiceToText = x.IsVoiceToText,
                    CreatedAt = x.CreatedAt,
                    IsRead = x.IsRead,
                    Sender = x.Sender
                }),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                StatusCode = 200,
                StatusMessage = "Success"
            };

            return result;
        }


        /// <summary>
        /// Creates and persists a new message to the database.
        /// Validates that the chat exists and that the sender is a member of the chat.
        /// </summary>
        /// <param name="senderId">The identifier of the user sending the message.</param>
        /// <param name="chatId">The identifier of the chat.</param>
        /// <param name="content">The text content of the message.</param>
        /// <param name="isVoiceToText">Indicates if the message was generated via voice-to-text.</param>
        /// <returns>A task representing the asynchronous operation, 
        /// containing the unique identifier of the saved message.</returns>
        /// <exception cref="InvalidOperationException">Thrown when chat does not exist or sender is not a member.</exception>
        public async Task<Guid> AddMessage(Guid senderId, Guid chatId, string content, bool isVoiceToText)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Message content cannot be empty.", nameof(content));
            }

            if (content.Length > 5000)
            {
                throw new ArgumentException("Message content exceeds maximum length of 5000 characters.", nameof(content));
            }

            // Validate chat existence
            var chatExists = await _context.Chats
                .AsNoTracking()
                .AnyAsync(c => c.Id == chatId);

            if (!chatExists)
            {
                throw new InvalidOperationException($"Chat with ID '{chatId}' does not exist.");
            }

            // Validate sender is member of chat
            var isMember = await _context.ChatMembers
                .AsNoTracking()
                .AnyAsync(cm => cm.ChatId == chatId && cm.UserId == senderId);

            if (!isMember)
            {
                throw new InvalidOperationException($"User '{senderId}' is not a member of chat '{chatId}'.");
            }

            var message = new Messages
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                IsVoiceToText = isVoiceToText,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return message.Id;
        }
    }
}