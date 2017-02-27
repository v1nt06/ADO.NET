using DTO;
using System.Collections.Generic;

namespace DAL
{
    interface IOrderEngine
    {
        IList<Order> GetOrders();
    }
}
