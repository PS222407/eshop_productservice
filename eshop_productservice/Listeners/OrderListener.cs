using eshop_productservice.Interfaces;
using Newtonsoft.Json;

namespace eshop_productservice.Listeners;

public class OrderListener(IProductService productService) : IOrderListener
{
    public async Task NewOrderCreated(string data)
    {
        try
        {
            var order = JsonConvert.DeserializeObject<dynamic>(data);
            if (order == null)
            {
                Console.WriteLine("Error parsing NewOrderCreated message");
                return;
            }

            foreach (var product in order.Products)
                await productService.DecreaseStockBy((string)product.Id, (int)product.Quantity);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error parsing NewOrderCreated message");
        }
    }
}