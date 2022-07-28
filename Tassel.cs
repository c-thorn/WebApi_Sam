using System.Collections.Generic;

namespace KRealm_WebApi.Models
{
    public class Tassel
    {
        internal Product AddTassel(Product newTassel)
        {
            Product p = new Product();
            return p.AddProduct( newTassel );
        }

        internal List<Product> GetTassels()
        {
            Product p = new Product();
            return p.GetProducts("tassel");
        }

        internal Product GetTassel(int number)
        {
            Product p = new Product();
            return p.GetProduct( number, "tassel" );
        }

        internal Product UpdateTassel(int productID, Product updatedTassel)
        {
            Product p = new Product();
            return p.UpdateProduct( productID, updatedTassel );
        }

        internal bool DeleteTassel( int number )
        {
            Product p = new Product();
            return p.DeleteProduct( number, "tassel" );
        }
    }
}