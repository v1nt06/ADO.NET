namespace DTO
{
    public sealed class OrderDetailElement
    {
        public string ProductName { get; }
        public decimal UnitPrice { get; }
        public short Quantity { get; }
        public int Discount { get; }
        public decimal ExtendedPrice { get; }

        public OrderDetailElement(string productName, decimal unitPrice, short quantity,
            int discount, decimal extendedPrice)
        {
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            Discount = discount;
            ExtendedPrice = extendedPrice;
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

            var orderDetailElement = (OrderDetailElement)obj;
            return orderDetailElement.ProductName == ProductName
                   && orderDetailElement.Discount == Discount
                   && orderDetailElement.ExtendedPrice == ExtendedPrice
                   && orderDetailElement.Quantity == Quantity
                   && orderDetailElement.UnitPrice == UnitPrice;
        }
    }
}
