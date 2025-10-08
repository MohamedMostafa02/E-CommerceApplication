# 🛒 Real-time ECommerce Application - ASP.NET Core Web API

A real-time ECommerce Application built using **ASP.NET Core Web API** and **Entity Framework Core** with **SQL Server Database**.  
This project demonstrates a full-featured online store system with real-time functionality, covering all key business modules from customers to payments and feedback.

---

## ✅ Features

### 👤 Customer Module
- Manage customer accounts and information.
- Support for creating and maintaining user profiles.

### 🏠 Address Module
- Add, update, or remove multiple customer addresses.
- Support for managing shipping and billing addresses.

### 🗂️ Category Module
- Organize products under structured categories.
- Easy browsing of products by category.

### 📦 Product Module
- Manage products with details like name, description, price, and stock.
- Retrieve product listings and details.

### 🛒 Shopping Cart Module
- Add and remove products in the cart.
- Update quantities and calculate totals in real time.

### 📑 Order Module
- Place new orders directly from the shopping cart.
- Track order details and statuses.

### 💳 Payment Module
- Handle order payments and transactions.
- Record and verify payment details.

### ❌ Cancellation Module
- Request and manage order cancellations.
- Support for reviewing and updating cancellation requests.

### 💰 Refund Module
- Process refunds for eligible orders.
- Manage refund requests and history.

### ⭐ User Feedback Module
- Allow customers to provide feedback on products.
- Display product ratings and user reviews.

---

## ⚙️ How It Works

1. Customers register and create profiles.  
2. Products are listed under categories and displayed for browsing.  
3. Customers add products to their shopping cart.  
4. Orders are created from the cart and processed.  
5. Payments are handled and recorded.  
6. Customers can cancel or refund orders when needed.  
7. Feedback is collected to improve the shopping experience.  

---

## ▶️ How to Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/realtime-ecommerce-api.git
   cd realtime-ecommerce-api
   ```

2. **Update database connection**
   Open `appsettings.json` and set your SQL Server connection string.

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the project**
   ```bash
   dotnet run
   ```

5. **Access the API**
   Open your browser or API testing tool and navigate to:
   ```
   https://localhost:5001
   ```

---

## 📦 Requirements

- .NET 8 SDK or later  
- SQL Server 2019 or later  
- Visual Studio 2022 or VS Code  

---

## 👤 Author

**Mohamed Mostafa**   
