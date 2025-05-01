using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Repository;
using MyShopClassLibrary.DTOs;
using MyShopClassLibrary.Models;
using MyShopClassLibrary.Response;

namespace MyShop.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AdminController(IAdminRepository adminRepository) : ControllerBase
{
    private readonly IAdminRepository _adminRepository = adminRepository;

    //Category
    [HttpGet("getcategories")]
    [AllowAnonymous]
    public async Task<ActionResult<Category[]>> GetCategoriesAsync()
    {
        var categories = await _adminRepository.GetCategoryAsync();
        return Ok(categories);
    }

    [HttpPost("addcategory")]
    public async Task<ActionResult<ServiceResponse>> AddCategoryAsync(Category catgeory)
    {
        if (catgeory is null) return BadRequest("Category is null");
        var response = await _adminRepository.AddCatgeoryAsync(catgeory);
        return Ok(response);
    }

    [HttpDelete("deletecategory/{Id}")]
    public async Task<ActionResult<Category>> DeleteCategory(int Id)
    {
        var deleteCategory = await _adminRepository.DeleteCategory(Id);
        return Ok(deleteCategory);
    }

    [HttpPost("refresh-token")]
    public ActionResult<LoginResponse> RefreshToken(UserSession model)
    {
        var result = _adminRepository.RefreshToken(model);
        return Ok(result);
    }

    //Product
    [HttpGet("getproducts")]
    public async Task<ActionResult> GetProductsAsync()
    {
        return Ok(await _adminRepository.GetAllProductAsync());
    }

    [HttpGet("getproductbyid/{Id}")]
    public async Task<ActionResult> GetProductById(string Id)
    {
        return Ok(await _adminRepository.GetProductByIdAsync(Id));
    }

    [HttpPost("addproduct")]
    public async Task<ActionResult<ServiceResponse>> AddProductAsync(Product product)
    {
        return Ok(await _adminRepository.AddProductAsync(product));
    }

    [HttpDelete("deleteproduct/{Id}")]
    public async Task<ActionResult<ServiceResponse>> DeleteProduct(string Id)
    {
        var deleteProduct = await _adminRepository.DeleteProduct(Id);
        return Ok(deleteProduct);
    }

    [HttpPut("updateStock")]
    public ActionResult UpdateStock(StockModel stock)
    {
        return Ok(_adminRepository.UpdateStock(stock));
    }

    [HttpGet("getstock")]
    public ActionResult GetStockAsync()
    {
        return Ok(_adminRepository.GetProductStock());
    }

    [HttpPut("updateproduct")]
    public async Task<ActionResult<ServiceResponse>> UpdateProductAsync(Product model)
    {
        return Ok(await _adminRepository.UpdateProduct(model));
    }

    //Other Services
    [HttpGet("getorderdetails/{orderNumber}")]
    public ActionResult GetOrderDetails(string orderNumber)
    {
        return Ok(_adminRepository.GetOrderDetails(orderNumber));
    }

    [HttpGet("getorders")]
    public ActionResult GetOrdersByCustomerId()
    {
        return Ok(_adminRepository.GetOrders());
    }
}
