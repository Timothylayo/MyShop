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

namespace MyShop.Repository;

public class ClientRepository(ApplicationDbContext applicationDb, IConfiguration configuration) : IClientRepository
{
    private readonly ApplicationDbContext _applicationDb = applicationDb;
    private readonly IConfiguration configuration = configuration;


    //Products
    public async Task<List<Product>> GetAllProductAsync()
    {
        var products = await _applicationDb.Products.AsNoTracking()
                                                    .Include(c => c.Category)
                                                    .Where(c => c.IsActive)
                                                    .ToListAsync();
        return products;
    }
    public async Task<Product?> GetProductByIdAsync(string Id)
    {
        var product = await _applicationDb.Products
                                    .AsNoTracking()
                                    .Include(b => b.Category)
                                    .FirstOrDefaultAsync(b => b.ProdNum == Id);
        product!.ViewCount++;
        await _applicationDb.SaveChangesAsync();
        return product;
    }

    public async Task<List<Product>> GetFeaturedProductsAsync()
    {
        var query = await _applicationDb.Products
                            .AsNoTracking()
                            .Include(p => p.Category)
                            .Where(p => p.Featured)
                            .Where(p => p.CreatedDate.AddDays(30) > DateTime.Now)
                            .Where(p => p.IsActive)
                            .ToListAsync();

        return query;
    }

    public async Task<List<Product>> GetNewArrival()
    {
        var newArrival = await _applicationDb.Products
                            .AsNoTracking()
                            .Include(p => p.Category)
                            .Where(p => p.Featured)
                            .Where(p => p.IsActive)
                            .ToListAsync();

        return newArrival;
    }

    public async Task<List<Product>> GetProductByCategory(int categoryId)
    {
        var byCategory = await _applicationDb.Products.AsNoTracking()
                                                      .Where(p => p.CategoryId == categoryId)
                                                      .Where(p => p.IsActive)
                                                      .ToListAsync();
        return byCategory;
    }

    public async Task<Product> GetProductDescription(string name)
    {
        var products = await _applicationDb.Products.AsNoTracking()
                                              .Include(p => p.Category)
                                              .FirstOrDefaultAsync(p => p.Name == name && p.IsActive);
        return products!;
    }

