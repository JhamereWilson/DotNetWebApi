using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Square;
using Square.Models;
using Square.Apis;
using Square.Exceptions;
using CSWebAPI.Models;

namespace CSWebAPI.Controllers
{
    [ApiController]
    public class PaymentsController : ControllerBase
    {

        private SquareClient client;
        private readonly string _locationId;

        private Models.Address _address;

        private readonly List<OrderLineItem> _clientLineItems;
        public PaymentsController(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // Get environment
            Square.Environment environment = configuration["AppSettings:Environment"] == "sandbox" ?
              Square.Environment.Sandbox : Square.Environment.Production;

            // Build base client
            client = new SquareClient.Builder()
              .Environment(environment)
              .AccessToken(configuration["AppSettings:AccessToken"])
              .Build();

            _locationId = configuration["AppSettings:LocationId"];

        }

        [HttpPost("createCustomer")]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            var address = new Square.Models.Address.Builder()
              .AddressLine1(customer.Address.AddressLine1)
              .PostalCode(customer.Address.PostalCode)
              .Country("US")
              .FirstName(customer.GivenName)
              .LastName(customer.FamilyName)
              .Build();

            var body = new CreateCustomerRequest.Builder()
              .GivenName(customer.GivenName)
              .FamilyName(customer.FamilyName)
              .EmailAddress(customer.EmailAddress)
              .Address(customer.Address)
              .Birthday(customer.Birthday)
              .Build();

            try
            {
                var result = await client.CustomersApi.CreateCustomerAsync(body: body);
                return Ok(result);
            }
            catch (ApiException e)
            {
                Console.WriteLine(body);
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
                return BadRequest();
            }



        }


        [HttpPost("checkout")]
        async public Task<IActionResult> OnPost()
        {
            ICheckoutApi checkoutApi = client.CheckoutApi;
            try
            {
                // Get the currency for the location
                RetrieveLocationResponse locationResponse = await client.LocationsApi.RetrieveLocationAsync(locationId: _locationId);
                string currency = locationResponse.Location.Currency;

                // create line items for the order
                // This example assumes the order information is retrieved and hard coded
                // You can find different ways to retrieve order information and fill in the following lineItems object.
                List<OrderLineItem> lineItems = new List<OrderLineItem>();




                foreach (OrderLineItem l in _clientLineItems)
                {

                    var orderLineItem = new OrderLineItem.Builder(quantity: l.Quantity)
                                                      .CatalogObjectId(l.CatalogObjectId)
                                                      .Build();
                    lineItems.Add(orderLineItem);
                }

                var squareAddress = new Square.Models.Address.Builder()
                    .AddressLine1(_address.AddressLine1)
                    .AddressLine2(_address.AddressLine2)
                    .PostalCode(_address.PostalCode)
                    .Country("US")
                    .FirstName(_address.FirstName)
                    .LastName(_address.LastName)
                    .Build();


                var squareRecipient = new OrderFulfillmentRecipient.Builder()
                .DisplayName(_address.FirstName + _address.LastName)
                .EmailAddress(_address.EmailAddress)
                .PhoneNumber(_address.PhoneNumber)
                .Address(squareAddress)
                .Build();

                var squareShipmentDetails = new OrderFulfillmentShipmentDetails.Builder()
                .Recipient(squareRecipient)
                .Build();

                var orderFulfillment = new OrderFulfillment.Builder()
                .ShipmentDetails(squareShipmentDetails)
                .Build();

                var squareFulfillments = new List<OrderFulfillment>();
                squareFulfillments.Add(orderFulfillment);



                // create Order object with line items
                Order order = new Order.Builder(_locationId)
                  .LineItems(lineItems)
                  .Fulfillments(squareFulfillments)
                  .Build();

                // create order request with order
                CreateOrderRequest orderRequest = new CreateOrderRequest.Builder()
                  .Order(order)
                  .Build();

                // create checkout request with the previously created order
                CreateCheckoutRequest createCheckoutRequest = new CreateCheckoutRequest.Builder(
                    Guid.NewGuid().ToString(),
                    orderRequest)
                  .Build();

                // create checkout response, and redirect to checkout page if successful
                CreateCheckoutResponse response = checkoutApi.CreateCheckout(_locationId, createCheckoutRequest);
                return Redirect(response.Checkout.CheckoutPageUrl);
            }
            catch (ApiException e)
            {
                return RedirectToPage("Error", new { error = e.Message });
            }
        }




    }

}
