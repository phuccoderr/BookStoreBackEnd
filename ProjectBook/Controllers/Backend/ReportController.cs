using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectBook.Data;
using ProjectBook.DTO;
using ProjectBook.Models;
using ProjectBook.Serivces;

namespace ProjectBook.Controllers.Backend
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController(ApiDbContext dbContext) : ControllerBase
    {
        private readonly ApiDbContext _dbContext = dbContext;

        [HttpGet("sales_by_date/{period}")]
        public IActionResult getReportDataByDatePeriod(string period)
        {
            switch (period)
            {
                case "last_7_days":
                    return Ok();
            }
            return NotFound();  
        }

        private List<OrderDetails> getReportDataBy7Days()
        {
            DateTime dateTime = DateTime.Now;
            return null;
        }
    }
}
