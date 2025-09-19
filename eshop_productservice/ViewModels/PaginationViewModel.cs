namespace eshop_productservice.ViewModels;

public class PaginationViewModel<T>
{
    public int found { get; set; }

    public int page { get; set; }

    public List<T> hits { get; set; }

    public RequestParams request_params { get; set; }
}