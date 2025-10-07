namespace eshop_productservice.Interfaces;

public interface IOrderListener
{
    public Task NewOrderCreated(string data);
}