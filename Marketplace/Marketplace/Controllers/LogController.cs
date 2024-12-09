using MarketplaceClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly MarketplaceContext _context;

        public LogController(MarketplaceContext context)
        {
            _context = context;
        }

        // GET: api/logs/get/N 
        [HttpGet("get/{N:int?}")]
        public ActionResult<IEnumerable<Log>> GetLogs(int N = 10)
        {
            try
            {
                var logs = _context.Logs
                .Take(N)
                .ToList();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/logs/count
        [HttpGet("count")]
        public ActionResult<int> GetLogCount()
        {
            try
            {
                var count = _context.Logs.Count();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/<LogController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LogController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LogController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LogController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LogController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
