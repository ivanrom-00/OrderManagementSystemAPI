using CustomerService.Interfaces;
using CustomerService.Models;
using CustomerService.Repositories;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.Services
{
    public class CustomerManagementService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerManagementService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _customerRepository.AddCustomerAsync(customer);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            return customers;
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            return customer;
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _customerRepository.UpdateCustomerAsync(customer);
        }
    }
}
