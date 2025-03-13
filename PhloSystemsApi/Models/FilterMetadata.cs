namespace PhloSystemsApi.Models
{
    public class ProductResponse{
        public List<Product> Products { get; set; }
        public FilterMetadata filterMetaData { get; set; }
    }

    public class FilterMetadata
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string[] Sizes { get; set; }
        public string[] MostCommonWords { get; set; }
    }
}
