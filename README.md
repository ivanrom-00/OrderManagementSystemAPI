# Order Management System API

This is a  **.NET** API Project for completing the Tech Assessment.
It consists of three microservices that manage three main entities:
- Products
- Customers
- Orders

Each microservice has its own database and handles the basic CRUD operations for their corresponding entity.

## Project Structure

The project has a general solution: **OrderManagementSystem.sln**
This solution is composed by different projects:

- Libraries
	- Contains a SharedLibrary with reusable classes used accross the services
- RabbitMQSamples
	- Contains examples on how to communicate the services between each other
- Services
	- Contains each service with their corresponding API structure (Data, Models, Repositories, Services, Controllers)
- Tests
	- xUnit Project that contains unit tests for all important operations in the services