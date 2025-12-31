using drinking_be.Enums;

namespace drinking_be.Domain.Orders
{
    public static class OrderRolePolicy
    {
        private static readonly Dictionary<string, List<(OrderStatusEnum from, OrderStatusEnum to)>> _rules
            = new()
            {
                // ================= ADMIN =================
                [AppRoles.Admin] = new()
            {
                (OrderStatusEnum.New, OrderStatusEnum.Confirmed),
                (OrderStatusEnum.Confirmed, OrderStatusEnum.Preparing),
                (OrderStatusEnum.Preparing, OrderStatusEnum.Ready),
                (OrderStatusEnum.Ready, OrderStatusEnum.Delivering),
                (OrderStatusEnum.Delivering, OrderStatusEnum.Completed),
                (OrderStatusEnum.New, OrderStatusEnum.Cancelled),
                (OrderStatusEnum.Confirmed, OrderStatusEnum.Cancelled)
            },

                // ================= MANAGER =================
                [AppRoles.Manager] = new()
            {
                (OrderStatusEnum.New, OrderStatusEnum.Confirmed),
                (OrderStatusEnum.Confirmed, OrderStatusEnum.Preparing),
                (OrderStatusEnum.Preparing, OrderStatusEnum.Ready),
                (OrderStatusEnum.New, OrderStatusEnum.Cancelled)
            },

                // ================= STAFF =================
                [AppRoles.Staff] = new()
            {
                (OrderStatusEnum.Confirmed, OrderStatusEnum.Preparing),
                (OrderStatusEnum.Preparing, OrderStatusEnum.Ready)
            },

                // ================= SHIPPER =================
                [AppRoles.Shipper] = new()
            {
                (OrderStatusEnum.Ready, OrderStatusEnum.Delivering),
                (OrderStatusEnum.Delivering, OrderStatusEnum.Completed)
            },

                // ================= CUSTOMER =================
                [AppRoles.Customer] = new()
            {
                (OrderStatusEnum.New, OrderStatusEnum.Cancelled)
            }
            };

        public static bool CanChangeStatus(
            string role,
            OrderStatusEnum current,
            OrderStatusEnum next)
        {
            if (!_rules.ContainsKey(role))
                return false;

            return _rules[role].Any(r => r.from == current && r.to == next);
        }
    }
}
