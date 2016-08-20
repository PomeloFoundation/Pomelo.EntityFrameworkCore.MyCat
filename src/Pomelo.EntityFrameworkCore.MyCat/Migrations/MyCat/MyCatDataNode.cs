using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Migrations
{
    public class MyCatDataNode
    {
        public MyCatDatabaseHost Slave { get; set; }
        public MyCatDatabaseHost Master { get; set; }
    }
}
