using drinking_be.Domain.Orders;
using drinking_be.Dtos.OrderPaymentDtos;
using drinking_be.Enums;
using drinking_be.Models;

namespace drinking_be.Interfaces.OrderInterfaces
{
    public interface IOrderPaymentService
    {
        Task<OrderPaymentReadDto> CreateChargeAsync(long orderId, int paymentMethodId);
        Task<bool> MarkPaymentSuccessAsync(long paymentId, string transactionCode);
        Task<bool> MarkPaymentFailedAsync(long paymentId, string reason);
        Task<OrderPaymentReadDto> RefundAsync(long orderId, decimal amount, string reason);
        Task<bool> RecalculateOrderPaymentStatusAsync(long orderId);
        Task<OrderPaymentSnapshot> BuildPaymentSnapshotAsync(long orderId);

    }

}
