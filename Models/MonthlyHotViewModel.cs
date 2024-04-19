using YYBagProgram.Models;

namespace YYBagProgram.Models
{
    public class MonthlyHotViewModel
    {
        public MonthlyHot MonthlyHot { get; set; }

        public IList<MonthlyHot> listMonthlyHot { get; set; }

        public IList<Product> listProduct { get; set; }

        public string? strOriImageUrl { get; set; }

    }
}
