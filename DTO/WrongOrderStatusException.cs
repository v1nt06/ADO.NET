using System;
using DTO.Enums;

namespace DTO
{
    public sealed class WrongOrderStatusException : Exception
    {
        public WrongOrderStatusException() { }

        public WrongOrderStatusException(OrderStatus expectedStatus)
            : base($"Expected {expectedStatus} order status") { }
    }
}