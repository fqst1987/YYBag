using YYBagProgram.Models;

namespace YYBagProgram.Models
{
    public class MonthlyHotViewModel
    {
        public MonthlyHot? monthlyHot { get; set; }

        public IList<Product>? product { get; set; }

        public string? OriImages { get; set; }
    }
}
