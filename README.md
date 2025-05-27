# .NET 8 Microservices E-Commerce Application

This repository contains a complete microservices-based e-commerce application built using the latest features of **.NET 8**, **Entity Framework Core**, **Ocelot API Gateway**, and **Azure Service Bus**. This is a practical, code-first guide where you will learn to build, structure, and deploy a real-world .NET-based distributed system.

## 📦 Microservices Implemented

| Microservice       | Description |
|--------------------|-------------|
| **Product**        | Handles product catalog and item details |
| **Shopping Cart**  | Manages user shopping carts and updates |
| **Coupon**         | Applies discount codes during checkout |
| **Order**          | Processes customer orders |
| **Payment**        | Handles payment processing |
| **Email**          | Sends order confirmation and related emails |
| **Identity**       | Handles authentication, registration, role-based authorization |

## 🎯 Features

- ✅ Built on **.NET 8** with **Clean Architecture**
- ✅ Role-based Authentication and Authorization with **.NET Identity**
- ✅ **Azure Service Bus** for async communication (Topics & Queues)
- ✅ **RabbitMQ** for async communication (Topics & Queues)
- ✅ **Ocelot Gateway** for centralized routing and API security
- ✅ **MVC Web Application** frontend using Bootstrap 5
- ✅ Swagger UI for all APIs
- ✅ Repository Pattern with EF Core and SQL Server
- ✅ Fully containerized (Docker)


## 🚀 Technologies Used

- ASP.NET Core 8 Web API & MVC
- Ocelot API Gateway
- Entity Framework Core
- Azure Service Bus (Topics, Queues)
- .NET Identity (Authentication & Authorization)
- SQL Server
- Docker, Docker Compose
- Swagger / OpenAPI
- Bootstrap 5

🔐 Authentication
Uses ASP.NET Core Identity for Registration/Login

Role-based access control (Admin, User)

Tokens managed via Identity Microservice

📬 Microservices Communication
Type	Technology	Description
Sync	REST via Ocelot	For immediate inter-service calls
Async	Azure Service Bus	For decoupled message-driven comms

🧱 Architecture
Clean Architecture (Presentation, Application, Infrastructure, Domain)

Repository Pattern + Unit of Work

DTOs, AutoMapper

Dependency Injection

## 📁 Project Structure
📦 FoodCartApplication.sln             # Solution file to build and run all projects
├── 📂 Mango.GatewaySolution          # Ocelot API Gateway for routing and security
│   └── Ocelot configurations, logging, and middleware
│
├── 📂 Mango.MessageBus               # MessageBus abstraction layer
│   └── Azure Service Bus or RabbitMQ integration
│
├── 📂 Mango.Web                      # ASP.NET MVC Web frontend (Bootstrap 5 UI)
│   └── Razor Views, Controllers, Identity Client Integration
│
├── 📂 Mango.Services.AuthAPI        # Identity microservice
│   └── ASP.NET Core Identity, JWT, Roles (Admin/User)
│
├── 📂 Mango.Services.ProductAPI     # Product Catalog microservice
│   └── CRUD operations for products using EF Core
│
├── 📂 Mango.Services.CouponAPI      # Coupon and discount logic
│   └── Manage and validate discount codes
│
├── 📂 Mango.Services.ShoppingCartAPI # Shopping cart operations
│   └── Add/remove/update items in user’s cart
│
├── 📂 Mango.Services.OrderAPI       # Order processing service
│   └── Order creation and status tracking
│
├── 📂 Mango.Services.EmailAPI       # Email notification service
│   └── Sends emails after order confirmation
│
├── 📂 Mango.Services.RewardAPI      # Reward points service
│   └── Manages user loyalty and points
│
├── .gitignore                        # Files/folders to exclude from Git
├── .gitattributes                    # Git settings for line endings, etc.
└── README.md                         # Project overview and usage guide

