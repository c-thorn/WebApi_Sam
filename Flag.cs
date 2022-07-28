using System.Collections.Generic;

namespace KRealm_WebApi.Models
{
    public class Flag
    {
        internal Product AddFlag(Product newFlag)
        {
            Product p = new Product();
            return p.AddProduct( newFlag );
        }

        internal List<Product> GetFlags()
        {
            Product p = new Product();
            return p.GetProducts( "flag" );
        }

        internal Product GetFlag( int number )
        {
            Product p = new Product();
            return p.GetProduct( number, "flag" );
        }

        internal Product UpdateFlag( int number, Product updatedFlag )
        {
            Product p = new Product();
            return p.UpdateProduct( number, updatedFlag );
        }

        internal bool DeleteFlag(int number)
        {
            Product p = new Product();
            return p.DeleteProduct( number, "flag" );
        }
    }
}