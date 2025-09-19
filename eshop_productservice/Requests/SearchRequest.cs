namespace eshop_productservice.Requests;

public class SearchRequest
{
    public string q { get; set; }
    public string? filter_by { get; set; } // {"categories": ["019961f6-1634-796f-971a-6caa57cabf2a"]}
    public string? sort_by { get; set; }
    public int page { get; set; } = 1;
    public int per_page { get; set; } = 12;
}