using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore
{
    public class MyCatDatabaseHost
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public uint Port { get; set; } = 3306;
        public string Database { get; set; }
    }
}
