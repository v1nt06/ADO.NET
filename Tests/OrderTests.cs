using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public sealed class OrderTests
    {
        [TestMethod]
        public void GetOrders()
        {
            var orderEngine = new OrderEngine();
            var actualOrders = orderEngine.GetOrders();

            var expectedOrders = new List<Order>();

            var connection = DataFactory.CreateConnection();
            using (connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "select OrderDate, ShippedDate " +
                                      "from Orders";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new Order();
                        if (!(reader["OrderDate"] is DBNull))
                        {
                            order.StartProcessing((DateTime)reader["OrderDate"]);

                            if (!(reader["ShippedDate"] is DBNull))
                            {
                                order.Deliver((DateTime)reader["ShippedDate"]);
                            }
                        }
                        expectedOrders.Add(order);
                    }
                }
            }

            var expectedStatuses = expectedOrders.Select(o => o.Status).ToList();
            var actualStatuses = actualOrders.Select(o => o.Status).ToList();

            Assert.AreEqual(expectedOrders.Count, actualOrders.Count);
            CollectionAssert.AreEqual(expectedStatuses, actualStatuses);
        }

        [TestMethod]
        public void GetOrderDetailedInfo()
        {
            var orderEngine = new OrderEngine();
            var actualInfo = orderEngine.GetOrderDetailedInformation(10248);

            var expectedInfo = new List<OrderDetailedInfo>
                {
                    new OrderDetailedInfo(10248, "VINET", 11, "Queso Cabrales", 14m, 12, 0),
                    new OrderDetailedInfo(10248, "VINET", 42, "Singaporean Hokkien Fried Mee", 9.80m, 10, 0),
                    new OrderDetailedInfo(10248, "VINET", 72, "Mozzarella di Giovanni", 34.80m, 5, 0)
                };

            CollectionAssert.AreEqual(expectedInfo, actualInfo);
        }

        [TestMethod]
        public void CreateOrder()
        {
            var orderEngine = new OrderEngine();

            var order = new Order
            {
                CustomerId = "VINET",
                EmployeeId = 5,
                Freight = 32.38m,
                RequiredDate = new DateTime(1996, 08, 01),
                ShipAddress = "59 rue de l'Abbaye",
                ShipCity = "Reims",
                ShipCountry = "France",
                ShipName = "Vins et alcools Chevalier",
                ShipPostalCode = "51100",
                ShipRegion = null,
                ShipVia = 3,
            };

            var orderId = orderEngine.GetNextOrderId();

            orderEngine.CreateOrder(order);

            Assert.AreEqual(order, orderEngine.GetOrder(orderId));
        }

        [TestMethod]
        public void EditNewOrder()
        {
            var order = new Order();
            var orderEngine = new OrderEngine();
            var newOrderId = orderEngine.CreateOrder(order);

            order.CustomerId = "VINET";
            order.EmployeeId = 5;
            order.Freight = 32.38m;
            order.RequiredDate = new DateTime(1996, 08, 01);
            order.ShipAddress = "59 rue de l'Abbaye";
            order.ShipCity = "Reims";
            order.ShipCountry = "France";
            order.ShipName = "Vins et alcools Chevalier";
            order.ShipPostalCode = "51100";
            order.ShipRegion = "SP";
            order.ShipVia = 3;

            orderEngine.EditOrder(newOrderId, order);

            Assert.AreEqual(order, orderEngine.GetOrder(newOrderId));
        }

        [TestMethod]
        [ExpectedException(typeof(WrongOrderStatusException))]
        public void EditNotNewOrder()
        {
            var order = new Order();
            order.StartProcessing(DateTime.Today);
            var orderEngine = new OrderEngine();
            var orderId = orderEngine.CreateOrder(order);
            orderEngine.EditOrder(orderId, order);
        }

        [TestMethod]
        public void DeleteOrder()
        {
            var order = new Order();
            order.StartProcessing(DateTime.Today);
            var orderEngine = new OrderEngine();
            var orderId = orderEngine.CreateOrder(order);
            orderEngine.DeleteOrder(orderId);
            Assert.IsNull(orderEngine.GetOrder(orderId));
        }

        [TestMethod]
        [ExpectedException(typeof(WrongOrderStatusException))]
        public void DeleteNewOrder()
        {
            var order = new Order();
            var orderEngine = new OrderEngine();
            var newOrderId = orderEngine.CreateOrder(order);
            orderEngine.DeleteOrder(newOrderId);
        }

        [TestMethod]
        public void StartOrderProcessing()
        {
            var order = new Order();
            order.StartProcessing(DateTime.Today);
            Assert.AreEqual(OrderStatus.Processing, order.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(WrongOrderStatusException))]
        public void DeliverUnprocessedOrder()
        {
            var order = new Order();
            order.Deliver(DateTime.Today);
        }

        [TestMethod]
        public void DeliverOrder()
        {
            var order = new Order();
            order.StartProcessing(DateTime.Today);
            order.Deliver(DateTime.Today);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeliverOrdeerBeforeProcessing()
        {
            var order = new Order();
            order.StartProcessing(DateTime.Today);
            order.Deliver(DateTime.Today - TimeSpan.FromDays(1));
        }

        [TestMethod]
        public void GetCustomerOrderHistory()
        {
            var orderEngine = new OrderEngine();
            var actualOrderHistory = orderEngine.GetCustomerOrderHistory("TOMSP");

            var expectedOrderHistory = new List<OrderHistoryElement>
            {
                new OrderHistoryElement("Filo Mix", 15),
                new OrderHistoryElement("Gnocchi di nonna Alice", 28),
                new OrderHistoryElement("Gorgonzola Telino", 3),
                new OrderHistoryElement("Guarana Fantastica", 20),
                new OrderHistoryElement("Jack's New England Clam Chowder", 14),
                new OrderHistoryElement("Manjimup Dried Apples", 40),
                new OrderHistoryElement("Maxilaku", 40),
                new OrderHistoryElement("Ravioli Angelo", 15),
                new OrderHistoryElement("Sasquatch Ale", 30),
                new OrderHistoryElement("Teatime Chocolate Biscuits", 39),
                new OrderHistoryElement("Tofu", 9)
            };

            

           CollectionAssert.AreEqual(expectedOrderHistory, actualOrderHistory);
        }

        [TestMethod]
        public void GetCustomerOrderDetail()
        {
            var orderEngine = new OrderEngine();
            var actualOrderDetails = orderEngine.GetCustomerOrderDetail(10248);

            var expectedOrderDetails = new List<OrderDetailElement>
            {
                new OrderDetailElement("Queso Cabrales", 14.00m, 12, 0, 168.00m),
                new OrderDetailElement("Singaporean Hokkien Fried Mee", 9.80m, 10, 0, 98.00m),
                new OrderDetailElement("Mozzarella di Giovanni", 34.80m, 5, 0, 174.00m)
            };

            CollectionAssert.AreEqual(expectedOrderDetails, actualOrderDetails);
        }
    }
}
