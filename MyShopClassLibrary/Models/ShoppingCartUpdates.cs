namespace MyShopClassLibrary.Models
{
    class ShoppingCartUpdates
    {
        public struct ShoppingCartUpdate
        {
            public int ProductID;
            public int PurchaseQuantity;
            public bool RemoveItem;
        }
    }
}
