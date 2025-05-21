# HyperSwitch.Net.Customers SDK

A .NET SDK for interacting with the HyperSwitch Customer CRUD APIs. This library provides a convenient way to manage customer resources in your .NET applications.

## Overview

The SDK allows you to perform the following operations:
- Create a new customer.
- Retrieve an existing customer by their ID.
- Update an existing customer's details.
- Delete a customer.
- List all customers.

It includes models for request and response objects, a typed HTTP client for API communication, and custom exception handling for API errors.

## Installation

You can install the HyperSwitch.Net.Customers SDK via NuGet.

**Using .NET CLI:**
```bash
dotnet add package HyperSwitch.Net.Customers --version 1.0.0
```

**Using NuGet Package Manager Console:**
```powershell
Install-Package HyperSwitch.Net.Customers -Version 1.0.0
```
*(Replace `1.0.0` with the desired version if different.)*

## Usage

### 1. Initialization

First, you need to initialize the `HyperSwitchHttpClient` with your base API URL and API key. Then, create an instance of `CustomerService`.

```csharp
using HyperSwitch.Net.Customers;
using System.Net.Http; // Optional: for custom HttpClient instance

// Your HyperSwitch API base URL and API key
string baseUrl = "https://sandbox.hyperswitch.io"; // Or your production URL
string apiKey = "your_api_key_here";

// Initialize the HttpClient
// You can pass your own HttpClient instance if needed for custom configurations (e.g., proxies, Polly policies)
// var customHttpClient = new HttpClient(); 
// var hyperSwitchClient = new HyperSwitchHttpClient(baseUrl, apiKey, customHttpClient);
var hyperSwitchClient = new HyperSwitchHttpClient(baseUrl, apiKey);

// Initialize the CustomerService
var customerService = new CustomerService(hyperSwitchClient);
```

### 2. API Operations

All API methods are asynchronous and return a `Task`. They also accept an optional `CancellationToken`.

#### Create a Customer

```csharp
using HyperSwitch.Net.Customers.Models;
using System.Threading.Tasks;

// ... (Initialization code from above)

async Task CreateNewCustomerAsync()
{
    var newCustomerRequest = new CustomerRequest
    {
        CustomerId = "cust_unique_id_12345", // Optional, can be set by HyperSwitch if omitted
        Email = "new.customer@example.com",
        Name = "John Doe",
        Phone = "1234567890",
        PhoneCountryCode = "+1",
        Description = "A new customer for testing",
        Address = new AddressDetails
        {
            Line1 = "123 Main St",
            City = "Anytown",
            State = "CA",
            Zip = "90210",
            Country = "US",
            FirstName = "John", // Optional, can be part of address
            LastName = "Doe"    // Optional, can be part of address
        },
        Metadata = new System.Collections.Generic.Dictionary<string, string>
        {
            { "internal_ref", "XYZ789" }
        }
    };

    try
    {
        CustomerResponse? createdCustomer = await customerService.CreateCustomerAsync(newCustomerRequest);
        if (createdCustomer != null)
        {
            System.Console.WriteLine($"Customer created successfully. ID: {createdCustomer.CustomerId}");
            // Use createdCustomer properties
        }
    }
    catch (HyperSwitch.Net.Customers.Exceptions.HyperSwitchApiException ex)
    {
        System.Console.WriteLine($"API Error: {ex.Message}");
        System.Console.WriteLine($"Status Code: {ex.StatusCode}");
        System.Console.WriteLine($"API Error Code: {ex.ApiErrorCode}");
        System.Console.WriteLine($"API Error Message: {ex.ApiErrorMessage}");
        // Handle specific error codes or messages
    }
    catch (System.Exception ex)
    {
        System.Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
}
```

#### Retrieve a Customer

```csharp
// ... (Initialization code from above)

async Task GetCustomerAsync(string customerId)
{
    try
    {
        CustomerResponse? customer = await customerService.RetrieveCustomerAsync(customerId);
        if (customer != null)
        {
            System.Console.WriteLine($"Retrieved Customer - ID: {customer.CustomerId}, Email: {customer.Email}, Name: {customer.Name}");
        }
        else
        {
            System.Console.WriteLine($"Customer with ID {customerId} not found or an issue occurred.");
        }
    }
    catch (HyperSwitch.Net.Customers.Exceptions.HyperSwitchApiException ex)
    {
        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            System.Console.WriteLine($"Customer with ID {customerId} not found.");
        }
        else
        {
            System.Console.WriteLine($"API Error retrieving customer: {ex.Message}");
        }
    }
    // ... other catch blocks
}
```

