using Azure.Identity;
using ECommerceApp.Data;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.OrderDTOs;
using ECommerceApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Security.Cryptography;

namespace ECommerceApp.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        // Allowed order status transitions for validating status changes.

        private static readonly Dictionary<OrderStatus, List<OrderStatus>>
            AllowedStatusTransitions = new()
            {
                {
                    OrderStatus.Pending , new List<OrderStatus>() {OrderStatus.Processing,OrderStatus.Canceled }
                },
                {
                    OrderStatus.Processing, new List<OrderStatus>() {OrderStatus.Shipped,OrderStatus.Canceled }
                },
                {
                    OrderStatus.Shipped,new List<OrderStatus>() {OrderStatus.Delivered }
                },
                {
                    OrderStatus.Delivered,new List<OrderStatus>() // Terminal state
                },
                {
                OrderStatus.Canceled, new List<OrderStatus>()  // Terminal state
                }

            };

        // Inject the Application DdContext.
        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new order from the provided OrderCreateDTO.
        // This method validates customer and address data, chekds product stock.
        // Calculates financial totals, deducts product stock, and marks any active cart as checked out.

        public async Task<ApiResponse<OrderResponseDTO>> CreateOrderAsync(OrderCreateDTO orderCreateDTO)
        {
            try
            {
                // Validate that the customer exists.
                var customer = await _context.Customers.FindAsync(orderCreateDTO.CustomerId);

                if (customer == null)
                {
                    return new ApiResponse<OrderResponseDTO>(404, "Customer doesn't exits.");
                }

                // Check if there active cart before making the order

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CustomerId == orderCreateDTO.CustomerId && !c.IsCheckOut);

                if (cart == null || !cart.CartItems.Any())
                {
                    return new ApiResponse<OrderResponseDTO>(400, "You can't create an order without items in your cart.");
                }

                // Validate that billing Address exits and belongs to the customer.
                var billingAddress = await _context.Addresses.FindAsync(orderCreateDTO.BillingAddressId);
                if(billingAddress == null || billingAddress.CustomerID != orderCreateDTO.CustomerId)
                {
                    return new ApiResponse<OrderResponseDTO>(400, "Billing Address is invalid or doesn't belong to the customer.");
                }

                // Validate that shipping address exists and belongs to the customer.
                var shippingAddress = await _context.Addresses.FindAsync(orderCreateDTO.ShippingAddressId);

                if (shippingAddress == null || shippingAddress.CustomerID != orderCreateDTO.CustomerId)
                {
                    return new ApiResponse<OrderResponseDTO>(400, "Shipping Address is invalid or doesn't belong to the customer.");
                }

                // Initialize financial tracking.
                decimal totalBaseAmount = 0;
                decimal totalDiscountAmount = 0;
                decimal shippingCost = 10.00m;  // example fixed shipping cost
                decimal totalAmount = 0;

                // Generate a unique order number.
                string orderNumber = GenerateOrderNumber();
                // List to hold order items.
                var orderItems = new List<OrderItem>();

                //// Process each order item from the DTO

                //foreach(var itemDto in orderCreateDTO.OrderItems)
                //{
                //    // Check if the product exits.
                //    var product = await _context.Products.FindAsync(itemDto.ProductId);
                //    if(product == null)
                //    {
                //        return new ApiResponse<OrderResponseDTO>(404, $"Product with ID {itemDto.ProductId} doesn't exits.");
                //    }

                //    // Check if sufficient stock is available.
                //    if(product.StockQuantity < itemDto.Quantity)
                //    {
                //        return new ApiResponse<OrderResponseDTO>(400, $"Insufficient stock for product {product.Name}");
                //    }

                //    // Calculate base price, discount, and total price for the order item.
                //    decimal basePrice = itemDto.Quantity * product.Price;
                //    decimal discount = (product.DiscountPercentage / 100.0m) * basePrice;
                //    decimal totalPrice = basePrice - discount;

                //    // Create a new orderItem.
                //    var orderItem = new OrderItem
                //    {
                //        ProductId = product.Id,
                //        Quantity = itemDto.Quantity,
                //        UnitPrice = product.Price,
                //        Discount = discount,
                //        TotalPrice = totalPrice,
                //    };

                //    // Add the order item to the list.
                //    orderItems.Add(orderItem);

                //    // Update the running totals.
                //    totalBaseAmount += basePrice;
                //    totalDiscountAmount += discount;

                //    // Deduct the purchased quntity from the product's stock.
                //    product.StockQuantity -= itemDto.Quantity;
                //    _context.Products.Update(product);
                //}

                // Move the CartItems to the Order.
                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _context.Products.FindAsync(cartItem.ProductId);

                    if (product == null)
                    {
                        return new ApiResponse<OrderResponseDTO>(404, $"Product with ID {cartItem.ProductId} doesn't exits.");

                    }

                    // Check if sufficient stock is available.
                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        return new ApiResponse<OrderResponseDTO>(400, $"Insufficient stock for product {product.Name}");
                    }

                    // Calculate base price, discount, and total price for the order item.
                    decimal basePrice = cartItem.Quantity * product.Price;
                    decimal discount = (product.DiscountPercentage / 100.0m) * basePrice;
                    decimal totalPrice = basePrice - discount;

                    // Create a new orderItem.
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = cartItem.Quantity,
                        UnitPrice = product.Price,
                        Discount = discount,
                        TotalPrice = totalPrice,
                    };

                    // Add the order item to the list.
                    orderItems.Add(orderItem);

                    // Update the running totals.
                    totalBaseAmount += basePrice;
                    totalDiscountAmount += discount;

                    // Deduct the purchased quntity from the product's stock.
                    product.StockQuantity -= cartItem.Quantity;
                    _context.Products.Update(product);
                }

                // Calculate the final total amount 
                totalAmount = totalBaseAmount - totalDiscountAmount + shippingCost;

                // Manually map from DTO to Order model.

                var order = new Order
                {
                    OrderNumber = orderNumber,
                    CustomerId = orderCreateDTO.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    BillingAddressId = orderCreateDTO.BillingAddressId,
                    ShippingAddressId = orderCreateDTO.ShippingAddressId,
                    TotalBaseAmount = totalBaseAmount,
                    TotalDiscountAmount = totalDiscountAmount,
                    ShippingCost = shippingCost,
                    TotalAmount = totalAmount,
                    OrderStatus = OrderStatus.Pending,
                    OrderItems = orderItems
                };

                // Add the order to the database
                _context.Orders.Add(order);
                // Mark the customer's active cart as checked out (if it exists).
                //var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == orderCreateDTO.CustomerId && !c.IsCheckOut);

                if(cart != null)
                {
                    cart.IsCheckOut = true;
                    cart.UpdatedAt = DateTime.UtcNow;
                    _context.Carts.Update(cart);
                }

                // Save all the changes.
                await _context.SaveChangesAsync();

                // Map the saved order to OrderResponseDTO
                var orderResponse = MapOrderToDTO(order, customer, billingAddress, shippingAddress);
                return new ApiResponse<OrderResponseDTO>(200,orderResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        // Retrieve an order by it's ID along with related entities
        public async Task<ApiResponse<OrderResponseDTO>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                // Retrieve the order with it's items, customer, and addresses details.

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Include(o => o.Customer)
                    .Include(o => o.BillingAddress)
                    .Include(o => o.ShippingAddress)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if(order == null)
                {
                    return new ApiResponse<OrderResponseDTO>(404, "Order not found.");
                }

                // Map the order to a DTO
                var orderResponse = MapOrderToDTO(order, order.Customer, order.BillingAddress, order.ShippingAddress);

                return new ApiResponse<OrderResponseDTO>(200, orderResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderResponseDTO>(500,$"An unexpected error occurred while processing your request, Error: {ex.Message}" );
            }
        }
        
        // Updates the status of an existing order.
        // Validates allowed status transitions before applying the update

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateOrderStatusAsync(OrderStatusUpdateDTO orderStatusUpdateDTO)
        {
            try
            {
                // Retrieve the order 
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderStatusUpdateDTO.OrderId);

                if( order == null )
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Order not found.");
                }

                var currentStatus = order.OrderStatus;
                var newStatus = orderStatusUpdateDTO.OrderStatus;

                // Validate the status transition.

                if(!AllowedStatusTransitions.TryGetValue(currentStatus,out var allowedStatuses))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(500, "Current order status is invalid.");
                }

                if(!allowedStatuses.Contains(newStatus))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, $"Can't change order status from {currentStatus} to {newStatus}.");
                }

                // Update the order status.
                order.OrderStatus = newStatus;
                await _context.SaveChangesAsync();

                // Prepare a confirmation message

                var confirmation = new ConfirmationResponseDTO
                {
                    Message = $"Order Status with Id {orderStatusUpdateDTO.OrderId} updated successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }

        // Retrieves all orders in the system.
        public async Task<ApiResponse<List<OrderResponseDTO>>> GetAllOrdersAsync()
        {
            try
            {
                // REtrieves all orders with related entities.

                var orders = await _context.Orders
                    .Include(o =>  o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Include(o => o.Customer)
                    .Include(o => o.ShippingAddress)
                    .Include(o => o.BillingAddress)
                    .AsNoTracking()
                    .ToListAsync(); 

                // Map each order to it's corresponding DTO.
                var orderList = orders.Select(o => MapOrderToDTO(o,o.Customer,o.BillingAddress,o.ShippingAddress)).ToList();

                return new ApiResponse<List<OrderResponseDTO>>(200, orderList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<OrderResponseDTO>>(500, $"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }

        // Retrieves all orders associated with a specific customer.

        public async Task<ApiResponse<List<OrderResponseDTO>>> GetOrdersByCustomerAsync(int customerId)
        {
            try
            {
                // Retrieves the customer along with their orders and related data.
                var customer = await _context.Customers
                    .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Include(c => c.Addresses)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == customerId);  

                if(customer == null)
                {
                    return new ApiResponse<List<OrderResponseDTO>>(404, "Customer not found.");
                }

                // Map each order to a DTO.

                var orders = customer.Orders.Select(o => MapOrderToDTO(o,customer,o.BillingAddress,o.ShippingAddress)).ToList();   
                return new ApiResponse<List<OrderResponseDTO>>(200,orders);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<OrderResponseDTO>>(500, $"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }

        #region Helper Methods
        // Maps an Order entity to an OrderResponseDTO.
        private OrderResponseDTO MapOrderToDTO(Order order,Customer customer,Address billingAddress, Address shippingAddress)
        {
            // Map order items.
            var orderItemDTO = order.OrderItems.Select(oi => new OrderItemResponseDTO
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Discount = oi.Discount,
                TotalPrice = oi.TotalPrice
            }).ToList();

            // Create and return DTO
            return new OrderResponseDTO
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                BillingAddressId = order.BillingAddressId,
                ShippingAddressId = order.ShippingAddressId,
                TotalBaseAmount = order.TotalBaseAmount,
                TotalDiscountAmount = order.TotalDiscountAmount,
                ShippingCost = order.ShippingCost,
                TotalAmount = Math.Round(order.TotalAmount, 2),
                OrderStatus = order.OrderStatus,
                OrderItems = orderItemDTO
            };
        }
        // Generates a unique order number using the current UTC date/time and a random number.
        // Format: ORD-yyyyMMdd-HHmmss-XXXX
        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")}-{RandomNumber(1000, 9999)} ";
        }

        // Generates a random number between min and max.
        private int RandomNumber(int min,int max)
        {
            using(var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return Math.Abs(BitConverter.ToInt32(bytes, 0) % (max - min + 1)) + min;
            }
        }
#endregion
    }
}
