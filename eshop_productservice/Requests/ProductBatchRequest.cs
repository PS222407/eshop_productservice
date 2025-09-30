using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eshop_productservice.Requests;

public class ProductBatchRequest
{
    [Required]
    [DefaultValue(new[]
    {
        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "a3bb189e-8bf9-3888-9912-ace4e6543002"
    })]
    public List<Guid> Ids { get; set; }
}