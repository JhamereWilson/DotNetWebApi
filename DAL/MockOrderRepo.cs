using System.Collections.Generic;
using Square;
using Square.Models;
using Square.Apis;

namespace CSWebAPI.DAL
{
    public class MockOrderRepo : IOrderRepo
    {

        IEnumerable<Order> IOrderRepo.GetAllOrders()
        {
            throw new System.NotImplementedException();
        }

        Order IOrderRepo.GetOrderById()
        {
            throw new System.NotImplementedException();
        }

        void IOrderRepo.CreateOrder()
        {
            throw new System.NotImplementedException();
        }
    }

}