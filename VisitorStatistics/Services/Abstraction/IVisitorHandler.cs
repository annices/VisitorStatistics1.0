using System.Collections.Generic;
using VisitorStatistics.Models;

namespace VisitorStatistics.Services.Abstraction
{
    public interface IVisitorHandler
    {
        public bool IsRegisteredUrl();
        public List<string> GetAppUrls(out int appID);
        public void SaveVisitorStats(VisitorStats input);
        public void DeleteVisitors();
    }
}
