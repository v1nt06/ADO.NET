using System;
using System.Collections.Generic;
using System.Data;
using DTO;
using DTO.Enums;

namespace DAL
{
    public class OrderEngine : IOrderEngine
    {
        private readonly string connectionString;
        private readonly string provider;

        public OrderEngine(string connectionString, string provider)
        {
            this.connectionString = connectionString;
            this.provider = provider;
        }

        public IList<Order> GetOrders()
        {
            var orders = new List<Order>();
            using (var connection = DataFactory.CreateConnection(connectionString, provider))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, " +
                                      "ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, " +
                                      "ShipRegion, ShipPostalCode, ShipCountry " +
                                      "from Orders";
                command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new Order((int)reader["OrderID"]);
                        FillOrderProperties(order, reader);
                        orders.Add(order);
                    }
                }
            }
            return orders;
        }

        public List<OrderDetailedInfo> GetOrderDetailedInformation(int orderId)
        {
            var orderInfos = new List<OrderDetailedInfo>();
            using (var connection = DataFactory.CreateConnection(connectionString, provider))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText =
                    "select o.CustomerID, o.ProductID, p.ProductName, o.UnitPrice, o.Quantity, o.Discount " +
                    "from " +
                    "(select o.OrderID, CustomerID, ProductID, UnitPrice, Quantity, Discount " +
                    "from Orders o " +
                    "left " +
                    "join [Order Details] od " +
                    "on o.OrderID = od.OrderID " +
                    "where o.OrderID = @orderId) o " +
                    "left join Products p " +
                    "on o.ProductID = p.ProductID";

                var orderIdParam = command.CreateParameter();
                orderIdParam.ParameterName = "@orderId";
                orderIdParam.DbType = DbType.Int32;
                orderIdParam.Value = orderId;

                command.Parameters.Add(orderIdParam);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var customerId = (string)reader["CustomerId"];
                        var productId = (int)reader["ProductId"];
                        var productName = (string)reader["ProductName"];
                        var unitPrice = (decimal)reader["UnitPrice"];
                        var quiantity = (short)reader["Quantity"];
                        var discount = (float)reader["Discount"];

                        var orderInfo = new OrderDetailedInfo(orderId, customerId, productId,
                            productName, unitPrice, quiantity, discount);

                        orderInfos.Add(orderInfo);
                    }
                }
            }
            return orderInfos;
        }

        public int CreateOrder(Order order)
        {
            using (var connection = DataFactory.CreateConnection(connectionString, provider))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    "insert Orders " +
                    "(" +
                    "    CustomerID, EmployeeID, OrderDate, RequiredDate," +
                    "    ShippedDate, ShipVia, Freight, ShipName, ShipAddress," +
                    "    ShipCity, ShipRegion, ShipPostalCode, ShipCountry" +
                    ")" +
                    "values" +
                    "(" +
                    "    @customerId, @employeeId, @orderDate, @requiredDate," +
                    "    @shippedDate, @shipVia, @freight, @shipName, @shipAddress," +
                    "    @shipCity, @shipRegion, @shipPostalCode, @shipCountry" +
                    ")";

                CreateParameter("@customerId", DbType.AnsiStringFixedLength, order.CustomerId, command);
                CreateParameter("@employeeId", DbType.Int32, order.EmployeeId, command);
                CreateParameter("@orderDate", DbType.DateTime, order.OrderDate, command);
                CreateParameter("@requiredDate", DbType.DateTime, order.RequiredDate, command);
                CreateParameter("@shippedDate", DbType.DateTime, order.ShippedDate, command);
                CreateParameter("@shipVia", DbType.Int32, order.ShipVia, command);
                CreateParameter("@freight", DbType.Currency, order.Freight, command);
                CreateParameter("@shipName", DbType.AnsiString, order.ShipName, command);
                CreateParameter("@shipAddress", DbType.AnsiString, order.ShipAddress, command);
                CreateParameter("@shipCity", DbType.AnsiString, order.ShipCity, command);
                CreateParameter("@shipRegion", DbType.AnsiString, order.ShipRegion, command);
                CreateParameter("@shipPostalCode", DbType.AnsiString, order.ShipPostalCode, command);
                CreateParameter("@shipCountry", DbType.AnsiString, order.ShipCountry, command);

                var orderId = GetNextOrderId();
                command.ExecuteNonQuery();
                return orderId;
            }
        }

        private void CreateParameter(string name, DbType type, object value, IDbCommand command)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = type;
            param.Value = value ?? DBNull.Value;
            command.Parameters.Add(param);
        }

        public Order GetOrder(int orderId)
        {
            using (var connection = DataFactory.CreateConnection(connectionString, provider))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, " +
                                      "ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, " +
                                      "ShipRegion, ShipPostalCode, ShipCountry " +
                                      "from Orders where OrderID = @orderId";
                command.CommandType = CommandType.Text;
                CreateParameter("@orderId", DbType.Int32, orderId, command);
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    Order order;
                    try
                    {
                        order = new Order((int)reader["OrderID"]);
                        FillOrderProperties(order, reader);
                    }
                    catch (InvalidOperationException)
                    {
                        return null;
                    }
                    return order;
                }
            }
        }

        private void FillOrderProperties(Order order, IDataReader reader)
        {
            order.CustomerId = (string)GetItem("CustomerID", reader);
            order.EmployeeId = (int?)GetItem("EmployeeID", reader);
            order.RequiredDate = (DateTime?)GetItem("RequiredDate", reader);
            order.ShipVia = (int?)GetItem("ShipVia", reader);
            order.Freight = (decimal?)GetItem("Freight", reader);
            order.ShipName = (string)GetItem("ShipName", reader);
            order.ShipAddress = (string)GetItem("ShipAddress", reader);
            order.ShipCity = (string)GetItem("ShipCity", reader);
            order.ShipRegion = (string)GetItem("ShipRegion", reader);
            order.ShipPostalCode = (string)GetItem("ShipPostalCode", reader);
            order.ShipCountry = (string)GetItem("ShipCountry", reader);

            if (reader["OrderDate"] is DBNull) return;
            order.StartProcessing((DateTime)reader["OrderDate"]);

            if (!(reader["ShippedDate"] is DBNull))
            {
                order.Deliver((DateTime)reader["ShippedDate"]);
            }
        }

        private object GetItem(string name, IDataReader reader)
        {
            return reader[name] is DBNull ? null : reader[name];
        }

        public void EditOrder(int orderId, Order editedOrder)
        {
            var order = GetOrder(orderId);
            if (order.Status != OrderStatus.New)
            {
                throw new WrongOrderStatusException(OrderStatus.New);
            }

            using (var connection = DataFactory.CreateConnection(connectionString, provider))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "update Orders " +
                    "set " +
                    "CustomerID = @customerId, EmployeeID = @employeeId, OrderDate = @orderDate," +
                    "RequiredDate = @requiredDate, ShippedDate = @shippedDate, " +
                    "ShipVia = @shipVia, Freight = @freight, ShipName = @shipName, " +
                    "ShipAddress = @shipAddress, ShipCity = @shipCity, ShipRegion = @shipRegion, " +
                    "ShipPostalCode = @shipPostalCode, ShipCountry = @shipCountry " +
                    "where OrderID = @orderId";
                CreateParameter("@orderId", DbType.Int32, orderId, command);
                CreateParameter("@customerId", DbType.AnsiStringFixedLength, editedOrder.CustomerId, command);
                CreateParameter("@employeeId", DbType.Int32, editedOrder.EmployeeId, command);
                CreateParameter("@orderDate", DbType.DateTime, editedOrder.OrderDate, command);
                CreateParameter("@requiredDate", DbType.DateTime, editedOrder.RequiredDate, command);
                CreateParameter("@shippedDate", DbType.DateTime, editedOrder.ShippedDate, command);
                CreateParameter("@shipVia", DbType.Int32, editedOrder.ShipVia, command);
                CreateParameter("@freight", DbType.Currency, editedOrder.Freight, command);
                CreateParameter("@shipName", DbType.AnsiString, editedOrder.ShipName, command);
                CreateParameter("@shipAddress", DbType.AnsiString, editedOrder.ShipAddress, command);
                CreateParameter("@shipCity", DbType.AnsiString, editedOrder.ShipCity, command);
                CreateParameter("@shipRegion", DbType.AnsiString, editedOrder.ShipRegion, command);
                CreateParameter("@shipPostalCode", DbType.AnsiString, editedOrder.ShipPostalCode, command);
                CreateParameter("@shipCountry", DbType.AnsiString, editedOrder.ShipCountry, command);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteOrder(int orderId)
        {
            var order = GetOrder(orderId);
            if (order.Status == OrderStatus.Delivered)
            {
                throw new WrongOrderStatusException();
            }

            using (var connection = DataFactory.CreateConnection(connectionString, provider))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "delete from Orders where OrderID = @orderId";
                CreateParameter("@orderId", DbType.Int32, orderId, command);
                command.ExecuteNonQuery();
            }
        }

        public int GetNextOrderId()
        {
            using (var connection = DataFactory.CreateConnection(connectionString, provider))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "select ident_current('Orders')";
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    var nextId = Convert.ToInt32(reader[0]);
                    return ++nextId;
                }
            }
        }

        public List<OrderHistoryElement> GetCustomerOrderHistory(string customerId)
        {
            using (var connection = DataFactory.CreateConnection(connectionString, provider))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                CreateParameter("@customerId", DbType.AnsiStringFixedLength, customerId, command);
                command.CommandText = "CustOrderHist";
                var orderHistory = new List<OrderHistoryElement>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orderHistory.Add(new OrderHistoryElement((string)reader["ProductName"],
                            (int)reader["Total"]));
                    }
                    return orderHistory;
                }
            }
        }

        public List<OrderDetailElement> GetCustomerOrderDetail(int orderId)
        {
            using (var connection = DataFactory.CreateConnection(connectionString, provider))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                CreateParameter("@orderId", DbType.Int32, orderId, command);
                command.CommandText = "CustOrdersDetail";
                var orderDetails = new List<OrderDetailElement>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orderDetails.Add(new OrderDetailElement((string)reader["ProductName"],
                            (decimal)reader["UnitPrice"], (short)reader["Quantity"],
                            (int)reader["Discount"], (decimal)reader["ExtendedPrice"]));
                    }
                    return orderDetails;
                }
            }
        }
    }
}
