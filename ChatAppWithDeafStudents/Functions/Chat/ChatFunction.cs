using ChatAppWithDeafStudents.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace ChatAppWithDeafStudents.API.Functions.Chat
{
    /// <summary>
    /// Provides business logic operations related 
    /// to chats, members, and message retrieval.
    /// </summary>
    public class ChatFunction : IChatFunction
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChatFunction> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatFunction"/> class.
        /// </summary>
        /// <param name="context">The database context instance.</param>
        /// <param name="logger">The logger instance.</param>
        public ChatFunction(ApplicationDbContext context, ILogger<ChatFunction> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of chat memberships for a specific 
        /// user, including associated chat details.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A collection of anonymous objects containing membership and chat information.</returns>
        public async Task<IEnumerable<dynamic>> GetUserChatMembersAsync(Guid userId)
        {
            return await _context.ChatMembers
                .Where(cm => cm.UserId == userId)
                .Include(cm => cm.Chat)
                .Select(cm => new
                {
                    cm.ChatId,
                    cm.UserId,
                    cm.JoinedAt,
                    Chat = new
                    {
                        cm.Chat.Id,
                        cm.Chat.Title,
                        cm.Chat.IsGroup,
                        cm.Chat.CreatedAt
                    }
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all chat entities associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A collection of <see cref="Chats"/> objects.</returns>
        public async Task<IEnumerable<Chats>> GetUserChatsAsync(Guid userId)
        {

            return await _context.ChatMembers
                .Where(cm => cm.UserId == userId)
                .Include(cm => cm.Chat) 
                .Select(cm => new Chats
                {
                    Id = cm.Chat.Id,
                    Title = cm.Chat.Title,
                    IsGroup = cm.Chat.IsGroup,
                    CreatedAt = cm.Chat.CreatedAt
                })
                .ToListAsync();
        }

        /// <summary>
        /// Fetches user information based on the provided user identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The <see cref="Users"/> entity if found; otherwise, a 
        /// new empty user instance with the provided ID.</returns>
        public async Task<Users> GetUserInfoAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId) ?? new Users { Id = userId };
        }

        /// <summary>
        /// Retrieves the most recent message from every chat the user is a member of.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A collection of anonymous objects 
        /// containing the latest message details per chat.</returns>
        public async Task<IEnumerable<dynamic>> GetLastestMessagesAsync(Guid userId)
        {
            // Get all chats for the user
            var userChats = await _context.ChatMembers
                .AsNoTracking()
                .Where(cm => cm.UserId == userId)
                .Select(cm => cm.ChatId)
                .ToListAsync();

            _logger.LogInformation($"GetLastestMessagesAsync: Found {userChats.Count} chats for user {userId}");

            // For each chat, get the latest message
            var latestMessages = new List<dynamic>();

            foreach (var chatId in userChats)
            {
                var latestMessage = await _context.Messages
                    .AsNoTracking()
                    .Where(m => m.ChatId == chatId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => new
                    {
                        m.Id,
                        UserId = m.SenderId,
                        m.ChatId,
                        m.Content,
                        SendDateTime = m.CreatedAt,
                        m.IsRead
                    })
                    .FirstOrDefaultAsync();

                if (latestMessage != null)
                {
                    _logger.LogInformation($"Chat {chatId}: Latest message: ID={latestMessage.Id}, Content='{latestMessage.Content}', CreatedAt={latestMessage.SendDateTime}");
                    latestMessages.Add(latestMessage);
                }
                else
                {
                    _logger.LogInformation($"Chat {chatId}: No messages");
                }
            }

            _logger.LogInformation($"Returning {latestMessages.Count} latest messages from DB (not cached)");
            return latestMessages;
        }
    }

}

