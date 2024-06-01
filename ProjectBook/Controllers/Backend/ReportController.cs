using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    return Ok(getReportDataBy7Days());
                case "last_28_days":
                    return Ok(getReportDataBy28Days());
                case "last_12_months":
                    return Ok(getReportDataBy12months());
            }
            return NotFound();  
        }

        private List<ReportItem> getReportDataBy7Days()
        {
            DateTime currentDate = DateTime.Now;
            DateTime startTime = currentDate.AddDays(-6);
            // Order 7 
            List<Order> orders = _dbContext.Orders.Include(o => o.Details).Where(o => o.Oder_time > startTime && o.Oder_time <= currentDate).ToList();
            // Date 7
            List<ReportItem> listReportItems = createReportData(startTime, currentDate, "day");


            calculateSalesForReportData(orders, listReportItems, "day");

           
            return listReportItems;
        }

        private List<ReportItem> getReportDataBy28Days()
        {
            DateTime currentDate = DateTime.Now;
            DateTime startTime = currentDate.AddDays(-29);
            // Order 7 
            List<Order> orders = _dbContext.Orders.Include(o => o.Details).Where(o => o.Oder_time > startTime && o.Oder_time <= currentDate).ToList();
            // Date 7
            List<ReportItem> listReportItems = createReportData(startTime, currentDate, "day");


            calculateSalesForReportData(orders, listReportItems, "day");


            return listReportItems;
        }   

        private List<ReportItem> getReportDataBy12months()
        {
            DateTime currentDate = DateTime.Now;
            DateTime startTime = currentDate.AddYears(-1);
            // Order 7 
            List<Order> orders = _dbContext.Orders.Include(o => o.Details).Where(o => o.Oder_time > startTime && o.Oder_time <= currentDate).ToList();
            // Date 7
            List<ReportItem> listReportItems = createReportData(startTime, currentDate,"month");


            calculateSalesForReportData(orders, listReportItems, "month");


            return listReportItems;
        }

        private List<ReportItem> createReportData (DateTime startTime, DateTime endTime,string type)
        {
            List<ReportItem> reportItems = new List<ReportItem> ();
            do
            {
                string formatDate = startTime.ToString("M/d/yyyy");

                if (type == "month")
                {
                    formatDate = startTime.ToString("MM/yyyy");
                    reportItems.Add(new ReportItem(formatDate));
                    startTime = startTime.AddMonths(1);
                } else
                {
                    
                    reportItems.Add(new ReportItem(formatDate));
                    startTime = startTime.AddDays(1);
                }

            } while (startTime <= endTime);
            return reportItems;
        }

        private void calculateSalesForReportData(List<Order> orders,List<ReportItem> reports, string type)
        {
            foreach (var item in orders)
            {
                string formatDate = item.Oder_time.ToString("M/d/yyyy");
                if (type == "month")
                {
                    formatDate = item.Oder_time.ToString("MM/yyyy");
                }
                ReportItem reportItem = new ReportItem(formatDate);

                foreach (var detail in item.Details)
                {
                    float grossSales = detail.Total;
                    float netSales = detail.Total - detail.Product_cost;

                    int itemIndex = reports.IndexOf(reportItem);

                    if (itemIndex >= 0)
                    {
                        reportItem = reports[itemIndex];
                        reportItem.addGrossSales(grossSales);
                        reportItem.addNetSales(netSales);
                        reportItem.increaseProductsCount(detail.Quantity);
                    }
                }
            }
        }
    }
}
