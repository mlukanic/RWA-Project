using Marketplace.Interfaces;
using MarketplaceClassLibrary.Models;

namespace Marketplace.Services
{
    public class LogService : ILogService
    {
        private readonly MarketplaceContext _context;

        public LogService(MarketplaceContext context)
        {
            _context = context;
        }

        public void Log(string level, string message)
        {
            var log = new Log
            {
                Level = level,
                Message = message,
                Timestamp = DateTime.Now.ToLocalTime()
            };

            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}
