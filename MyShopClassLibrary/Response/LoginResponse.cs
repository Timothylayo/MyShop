namespace MyShopClassLibrary.Response
{
    public record LoginResponse(bool Flag, string Message = null!, string Token = null!);
}