#### Update a Customer

```csharp
// ... (Initialization code from above)

async Task ModifyCustomerAsync(string customerIdToUpdate)
{
    var customerUpdateRequest = new CustomerUpdateRequest
    {
        Name = "Jane Doe Updated",
        Email = "jane.doe.updated@example.com",
        Description = "Updated customer information."
        // Include only the fields you want to update.
        // Null fields in the request might be ignored or might clear existing values,
        // depending on the API's behavior (check HyperSwitch documentation).
    };

    try
    {
        CustomerResponse? updatedCustomer = await customerService.UpdateCustomerAsync(customerIdToUpdate, customerUpdateRequest);
        if (updatedCustomer != null)
        {
            System.Console.WriteLine($"Customer updated successfully. ID: {updatedCustomer.CustomerId}, New Name: {updatedCustomer.Name}");
        }
    }
    // ... (Error handling similar to CreateCustomerAsync)
    catch (HyperSwitch.Net.Customers.Exceptions.HyperSwitchApiException ex)
    {
        System.Console.WriteLine($"API Error updating customer: {ex.Message}");
    }
}
```

#### Delete a Customer

```csharp
// ... (Initialization code from above)

async Task RemoveCustomerAsync(string customerIdToDelete)
{
    try
    {
        CustomerDeleteResponse? deleteResponse = await customerService.DeleteCustomerAsync(customerIdToDelete);
        if (deleteResponse != null && deleteResponse.CustomerDeleted)
        {
            System.Console.WriteLine($"Customer with ID {deleteResponse.CustomerId} deleted successfully.");
        }
        else
        {
            System.Console.WriteLine($"Failed to delete customer with ID {customerIdToDelete}. Response: {deleteResponse?.CustomerDeleted}");
        }
    }
    // ... (Error handling similar to CreateCustomerAsync)
    catch (HyperSwitch.Net.Customers.Exceptions.HyperSwitchApiException ex)
    {
        System.Console.WriteLine($"API Error deleting customer: {ex.Message}");
    }
}
```

#### List Customers

```csharp
// ... (Initialization code from above)

async Task GetAllCustomersAsync()
{
    try
    {
        System.Collections.Generic.List<CustomerResponse>? customers = await customerService.ListCustomersAsync();
        if (customers != null)
        {
            System.Console.WriteLine($"Retrieved {customers.Count} customers:");
            foreach (var customer in customers)
            {
                System.Console.WriteLine($"- ID: {customer.CustomerId}, Name: {customer.Name}, Email: {customer.Email}");
            }
        }
        else
        {
            System.Console.WriteLine("No customers found or an error occurred.");
        }
    }
    // ... (Error handling similar to CreateCustomerAsync)
    catch (HyperSwitch.Net.Customers.Exceptions.HyperSwitchApiException ex)
    {
        System.Console.WriteLine($"API Error listing customers: {ex.Message}");
    }
}
```

### 3. Error Handling

The SDK methods can throw a `HyperSwitchApiException` when the API returns an error (e.g., 4xx or 5xx status codes). This exception contains detailed information:
- `StatusCode`: The HTTP status code.
- `ApiErrorCode`: A specific error code from the HyperSwitch API (if provided in the response).
- `ApiErrorMessage`: A descriptive error message from the HyperSwitch API (if provided).
- `RawErrorContent`: The raw string content of the error response.

It's recommended to catch this exception and handle errors appropriately. Standard .NET exceptions like `ArgumentNullException` (for invalid input) or `HttpRequestException` (for network issues before a response is received) can also be thrown.

## Building from Source

If you need to build the SDK from source:
1. Clone the repository: `git clone https://github.com/your-repo/HyperSwitch.Net.Customers.git` (Replace with actual URL)
2. Navigate to the `src/HyperSwitch.Net.Customers` directory.
3. Run `dotnet build -c Release`.

The compiled DLL will be in `bin/Release/netstandard2.0/`.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for bugs, feature requests, or improvements.
(You might want to add more specific contribution guidelines here).

## License

This SDK is licensed under the MIT License. See the `PackageLicenseExpression` in the `.csproj` file or include a `LICENSE` file in your repository. 