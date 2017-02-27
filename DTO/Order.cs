using System;

namespace DTO
{
    public sealed class Order
    {
        private DateTime? orderDate;
        private DateTime? shippedDate;

        public int Id { get; }
        public string CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? OrderDate => orderDate;
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate => shippedDate;
        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }

        public OrderStatus Status { get; private set; }

        public Order()
        {
            Status = OrderStatus.New;
        }

        public Order(int id)
        {
            Id = id;
            Status = OrderStatus.New;
        }

        private void SetStatus()
        {
            if (OrderDate == null)
            {
                Status = OrderStatus.New;
            }
            else if (ShippedDate == null)
            {
                Status = OrderStatus.Processing;
            }
            else
            {
                Status = OrderStatus.Delivered;
            }
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

            var order = (Order)obj;

            var idsAreEqual = Id == 0 || order.Id == 0 || Id == order.Id;

            return idsAreEqual && CustomerId == order.CustomerId && EmployeeId == order.EmployeeId
                   && OrderDate == order.OrderDate && RequiredDate == order.RequiredDate
                   && ShippedDate == order.ShippedDate && ShipVia == order.ShipVia && Freight == order.Freight
                   && ShipName == order.ShipName && ShipAddress == order.ShipAddress
                   && ShipCity == order.ShipCity && ShipRegion == order.ShipRegion
                   && ShipPostalCode == order.ShipPostalCode && ShipCountry == order.ShipCountry
                   && Status == order.Status;
        }

        public void StartProcessing(DateTime date)
        {
            orderDate = date;
            SetStatus();
        }

        public void Deliver(DateTime date)
        {
            if (OrderDate == null)
            {
                throw new WrongOrderStatusException(OrderStatus.Processing);
            }

            if (date < OrderDate)
            {
                throw new ArgumentOutOfRangeException("date", date, "Shipped date less than order date");
            }

            shippedDate = date;
            SetStatus();
        }
    }
}
