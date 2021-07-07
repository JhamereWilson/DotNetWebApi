using System.Collections.Generic;
using Square;
using Square.Models;
using Square.Apis;

namespace CSWebAPI.DAL
{
    public interface IOrderRepo
    {

        IEnumerable<Order> GetAllOrders();
        Order GetOrderById();

        void CreateOrder();



    }

}