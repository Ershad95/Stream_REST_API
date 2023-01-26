using Flurl;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Stream.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IDictionary<int, Customer> _customers;
        private void FillCustomerFromMemory(int countOfCustomer, CancellationToken cancellationToken = default)
        {
            for (int CustomerId = 1; CustomerId <= countOfCustomer && !cancellationToken.IsCancellationRequested; CustomerId++)
            {
                _customers.Add(key: CustomerId, new Customer($"name_{CustomerId}", CustomerId));
            }
        }

        public CustomerController()
        {
            _customers = new Dictionary<int, Customer>();
            FillCustomerFromMemory(countOfCustomer: 100);
        }

        [HttpGet]
        public async IAsyncEnumerable<Customer> Get(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _customers.Any(_ => _.Key % 10 == 0))
            {
                var customer = _customers.First(_ => _.Key % 10 == 0);
                yield return new Customer(customer.Value.Name, customer.Key);
                _customers.Remove(customer);
                await Task.Delay(500, cancellationToken);
            }
        }

        public class Customer
        {
            public int Id { get; private set; }

            public string Name { get; private set; }
            public Customer(string name, int id)
            {
                Name = name;
                Id = id;
            }
        }
    }
}