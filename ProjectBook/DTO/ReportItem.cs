namespace ProjectBook.DTO
{
    public class ReportItem
    {
        public string Identifier {  get; set; }
        public float GrossSales {  get; set; }
        public float NetSales { get; set; } 
        public int OrdersCount { get; set; }  
        public int ProductsCount {  get; set; }

        public void addGrossSales(float amount)
        {
            this.GrossSales += amount;
        }

        public void addNetSales(float amount)
        {
            this.NetSales += amount;
        }

        public void increaseOrderCount()
        {
            this.OrdersCount++;
        }

    }
}
