using Microsoft.EntityFrameworkCore;
using ProjectBook.Data;
using ProjectBook.DTO;
using ProjectBook.Models;
using System.Globalization;

namespace ProjectBook.Serivces
{
    public  class masterOrderReportService
    {
        private readonly ApiDbContext _dbContext = new ApiDbContext();
        public  List<OrderDetails> GetReportDataLast7Days()
        {
            DateTime endTime = new DateTime();

            int days = 5;

            DateTime startDate = endTime.AddDays(-(days - 1));


            List<ReportItem> listReportItems = new List<ReportItem>();

            return null;
        }
    }
}
