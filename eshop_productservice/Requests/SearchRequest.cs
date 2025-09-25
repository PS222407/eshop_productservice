using System.ComponentModel;

namespace eshop_productservice.Requests;

public class SearchRequest
{
    [DefaultValue("brush")]
    public string q { get; set; }

    [DefaultValue("{\"categories\": [\"0199708c-4eb6-71d1-9e48-83102c0155a4\"]}")]
    public string? filter_by { get; set; }

    public string? sort_by { get; set; }

    [DefaultValue(1)] public int page { get; set; } = 1;
    [DefaultValue(12)] public int per_page { get; set; } = 12;
}