    public async Task<List<Product>> GetProductByTextInput(string text)
    {
        return await _applicationDb.Products
                            .Where(p => p.Name!.Contains(text) || p.Description!.Contains(text))
                            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsByQuery(string? query = null)
    {
        return await _applicationDb.Products
        .Where(p => p.Name!.Contains(query!, StringComparison.OrdinalIgnoreCase) == true || p.Description!.Contains(query!, StringComparison.OrdinalIgnoreCase) == true)
        .ToListAsync();
    }


    //Category
    public async Task<List<Category>> GetCategoryAsync()
    {
        var categories = await _applicationDb.Categories
                                        .AsNoTracking()
                                        .ToListAsync();
        return categories;
    }

    public async Task<Category> GetCategoryAsyncById(int Id)
    {
        var categories = await _applicationDb.Categories
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(p => p.Id == Id);
        return categories!;
    }

    //Cart 
    public string? ShoppingCartID { get; set; }

    public async Task<ServiceResponse> AddToCart(int id)
    {
        var user = DecryptJwtToken.GetClaims(JWTModel.JWTToken);
        ShoppingCartID = user.id;

        var cartItem = await _applicationDb.CartItems.FirstOrDefaultAsync(
            c => c.CartId == ShoppingCartID
            && c.ProductId == id);

        if (cartItem is null)
        {
            cartItem = new CartItems
            {
                CartId = ShoppingCartID,
                ProductId = id,
                Product = _applicationDb.Products.FirstOrDefault(
                    p => p.Id == id),
                Quantity = 1,
                DateCreated = DateTime.Now,
            };
            _applicationDb.CartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity++;
        }
        await _applicationDb.SaveChangesAsync();
        return new ServiceResponse(true, "Product Added To Cart");
    }

    public List<CartItemDto> GetCartItems()
    {
        List<CartItemDto> items = [];
        var user = DecryptJwtToken.GetClaims(JWTModel.JWTToken);

        ShoppingCartID = user.id;

        var cartitems = _applicationDb.CartItems.Where(
            c => c.CartId == ShoppingCartID).ToList();
        foreach (var cartitem in cartitems)
        {
            var products = _applicationDb.Products.Where(c => c.Id == cartitem.ProductId).FirstOrDefault();
            CartItemDto itemDto = new()
            {
                ProductId = cartitem.ProductId,
                ProductName = products!.Name,
                ProductDescription = products.Description,
                ProduuctImageUrl = products.ImageUrl!,
                CartId = cartitem.CartId!,
                Price = products.Price,
                Qty = cartitem.Quantity,
                AvailableStock = products.Quantity,
            };
            items.Add(itemDto);
        }
        return items;
    }

    public async Task<ServiceResponse> RemoveItem(string removeCartID, int removeProductID)
    {
        try
        {
            var myItem = await (from c in _applicationDb.CartItems
                                where c.CartId == removeCartID && c.ProductId == removeProductID
                                select c).FirstOrDefaultAsync();
            if (myItem != null)
            {
                //Remove Item.
                _applicationDb.CartItems.Remove(myItem);
                await _applicationDb.SaveChangesAsync();
            }
            return new ServiceResponse(true, "Item Removed Successfully");
        }
        catch (Exception exp)
        {
            throw new Exception("Error: Unable to Remove Cart Item - " + exp.Message.ToString(), exp);
        }
    }

    public async Task<ServiceResponse> UpdateItem(CartItemDto cartItem)
    {
        string cartId = GetCartId();
        try
        {
            var myItem = await (from c in _applicationDb.CartItems
                                where c.CartId == cartId
                                && c.ProductId == cartItem.ProductId
                                select c).FirstOrDefaultAsync();
            if (myItem != null)
            {
                myItem.Quantity = cartItem.Qty;
                await _applicationDb.SaveChangesAsync();
            }
            return new ServiceResponse(true, "Item Updated Successfully");

        }
        catch (Exception exp)
        {
            throw new Exception("ERROR: Unable to Update Cart Item - " + exp.Message.ToString(), exp);
        }
    }

    public int GetCount()
    {
        ShoppingCartID = GetCartId();

        //Get the count od each item in the cart and sum them up
        int? Count = (from cartItems in _applicationDb.CartItems
                      where cartItems.CartId == ShoppingCartID
                      select (int?)cartItems.Quantity).Sum();
        //Return 0 is all entries are null
        return Count ?? 0;
    }

    public async Task<ServiceResponse> EmptyCart()
    {
        ShoppingCartID = GetCartId();
        var cartItems = _applicationDb.CartItems.Where(
            c => c.CartId == ShoppingCartID);
        foreach (var cartitem in cartItems)
        {
            _applicationDb.Remove(cartitem);
        }
        await _applicationDb.SaveChangesAsync();
        return new ServiceResponse(true, "CartItems Deleted");
    }

    public string GetCartId()
    {
        string userId;
        if (JWTModel.JWTToken == null)
        {
            return string.Empty;
        }
        else
        {
            var user = DecryptJwtToken.GetClaims(JWTModel.JWTToken);
            userId = user.id;
        }
        return userId;
    }

    //Email
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        //var emailToSend = new MimeMessage();
        //emailToSend.From.Add(MailboxAddress.Parse("Your Gmail Id"));
        //emailToSend.To.Add(MailboxAddress.Parse(email));
        //emailToSend.Subject = subject;
        //emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        //{
        //    Text = htmlMessage
        //};
        //using (var client = new SmtpClient("smtp-mail.gmail.com", 587))
        //{
        //    client.Send();
        //    client.Dispose();
        //};

        throw new NotImplementedException();

    }

    //User 

