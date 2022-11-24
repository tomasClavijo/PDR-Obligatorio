using System;
using System.Collections.Generic;

namespace LogsServer.IService
{
    public interface IMQService
    {
        public List<String> GetMessages(string username, DateTime date, string word);
    }
}
