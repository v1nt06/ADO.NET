using System;

namespace DTO
{
    public sealed class WrongOrderStatusException : Exception
    {
        public WrongOrderStatusException() { }

        public WrongOrderStatusException(OrderStatus expectedStatus)
            : base($"Expected {expectedStatus} order status") { }
    }
}