namespace DTO
{
    public sealed class OrderDetailedInfo
    {
        public int OrderId { get; }
        public string CustomerId { get; }
        public int ProductId { get; }
        public string ProductName { get; }
        public decimal UnitPrice { get; }
        public short Quantity { get; }
        public float Discount { get; }

        public OrderDetailedInfo(int orderId, string customerId, int productId,
            string productName, decimal unitPrice, short quantity, float discount)
        {
            OrderId = orderId;
            CustomerId = customerId;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            Discount = discount;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(obj, this))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var orderInfo = (OrderDetailedInfo) obj;

            return OrderId == orderInfo.OrderId && CustomerId == orderInfo.CustomerId
                   && ProductId == orderInfo.ProductId && ProductName == orderInfo.ProductName
                   && UnitPrice == orderInfo.UnitPrice && Quantity == orderInfo.Quantity
                   && Discount == orderInfo.Discount;
        }
    }
}
