namespace ProjectBook.DTO
{
    public class ReportItem
    {
        private int quantity;

        public string Identifier {  get; set; }
        public float GrossSales {  get; set; }
        public float NetSales { get; set; } 
        public int OrdersCount { get; set; }  
        public int ProductsCount {  get; set; }

        public ReportItem() { }

        public ReportItem(string identifier, float grossSales, float netSales, int productsCount)
        {
            this.Identifier = identifier;
            this.GrossSales = grossSales;
            this.NetSales = netSales;
            this.ProductsCount = productsCount;
        }

        public ReportItem(string identifier)
        {
            this.Identifier = identifier;
        }

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

        public void increaseProductsCount(int productsCount)
        {
            this.ProductsCount += productsCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null || GetType() != obj.GetType()) return false;

            ReportItem that = (ReportItem)obj;
            return Identifier == that.Identifier;
        }

        public override int GetHashCode()
        {
            return Identifier != null ? Identifier.GetHashCode() : 0;
        }

    }
}
