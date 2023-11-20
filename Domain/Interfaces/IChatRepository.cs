using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IChatRepository : IBaseRepository<Chat>
    {
        List<Chat> GetChatByReceiver(Guid id);
        List<Chat> GetChatBySenderAndReceiver(Guid guid, Guid receiverId);
    }
}
