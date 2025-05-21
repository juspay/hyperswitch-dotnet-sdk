using Xunit;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using HyperSwitch.Net.Customers;
using HyperSwitch.Net.Customers.Models;
using HyperSwitch.Net.Customers.Exceptions;
using System.Net;
using System.Text.Json;
using System.Collections.Generic;

namespace HyperSwitch.Net.Customers.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<IHyperSwitchHttpClient> _mockHttpClient;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockHttpClient = new Mock<IHyperSwitchHttpClient>(); 
            _customerService = new CustomerService(_mockHttpClient.Object);
        }

        [Fact]
        public async Task CreateCustomerAsync_Successful_ReturnsCustomerResponse()
        {
            // Arrange
            var customerRequest = new CustomerRequest
            {
                CustomerId = "cust_test_123",
                Email = "test@example.com",
                Name = "Test User"
            };
            var expectedResponse = new CustomerResponse
            {
                CustomerId = "cust_test_123",
                Email = "test@example.com",
                Name = "Test User",
                CreatedAt = System.DateTime.UtcNow
            };

            _mockHttpClient.Setup(client => client.SendAsync<CustomerRequest, CustomerResponse>(
                HttpMethod.Post,
                "/customers",
                customerRequest,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((expectedResponse, null));

            // Act
            var actualResponse = await _customerService.CreateCustomerAsync(customerRequest);

            // Assert
            Assert.NotNull(actualResponse);
            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            Assert.Equal(expectedResponse.Email, actualResponse.Email);
            _mockHttpClient.Verify(client => client.SendAsync<CustomerRequest, CustomerResponse>(
                HttpMethod.Post, "/customers", customerRequest, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCustomerAsync_ApiError_ThrowsHyperSwitchApiException()
        {
            // Arrange
            var customerRequest = new CustomerRequest
            {
                Email = "test@example.com"
            };
            var apiException = new HyperSwitchApiException(HttpStatusCode.BadRequest, "invalid_email", "The email address is invalid.", "API error");
            
            _mockHttpClient.Setup(client => client.SendAsync<CustomerRequest, CustomerResponse>(
                HttpMethod.Post,
                "/customers",
                customerRequest,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, apiException));

            // Act & Assert
            var thrownException = await Assert.ThrowsAsync<HyperSwitchApiException>(
                () => _customerService.CreateCustomerAsync(customerRequest));
            
            Assert.Equal(HttpStatusCode.BadRequest, thrownException.StatusCode);
            Assert.Equal("invalid_email", thrownException.ApiErrorCode);
            Assert.Equal("The email address is invalid.", thrownException.ApiErrorMessage);
            _mockHttpClient.Verify(client => client.SendAsync<CustomerRequest, CustomerResponse>(
                HttpMethod.Post, "/customers", customerRequest, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCustomerAsync_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<System.ArgumentNullException>(
                () => _customerService.CreateCustomerAsync(null!));
        }

        [Fact]
        public async Task RetrieveCustomerAsync_Successful_ReturnsCustomerResponse()
        {
            // Arrange
            var customerId = "cust_test_456";
            var expectedResponse = new CustomerResponse
            {
                CustomerId = customerId,
                Email = "retrieved@example.com",
                Name = "Retrieved User",
                CreatedAt = System.DateTime.UtcNow
            };

            _mockHttpClient.Setup(client => client.SendAsync<object, CustomerResponse>(
                HttpMethod.Get,
                $"/customers/{customerId}",
                null,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((expectedResponse, null));

            // Act
            var actualResponse = await _customerService.RetrieveCustomerAsync(customerId);

            // Assert
            Assert.NotNull(actualResponse);
            Assert.Equal(expectedResponse.CustomerId, actualResponse.CustomerId);
            _mockHttpClient.Verify(client => client.SendAsync<object, CustomerResponse>(
                HttpMethod.Get, $"/customers/{customerId}", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveCustomerAsync_NotFound_ThrowsHyperSwitchApiException()
        {
            // Arrange
            var customerId = "cust_not_found_789";
            var apiException = new HyperSwitchApiException(HttpStatusCode.NotFound, "customer_not_found", "The requested customer was not found.", "API error");

            _mockHttpClient.Setup(client => client.SendAsync<object, CustomerResponse>(
                HttpMethod.Get,
                $"/customers/{customerId}",
                null,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, apiException));

            // Act & Assert
            var thrownException = await Assert.ThrowsAsync<HyperSwitchApiException>(
                () => _customerService.RetrieveCustomerAsync(customerId));
            
            Assert.Equal(HttpStatusCode.NotFound, thrownException.StatusCode);
             _mockHttpClient.Verify(client => client.SendAsync<object, CustomerResponse>(
                HttpMethod.Get, $"/customers/{customerId}", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RetrieveCustomerAsync_InvalidCustomerId_ThrowsArgumentNullException(string invalidCustomerId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<System.ArgumentNullException>(
                () => _customerService.RetrieveCustomerAsync(invalidCustomerId!));
        }

        [Fact]
        public async Task UpdateCustomerAsync_Successful_ReturnsUpdatedCustomerResponse()
        {
            // Arrange
            var customerId = "cust_update_123";
            var customerUpdateRequest = new CustomerUpdateRequest
            {
                Name = "Updated Name",
                Email = "updated.email@example.com"
            };
            var expectedResponse = new CustomerResponse
            {
                CustomerId = customerId,
                Name = customerUpdateRequest.Name,
                Email = customerUpdateRequest.Email,
                CreatedAt = System.DateTime.UtcNow
            };

            _mockHttpClient.Setup(client => client.SendAsync<CustomerUpdateRequest, CustomerResponse>(
                HttpMethod.Post, 
                $"/customers/{customerId}",
                customerUpdateRequest,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((expectedResponse, null));

            // Act
            var actualResponse = await _customerService.UpdateCustomerAsync(customerId, customerUpdateRequest);

            // Assert
            Assert.NotNull(actualResponse);
            Assert.Equal(expectedResponse.Name, actualResponse.Name);
            Assert.Equal(expectedResponse.Email, actualResponse.Email);
            _mockHttpClient.Verify(client => client.SendAsync<CustomerUpdateRequest, CustomerResponse>(
                HttpMethod.Post, $"/customers/{customerId}", customerUpdateRequest, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerAsync_CustomerNotFound_ThrowsHyperSwitchApiException()
        {
            // Arrange
            var customerId = "cust_not_found_for_update";
            var customerUpdateRequest = new CustomerUpdateRequest { Name = "New Name" };
            var apiException = new HyperSwitchApiException(HttpStatusCode.NotFound, "update_customer_not_found", "Customer to update not found.", "API error");

            _mockHttpClient.Setup(client => client.SendAsync<CustomerUpdateRequest, CustomerResponse>(
                HttpMethod.Post,
                $"/customers/{customerId}",
                customerUpdateRequest,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, apiException));

            // Act & Assert
            var thrownException = await Assert.ThrowsAsync<HyperSwitchApiException>(
                () => _customerService.UpdateCustomerAsync(customerId, customerUpdateRequest));
            
            Assert.Equal(HttpStatusCode.NotFound, thrownException.StatusCode);
            _mockHttpClient.Verify(client => client.SendAsync<CustomerUpdateRequest, CustomerResponse>(
                HttpMethod.Post, $"/customers/{customerId}", customerUpdateRequest, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdateCustomerAsync_InvalidCustomerId_ThrowsArgumentNullException(string invalidCustomerId)
        {
            var customerUpdateRequest = new CustomerUpdateRequest { Name = "Valid Name" };
            await Assert.ThrowsAsync<System.ArgumentNullException>(
                () => _customerService.UpdateCustomerAsync(invalidCustomerId!, customerUpdateRequest));
        }

        [Fact]
        public async Task UpdateCustomerAsync_NullRequest_ThrowsArgumentNullException()
        {
            var customerId = "cust_valid_id";
            await Assert.ThrowsAsync<System.ArgumentNullException>(
                () => _customerService.UpdateCustomerAsync(customerId, null!));
        }

        [Fact]
        public async Task DeleteCustomerAsync_Successful_ReturnsDeleteResponse()
        {
            // Arrange
            var customerId = "cust_to_delete_456";
            var expectedResponse = new CustomerDeleteResponse
            {
                CustomerId = customerId,
                CustomerDeleted = true
            };

            _mockHttpClient.Setup(client => client.SendAsync<object, CustomerDeleteResponse>(
                HttpMethod.Delete,
                $"/customers/{customerId}",
                null,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((expectedResponse, null));

            // Act
            var actualResponse = await _customerService.DeleteCustomerAsync(customerId);

            // Assert
            Assert.NotNull(actualResponse);
            Assert.True(actualResponse.CustomerDeleted);
            Assert.Equal(customerId, actualResponse.CustomerId);
            _mockHttpClient.Verify(client => client.SendAsync<object, CustomerDeleteResponse>(
                HttpMethod.Delete, $"/customers/{customerId}", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerAsync_CustomerNotFound_ThrowsHyperSwitchApiException()
        {
            // Arrange
            var customerId = "cust_not_found_for_delete";
            var apiException = new HyperSwitchApiException(HttpStatusCode.NotFound, "delete_customer_not_found", "Customer to delete not found.", "API error");

            _mockHttpClient.Setup(client => client.SendAsync<object, CustomerDeleteResponse>(
                HttpMethod.Delete,
                $"/customers/{customerId}",
                null,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, apiException));

            // Act & Assert
            var thrownException = await Assert.ThrowsAsync<HyperSwitchApiException>(
                () => _customerService.DeleteCustomerAsync(customerId));
            
            Assert.Equal(HttpStatusCode.NotFound, thrownException.StatusCode);
            _mockHttpClient.Verify(client => client.SendAsync<object, CustomerDeleteResponse>(
                HttpMethod.Delete, $"/customers/{customerId}", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task DeleteCustomerAsync_InvalidCustomerId_ThrowsArgumentNullException(string invalidCustomerId)
        {
            await Assert.ThrowsAsync<System.ArgumentNullException>(
                () => _customerService.DeleteCustomerAsync(invalidCustomerId!));
        }

        [Fact]
        public async Task ListCustomersAsync_Successful_ReturnsListOfCustomerResponses()
        {
            // Arrange
            var expectedResponseList = new List<CustomerResponse>
            {
                new CustomerResponse { CustomerId = "cust_list_1", Email = "list1@example.com" },
                new CustomerResponse { CustomerId = "cust_list_2", Email = "list2@example.com" }
            };

            _mockHttpClient.Setup(client => client.SendAsync<object, List<CustomerResponse>>(
                HttpMethod.Get,
                "/customers/list",
                null,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((expectedResponseList, null));

            // Act
            var actualResponseList = await _customerService.ListCustomersAsync();

            // Assert
            Assert.NotNull(actualResponseList);
            Assert.Equal(2, actualResponseList.Count);
            _mockHttpClient.Verify(client => client.SendAsync<object, List<CustomerResponse>>(
                HttpMethod.Get, "/customers/list", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ListCustomersAsync_Successful_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var expectedResponseList = new List<CustomerResponse>(); // Empty list

            _mockHttpClient.Setup(client => client.SendAsync<object, List<CustomerResponse>>(
                HttpMethod.Get,
                "/customers/list",
                null,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((expectedResponseList, null));

            // Act
            var actualResponseList = await _customerService.ListCustomersAsync();

            // Assert
            Assert.NotNull(actualResponseList);
            Assert.Empty(actualResponseList);
            _mockHttpClient.Verify(client => client.SendAsync<object, List<CustomerResponse>>(
                HttpMethod.Get, "/customers/list", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ListCustomersAsync_ApiError_ThrowsHyperSwitchApiException()
        {
            // Arrange
            var apiException = new HyperSwitchApiException(HttpStatusCode.InternalServerError, "list_error", "Error fetching customer list.", "API error");

            _mockHttpClient.Setup(client => client.SendAsync<object, List<CustomerResponse>>(
                HttpMethod.Get,
                "/customers/list",
                null,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, apiException));

            // Act & Assert
            var thrownException = await Assert.ThrowsAsync<HyperSwitchApiException>(
                () => _customerService.ListCustomersAsync());
            
            Assert.Equal(HttpStatusCode.InternalServerError, thrownException.StatusCode);
            _mockHttpClient.Verify(client => client.SendAsync<object, List<CustomerResponse>>(
                HttpMethod.Get, "/customers/list", null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}