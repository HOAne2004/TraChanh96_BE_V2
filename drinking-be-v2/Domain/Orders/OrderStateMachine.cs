using drinking_be.Domain.Orders;
using drinking_be.Enums;
using static drinking_be.Services.OrderService;

namespace drinking_be.Domain.Orders
{
    public static class OrderStateMachine
    {
        // ===============================
        // ENTRY POINT – GỌI DUY NHẤT HÀM NÀY
        // ===============================
        public static void ValidateTransition(
            OrderStatusEnum from,
            OrderStatusEnum to,
            PaymentFlowEnum paymentFlow,
            OrderPaymentSnapshot payment,
            UserRoleEnum actor,
            decimal orderGrandTotal)
        {
            ValidateNotFinalState(from);
            ValidateBasicFlow(from, to);
            ValidateRole(from, to, MapActorRole(actor));
            ValidatePayment(from, to, paymentFlow, payment, orderGrandTotal);
        }

        // ===============================
        // FINAL STATE CHECK
        // ===============================
        private static void ValidateNotFinalState(OrderStatusEnum from)
        {
            if (from == OrderStatusEnum.Cancelled ||
                from == OrderStatusEnum.Completed ||
                from == OrderStatusEnum.Received)
            {
                throw new AppException("Đơn hàng đã kết thúc, không thể thay đổi trạng thái.");
            }
        }

        // ===============================
        // BASIC FLOW (STATE GRAPH)
        // ===============================
        private static void ValidateBasicFlow(
            OrderStatusEnum from,
            OrderStatusEnum to)
        {
            var allowed = (from, to) switch
            {
                (OrderStatusEnum.PendingPayment, OrderStatusEnum.New) => true,

                (OrderStatusEnum.New, OrderStatusEnum.Confirmed) => true,
                (OrderStatusEnum.Confirmed, OrderStatusEnum.Preparing) => true,
                (OrderStatusEnum.Preparing, OrderStatusEnum.Ready) => true,

                (OrderStatusEnum.Ready, OrderStatusEnum.Delivering) => true,
                (OrderStatusEnum.Delivering, OrderStatusEnum.Completed) => true,

                (OrderStatusEnum.Ready, OrderStatusEnum.Received) => true,

                (OrderStatusEnum.New, OrderStatusEnum.Cancelled) => true,
                (OrderStatusEnum.Confirmed, OrderStatusEnum.Cancelled) => true,

                _ => false
            };

            if (!allowed)
                throw new AppException($"Không thể chuyển trạng thái từ {from} sang {to}.");
        }

        // ===============================
        // ROLE RULES
        // ===============================
        private static void ValidateRole(
            OrderStatusEnum from,
            OrderStatusEnum to,
            OrderActorRole actor)
        {
            switch (actor)
            {
                case OrderActorRole.Customer:
                    if (to != OrderStatusEnum.Cancelled)
                        throw new AppException("Khách hàng chỉ được phép hủy đơn.");
                    break;

                case OrderActorRole.Shipper:
                    if (to != OrderStatusEnum.Delivering &&
                        to != OrderStatusEnum.Completed)
                        throw new AppException("Shipper không được thực hiện hành động này.");
                    break;

                case OrderActorRole.Staff:
                case OrderActorRole.Admin:
                case OrderActorRole.System:
                    // Full quyền theo flow
                    break;

                default:
                    throw new AppException("Actor không hợp lệ.");
            }
        }

        // ===============================
        // PAYMENT RULES (CORE BUSINESS)
        // ===============================
        private static void ValidatePayment(
        OrderStatusEnum from,
        OrderStatusEnum to,
        PaymentFlowEnum paymentFlow,
        OrderPaymentSnapshot payment,
        decimal grandTotal)
        {
            if (paymentFlow == PaymentFlowEnum.PayBefore)
            {
                if (to == OrderStatusEnum.Confirmed && !payment.IsFullyPaid(grandTotal))
                    throw new AppException("Đơn hàng chưa được thanh toán.");

                if (to == OrderStatusEnum.Completed && !payment.IsFullyPaid(grandTotal))
                    throw new AppException("Không thể hoàn thành khi chưa thanh toán đủ.");
            }
        }

        private static OrderActorRole MapActorRole(UserRoleEnum role)
        {
            return role switch
            {
                UserRoleEnum.Customer => OrderActorRole.Customer,
                UserRoleEnum.Staff => OrderActorRole.Staff,
                UserRoleEnum.Manager => OrderActorRole.Admin,
                UserRoleEnum.Admin => OrderActorRole.Admin,
                _ => OrderActorRole.System
            };
        }

    }
}
