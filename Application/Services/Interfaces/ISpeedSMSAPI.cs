using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ISpeedSMSAPI
    {
        public Task<String> getUserInfo();
        public Task sendSMS(string phones, string content, int type);
        public Task<String> sendMMS(String[] phones, String content, String link, String sender);
    }
}