    public async Task<ServiceResponse> RegisterUserAsync(RegisterDTO registerDTO)
    {
        if (registerDTO is null)
            return new ServiceResponse(false, "Model is null");
        var getUser = await FindUserByEmail(registerDTO.Email!);
        if (getUser != null) return new ServiceResponse(false, "User Exists");
        var checkIfAdminCreated = await _applicationDb.Users.
            FirstOrDefaultAsync(_ => _.Role!.ToLower().Equals("Admin"));

        if (checkIfAdminCreated is null)
        {
            var result = _applicationDb.Users.Add(new ApplicationUsers()
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                UserName = registerDTO.FirstName!.ToLower() + registerDTO.LastName!.ToLower(),
                Email = registerDTO.Email,
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),

            }).Entity;
            await Commit();

        }
        else
        {
            var checkIfUserIsCreated = await _applicationDb.Users.
                FirstOrDefaultAsync(_ => _.Role!.ToLower().Equals("user"));
            var result = _applicationDb.Users.Add(new ApplicationUsers()
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                UserName = registerDTO.FirstName!.ToLower() + registerDTO.LastName!.ToLower(),
                Email = registerDTO.Email,
                Role = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),

            }).Entity;
            await Commit();
        }
        return new ServiceResponse(true, "Registration successful");
    }

    public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO)
    {
        if (loginDTO is null)
            return new LoginResponse(false, "Model is Empty");

        var getUser = await FindUserByEmail(loginDTO.Email!);
        if (getUser == null) return new LoginResponse(false, "User not found");

        bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.PasswordHash);
        if (checkPassword)
            return new LoginResponse(true, "Login successfully", GenerateJWtToken(getUser));
        else
            return new LoginResponse(false, "Email or Password Incorrect");
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

    public async Task<UserDTO> GetUserDetails(string userKey)
    {
        var user = await _applicationDb.Users
            .Where(_ => _.Id == userKey)
            .FirstOrDefaultAsync();
        if (user is null) return null!;
        return new UserDTO()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            Email = user.Email,
            Address = user.Address,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            UserName = user.UserName,
        };

    }

    public async Task<ServiceResponse> UpdateUserDetails(UserDTO user)
    {

        if (user is null) return null!;
        var userDto = await _applicationDb.Users.FindAsync(user.Id);
        if (userDto != null)
        {
            userDto.Id = user.Id!;
            userDto.PhoneNumber = user.PhoneNumber;
            userDto.Address = user.Address;

            await Commit();
            return new ServiceResponse(true, "UserDetails is updated");
        }
        else
        {
            return new ServiceResponse(false, "User not found");
        }

    }

    public ServiceResponse ChangePassword(PasswordModel password)
    {
        if (password == null) return new ServiceResponse(false, "model is null");
        var user = _applicationDb.Users.Where(x => x.Id == password.UserKey)
                                              .FirstOrDefault();
        if (user != null)
        {

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password.NewPassword);
            _applicationDb.Users.Update(user);
            _applicationDb.SaveChanges();
            return new ServiceResponse(true, "Password Updated Successfully !!");
        }
        else
        {
            return new ServiceResponse(false, "User does not Exist. Try again !!");
        }
    }

    //Orders
    public List<string> GetShippingStatusForOrder(string order_number)
    {
        List<string> shipping_status = [];
        var order = _applicationDb.Orders.Where(x => x.OrderId == order_number).FirstOrDefault();
        if (order != null && order.ShippingStatus != null)
        {
            shipping_status = [.. order.ShippingStatus.Split("|")];
        }
        return shipping_status;
    }

    public List<CartItemDto> GetOrderDetailForCustomer(string customerId, string order_number)
    {
        List<CartItemDto> cartItems = [];

        var customerOrder = _applicationDb.Orders.Where(x => x.UserId == customerId && x.OrderId == order_number)
                                                 .FirstOrDefault();

        if (customerOrder != null)
        {
            var orderDetail = _applicationDb.OrderItems.Where(x => x.OrderId == order_number).ToList();
            var productList = _applicationDb.Products.ToList();
            var user = _applicationDb.Users.Where(x => x.Id == customerId)
                                                  .FirstOrDefault();

            foreach (var order in orderDetail)
            {
                var product = productList.Where(x => x.Id == order.ProductId)
                                         .FirstOrDefault();
                CartItemDto cartItem = new()
                {
                    ProductName = product!.Name,
                    Price = product!.Price,
                    ProduuctImageUrl = product.ImageUrl,
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

    public List<CartItemDto> GetOrderDetails(string order_number)
    {
        List<CartItemDto> cartItems = [];

        var customerOrder = _applicationDb.Orders.Where(x => x.OrderId == order_number).FirstOrDefault();

        if (customerOrder != null)
        {
            var orderDetail = _applicationDb.OrderItems.Where(x => x.OrderId == order_number).ToList();
            var productList = _applicationDb.Products.ToList();
            var user = _applicationDb.Users.Where(x => x.Id == customerOrder.UserId)
                                                  .FirstOrDefault();

            foreach (var order in orderDetail)
            {
                var product = productList.Where(x => x.Id == order.ProductId)
                                         .FirstOrDefault();
                CartItemDto cartItem = new()
                {
                    ProductName = product!.Name,
                    Price = product!.Price,
                    ProduuctImageUrl = product.ImageUrl,
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

    public async Task<ServiceResponse> UpdateShippingStatus(OrderDTO orderDTO)
    {
        if (orderDTO is null) return null!;
        var order = await _applicationDb.Orders.Where(x => x.OrderId == orderDTO.OrderId).FirstOrDefaultAsync();
        if (order != null)
        {
            order.OrderId = orderDTO.OrderId;
            order.ShippingStatus = orderDTO.ShippingStatus;
            await Commit();
            return new ServiceResponse(true, "Shipping Status is updated");
        }
        else
        {
            return new ServiceResponse(false, "Error Occurred!! Try again");
        }
    }

    public List<OrderDTO> GetOrdersByCustomerId(string customerId)
    {
        List<OrderDTO> orders = [];
        var order = _applicationDb.Orders
                                                .Where(x => x.UserId == customerId)
                                                .OrderByDescending(x => x.Id)
                                                .ToList();
        foreach (var item in order)
        {
            var user = _applicationDb.Users.Where(x => x.Id == customerId)
                                                  .FirstOrDefault();
            OrderDTO orderDTO = new()
            {
                UserId = item.UserId,
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

    public async Task<ServiceResponse> Checkout(List<CartItemDto> cartItems)
    {
        string orderId = GenerateOrderId();
        var product = _applicationDb.Products.ToList();
        var user = DecryptJwtToken.GetClaims(JWTModel.JWTToken);
        var userDetails = await GetUserDetails(user.id);
        try
        {
            var detail = cartItems.FirstOrDefault();
            OrderDetail order = new()
            {
                OrderId = orderId,
                UserId = detail!.CartId,
                PaymentMode = detail.PaymentMode,
                ShippingCharges = detail.ShippingCharges,
                ShippingAddress = userDetails.Address,
                SubTotal = detail.SubTotal,
                TotalPrice = detail.ShippingCharges + detail.SubTotal,
                CreatedDate = DateTime.Now.ToString("dd/MM/yyyy"),
                UpdatedOn = DateTime.Now.ToString("dd/MM/yyyy"),
            };
            _applicationDb.Orders.Add(order);
            foreach (var items in cartItems)
            {
                OrderItems orderItem = new()
                {
                    OrderId = orderId,
                    ProductId = items.ProductId,
                    Quantity = items.Qty,
                    Price = items.Price,
                    SubTotal = items.Price * items.Qty,
                    CreatedDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    UpdatedOn = DateTime.Now.ToString("dd/MM/yyyy"),
                };
                _applicationDb.OrderItems.Add(orderItem);

                var selectedProduct = product.Where(x => x.Id == orderItem.ProductId).FirstOrDefault();
                selectedProduct!.Quantity = selectedProduct.Quantity - items.Qty;
                _applicationDb.Products.Update(selectedProduct);
            }
            await Commit();
            return new ServiceResponse(true, "Order Placed");

        }
        catch (Exception)
        {
            throw;
        }
    }

    public List<OrderDTO> GetOrders()
    {
        List<OrderDTO> orders = [];
        var order = _applicationDb.Orders.OrderByDescending(x => x.Id)
                                         .ToList();
        foreach (var item in order)
        {
            var user = _applicationDb.Users.Where(x => x.Id == item.UserId)
                                                  .FirstOrDefault();
            OrderDTO orderDTO = new()
            {
                UserId = item.UserId,
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

    //JWT Generation

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
            expires: DateTime.Now.AddDays(2),
            signingCredentials: credientials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    //Privated methods
    private async Task Commit() => await _applicationDb.SaveChangesAsync();
    private async Task<ApplicationUsers?> FindUserByEmail(string email) =>
            await _applicationDb.Users.FirstOrDefaultAsync(u => u.Email == email);

    private string GenerateOrderId()
    {
        string OrderId = string.Empty;
        Random num = null!;
        for (int i = 0; i < 1000; i++)
        {
            num = new Random();
            OrderId = "T" + num.Next(1, 10000000).ToString("D8");
            if (!_applicationDb.OrderItems.Where(x => x.OrderId == OrderId).Any())
            {
                break;
            }

        }
        return OrderId;
    }


}
