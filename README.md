# ECommerce API

## Overview

This ECommerce API is a robust, scalable, and high-performance backend service built using C# and ASP.NET Core. It provides a comprehensive set of endpoints to manage key e-commerce functionalities, including Customer management, Product catalog, Order processing, and Payment transactions.

## Features

- **Customer Management:**
  - CRUD operations for customer profiles.

- **Product Catalog:**
  - CRUD operations for products with categories and tags.
  - Support for product variants and inventory tracking.
  - Search and filtering capabilities for efficient product retrieval.

- **Order Processing:**
  - Order creation, modification, and status tracking.
  - Order history and invoice generation.

- **Payment Integration:**
  - Integration with multiple payment gateways.
![e-commerceApi](https://github.com/user-attachments/assets/dc83cb0b-0706-44a8-9955-6371e840ec31)

## Technology Stack

- **C#:** Core programming language used for building the API, ensuring strong type safety and performance.
- **ASP.NET Core API:** Framework utilized for creating the web API, offering a modular, lightweight, and high-performance architecture.
- **Entity Framework Core:** ORM used for data access, providing a robust and flexible way to interact with the SQL Server database.
- **SQL Server:** Database management system for persistent data storage.
- **Swagger:** Integrated for API documentation and testing, allowing easy interaction with the API endpoints.

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/SayyafHameed/ECommerce_API.git
   cd ECommerce_API
   ```

2. **Set up the database:**
   - Update the connection string in `appsettings.json`.
   - Run the EF Core migrations to set up the database schema.
   ```bash
   dotnet ef database update
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

4. **Access the API documentation:**
   - Navigate to `http://localhost:5000/swagger` to explore and test the API endpoints.

## Contribution

Contributions are welcome! Please fork this repository and submit a pull request for any features, bug fixes, or improvements.

