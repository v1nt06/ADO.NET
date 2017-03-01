using System.Collections.Generic;
using DTO;

namespace DAL
{
    public interface IOrderEngine
    {
        int CreateOrder(Order order);
        void DeleteOrder(int orderId);
        void EditOrder(int orderId, Order editedOrder);
        List<OrderDetailElement> GetCustomerOrderDetail(int orderId);
        List<OrderHistoryElement> GetCustomerOrderHistory(string customerId);
        int GetNextOrderId();
        Order GetOrder(int orderId);
        List<OrderDetailedInfo> GetOrderDetailedInformation(int orderId);
        IList<Order> GetOrders();
    }
}