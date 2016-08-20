using System.Linq;

namespace System.ComponentModel.DataAnnotations.Schema
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DataNodeAttribute : Attribute
    {
        public string[] DataNodes;

        public DataNodeAttribute(string dataNodes)
        {
            DataNodes = dataNodes.Split(',').Select(x => x.Trim()).ToArray();
        }
    }
}
