using System.ComponentModel;

namespace eshop_productservice.Requests;

public class SearchRequest
{
    [DefaultValue("brush")]
    public string q { get; set; }

    [DefaultValue("{\"categories\": [\"019961f6-1634-796f-971a-6caa57cabf2a\"]}")]
    public string? filter_by { get; set; }

    public string? sort_by { get; set; }

    [DefaultValue(1)] public int page { get; set; } = 1;
    [DefaultValue(12)] public int per_page { get; set; } = 12;
}