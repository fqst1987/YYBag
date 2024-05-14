using YYBagProgram.Models.CartFolder;
namespace YYBagProgram.Models.ViewModel;

public class CartViewModel
{
    public Cart MemberCart { get; set; }

    public Members Member { get; set; }

    public IList<ProductsColorDetail> ProductsColorDetails { get; set; }
}
