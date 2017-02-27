namespace DTO
{
    public sealed class OrderHistoryElement
    {
        public string ProductName { get; }
        public int ProductCount { get; }

        public OrderHistoryElement(string productName, int productCount)
        {
            ProductName = productName;
            ProductCount = productCount;
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

            var orderHistoryElement = (OrderHistoryElement)obj;

            return orderHistoryElement.ProductName == ProductName
                   && orderHistoryElement.ProductCount == ProductCount;
        }
    }
}
