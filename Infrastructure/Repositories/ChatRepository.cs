using Domain.DataModels;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        private readonly GoShareContext _context;

        public ChatRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public List<Chat> GetChatByReceiver(Guid id)
        {
            return _context.Chats.Where(c => c.Receiver.CompareTo(id) == 0).ToList();
        }

        public List<Chat> GetChatBySenderAndReceiver(Guid senderId, Guid receiverId)
        {
            return _context.Chats.Where(c => (c.Sender.CompareTo(senderId) == 0) || (c.Sender.CompareTo(receiverId) == 0)).ToList();
        }
    }
}
