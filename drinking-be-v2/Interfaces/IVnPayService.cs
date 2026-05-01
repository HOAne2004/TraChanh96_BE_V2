using drinking_be.Models;

namespace drinking_be.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(Order order, HttpContext context);
        bool ValidateSignature(IQueryCollection collections);
    }
}
