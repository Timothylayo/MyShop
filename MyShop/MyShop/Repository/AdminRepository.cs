using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyShop.Data;
using MyShop.Data.Entities;
using MyShopClassLibrary;
using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyShop.Repository
{
    public class AdminRepository(ApplicationDbContext applicationDb, IConfiguration configuration) : IAdminRepository
    {
        private readonly ApplicationDbContext _dbContext = applicationDb;


        //Products
        public async Task<Product?> GetProductByIdAsync(string prodId) =>
                            await _dbContext.Products
                                    .AsNoTracking()
                                    .Include(b => b.Category)
                                    .FirstOrDefaultAsync(b => b.ProdNum == prodId);
        public async Task<ServiceResponse> AddProductAsync(Product product)
        {
            if (product is null) return null!;
            var duplicateCheck = await _dbContext.Products
                                        .AnyAsync(c => c.Name == product.Name);
            if (duplicateCheck)
            {
                throw new InvalidOperationException($"Category with the name {product.Name} exists");
            }
            if (product.IsActive)
            {
                product.CreatedDate = DateTime.UtcNow;
            }
            product.ProdNum = GenerateUniqueProductId();
            var newProducts = _dbContext.Products.Add(product).Entity;
            await _dbContext.SaveChangesAsync();
            return new ServiceResponse(true, "Product Saved"); ;
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            var query = await _dbContext.Products
                                        .AsNoTracking()
                                        .Include(_ => _.Category)
                                        .OrderBy(_ => _.Id)
                                        .ToListAsync();
            return query;
        }

        public async Task<ServiceResponse> DeleteProduct(string Id)
        {
            var products = await GetProductByIdAsync(Id);
            if (products == null)
                return null!;
            _dbContext.Products.Remove(products);
            await _dbContext.SaveChangesAsync();
            return new ServiceResponse(true, "Product Deleted");
        }

        public async Task<ServiceResponse> UpdateProduct(Product model)
        {
            var product = await _dbContext.Products.FindAsync(model.Id);

            product!.Name = model.Name;
            product!.Description = model.Description;
            product.CategoryId = model.CategoryId;
            product.OriginalPrice = model.OriginalPrice;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            product.ImageUrl = model.ImageUrl;
            product.PublishedAt = model.PublishedAt;
            product.IsActive = model.IsActive;
            if (product.IsActive)
            {
                if (!product.IsActive)
                    product.PublishedAt = DateTime.UtcNow;
            }
            else
            {
                product.PublishedAt = null;
            }
            await _dbContext.SaveChangesAsync();
            return new ServiceResponse(true, "Product Saved");
        }

        public List<StockModel> GetProductStock()
        {
            List<StockModel> stocks = [];

            //List<Category> categories = [.. _dbContext.Categories];
            List<Product> products = _dbContext.Products.Include(c => c.Category).ToList();
            foreach (var product in products)
            {
                StockModel stock = new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Stock = product.Quantity,
                    CategoryName = product.Category!.Name
                };
                stocks.Add(stock);
            }
            return stocks;

        }

        public bool UpdateStock(StockModel model)
        {
            bool flag = false;
            var product = _dbContext.Products.Where(x => x.Id == model.Id).FirstOrDefault();
            if (product != null)
            {
                product.Quantity = model.Stock + model.NewStock;
                _dbContext.Products.Update(product);
                _dbContext.SaveChanges();
                flag = true;

            }
            return flag;
        }

        //Category
        public async Task<List<Category>> GetCategoryAsync()
        {
            var categories = await _dbContext.Categories
                                            .AsNoTracking()
                                            .ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategoryAsyncById(int Id)
        {
            var categories = await _dbContext.Categories
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(p => p.Id == Id);
            return categories!;
        }
        public async Task<ServiceResponse> AddCatgeoryAsync(Category model)
        {
            if (model.Id == 0)
            {
                if (await _dbContext.Categories.AsNoTracking().AnyAsync(c => c.Name == model.Name))
                {
                    throw new InvalidOperationException($"Category with the name {model.Name} exists");
                }

                await _dbContext.Categories.AddAsync(model);
            }
            else
            {
                if (await _dbContext.Categories.AsNoTracking().AnyAsync(c => c.Name == model.Name))
                {
                    throw new InvalidOperationException($"Category with the name {model.Name} exists");
                }
                var dbCategory = await _dbContext.Categories
                                       .FindAsync(model.Id);
                dbCategory!.Name = model.Name;
            }


            await _dbContext.SaveChangesAsync();

            return new ServiceResponse(true, "Catgeory Saved");
        }

        public async Task<ServiceResponse> DeleteCategory(int id)
        {
            var categoryToBeDeleted = await GetCategoryAsyncById(id);

            if (categoryToBeDeleted == null)
                return null!;
            _dbContext.Categories.Remove(categoryToBeDeleted);
            await _dbContext.SaveChangesAsync();
            return new ServiceResponse(true, "Category Deleted");
        }

        //Orders
        public List<CartItemDto> GetOrderDetails(string order_number)
        {
            List<CartItemDto> cartItems = [];

            var customerOrder = _dbContext.Orders.Where(x => x.OrderId == order_number).FirstOrDefault();

            if (customerOrder != null)
            {
                var orderDetail = _dbContext.OrderItems.Where(x => x.OrderId == order_number).ToList();
                var productList = _dbContext.Products.ToList();
                var user = _dbContext.Users.Where(x => x.Id == customerOrder.UserId)
                                                      .FirstOrDefault();

                foreach (var order in orderDetail)
                {
                    var product = productList.Where(x => x.Id == order.ProductId)
                                             .FirstOrDefault();
                    CartItemDto cartItem = new()
                    {
                        ProductName = product!.Name,
                        Price = product!.Price,
                        ProduuctImageUrl = product.ImageUrl!,
                        Qty = order.Quantity
                    };
                    cartItems.Add(cartItem);
                }

                cartItems.FirstOrDefault()!.ShippingAddress = user!.Address;
                cartItems.FirstOrDefault()!.ShippingCharges = Convert.ToDecimal(customerOrder.ShippingCharges);
                cartItems.FirstOrDefault()!.SubTotal = customerOrder.SubTotal;
                cartItems.FirstOrDefault()!.Total = customerOrder.TotalPrice;
                cartItems.FirstOrDefault()!.PaymentMode = customerOrder.PaymentMode;
            }
            return cartItems;
        }
        public List<OrderDTO> GetOrders()
        {
            List<OrderDTO> orders = [];
            var order = _dbContext.Orders.OrderByDescending(x => x.Id)
                                         .ToList();
            foreach (var item in order)
            {
                var user = _dbContext.Users.Where(x => x.Id == item.UserId)
                                                      .FirstOrDefault();
                OrderDTO orderDTO = new()
                {
                    UserId = item.UserId!,
                    OrderId = item.OrderId,
                    Address = user!.Address,
                    PhoneNumber = user.PhoneNumber,
                    TotalPrice = item.TotalPrice,
                    CreatedDate = item.CreatedDate!
                };
                orders.Add(orderDTO);
            }
            return orders;
        }
        public Task<OrderDTO> GetShippingStatus(string orderId)
        {
            var order = _dbContext.Orders.Where(x => x.OrderId == orderId)
                                         .FirstOrDefault();
            OrderDTO orderDTO = new()
            {
                OrderId = orderId,
                ShippingStatus = order!.ShippingStatus
            };
            return Task.FromResult(orderDTO);
        }

        public Task<ServiceResponse> UpdateShippingStatus(ShippingStatusModel shippingStatus)
        {
            var orderToBeUpdated = _dbContext.Orders.Where(x => x.OrderId == shippingStatus.OrderId)
                                                   .FirstOrDefault();
            if (orderToBeUpdated != null)
            {
                orderToBeUpdated.OrderId = shippingStatus.OrderId;
                orderToBeUpdated.ShippingStatus = shippingStatus.ShippingStatus;
                //_dbContext.Orders.Update(orderToBeUpdated);
                _dbContext.SaveChanges();
            }
            return Task.FromResult(new ServiceResponse(true, "Shipping Status Updated"));
        }
        //Private Methods
        private string GenerateUniqueProductId()
        {
            string productId = null!;
            Random random = null!;
            for (int i = 0; i < 1000; i++)
            {
                random = new Random();
                productId = "P" + random.Next(1, 10000).ToString("D4");
                if (!_dbContext.Products.Where(x => x.ProdNum == productId).Any())
                {
                    break;
                }

            }
            return productId;
        }
        public LoginResponse RefreshToken(UserSession session)
        {
            CustomClaims customClaims = DecryptJwtToken.GetClaims(session.ExipiredJWTToken);
            if (customClaims is null) return new LoginResponse(false, "Incorrect Token");
            string newToken = GenerateJWtToken(new ApplicationUsers()
            {
                Id = customClaims.id,
                FirstName = customClaims.Name,
                Email = customClaims.Email,
                Role = customClaims.Role
            });
            return new LoginResponse(true, "New Token", newToken);
        }

        private string GenerateJWtToken(ApplicationUsers getUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credientials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);



            var UserClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, getUser.Id.ToString()),
                new Claim(ClaimTypes.Name, getUser.FirstName!),
                new Claim(ClaimTypes.Email, getUser.Email!),
                new Claim(ClaimTypes.Role, getUser.Role!),
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: UserClaims,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: credientials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
