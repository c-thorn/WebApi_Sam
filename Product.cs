using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KRealm_WebApi.Models
{
    public class Product
    {
        private string connStr { get; set; }
        public int productID { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string type { get; set; }
        public string photoLoc { get; set; }
        public int qty { get; set; }
        public float price { get; set; }
        public bool active { get; set; }

        public Product() { connStr = System.Configuration.ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString; }

        internal Product AddProduct( Product newProduct )
        {
            Product p = null;

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                string commandText = "INSERT INTO Products (name,desc,photoLoc,qty,price,type,active) VALUES (@na1,@de,@ph,@qt,@pr,@ty1,1); " +
                    "SELECT productID,name,desc,photoLoc,qty,price FROM Products WHERE name=@na2 && active=1 && type=@ty2";
                SqlCommand sComm = new SqlCommand( commandText, sConn );
                sComm.Parameters.AddRange( new SqlParameter[] { new SqlParameter( "na1", newProduct.name ), new SqlParameter( "de", newProduct.desc ),
                    new SqlParameter( "ph", newProduct.photoLoc ), new SqlParameter( "qt", newProduct.qty ), new SqlParameter( "pr", newProduct.price ),
                    new SqlParameter( "ty1", newProduct.type ), new SqlParameter( "na2", newProduct.name ), new SqlParameter( "ty2", newProduct.type ) } );

                using( SqlDataReader sdr = sComm.ExecuteReader() )
                {
                    if( sdr.HasRows )
                    {
                        while( sdr.Read() )
                        {
                            p.productID = (int) sdr["productID"];
                            p.name = sdr["name"].ToString();
                            p.desc = sdr["desc"].ToString();
                            p.photoLoc = sdr["photoLoc"].ToString();
                            p.qty = (int) sdr["qty"];
                            p.price = (float) sdr["price"];
                        }
                    }
                }
            }

            return p;
        }

        internal List<Product> GetProducts( string type )
        {
            List<Product> products = new List<Product>();

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                SqlCommand sComm = new SqlCommand( "SELECT productID,name,desc,photoLoc,qty,price FROM Products WHERE type=@type && active=1", sConn );
                sComm.Parameters.AddWithValue( "type", type );

                using( SqlDataReader sdr = sComm.ExecuteReader() )
                {
                    if( sdr.HasRows )
                    {
                        while( sdr.Read() )
                        {
                            Product p = new Product
                            {
                                productID = (int) sdr["productID"],
                                name = sdr["name"].ToString(),
                                desc = sdr["desc"].ToString(),
                                photoLoc = sdr["photoLoc"].ToString(),
                                qty = (int) sdr["qty"],
                                price = (float) sdr["price"]
                            };

                            products.Add( p );
                        }
                    }
                }
            }

            return products;
        }

        internal Product GetProduct( int number, string type )
        {
            Product products = null;

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                SqlCommand sComm = new SqlCommand( "SELECT productID,name,desc,photoLoc,qty,price FROM Products WHERE type=@type && active=1 && id=@number", sConn );
                sComm.Parameters.AddWithValue( "type", type );
                sComm.Parameters.AddWithValue( "number", number );

                using( SqlDataReader sdr = sComm.ExecuteReader() )
                {
                    if( sdr.HasRows )
                    {
                        while( sdr.Read() )
                        {
                            products.productID = (int) sdr["productID"];
                            products.name = sdr["name"].ToString();
                            products.desc = sdr["desc"].ToString();
                            products.photoLoc = sdr["photoLoc"].ToString();
                            products.qty = (int) sdr["qty"];
                            products.price = (float) sdr["price"];
                        }
                    }
                }
            }

            return products;
        }

        internal Product UpdateProduct( int productID, Product updated )
        {
            Product p = null;

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                string commandText = "UPDATE Products SET name=@na,desc=@de,photoLoc=@ph,qty=@qt,price=@pr,active=@ac WHERE productID=@pid1 && type=@ty1; " +
                    "SELECT productID,name,desc,photoLoc,qty,price FROM Products WHERE type=@type2 && active=1 && productID=@pid2";
                SqlCommand sComm = new SqlCommand( commandText, sConn );
                sComm.Parameters.AddRange( new SqlParameter[] { new SqlParameter( "na", updated.name ), new SqlParameter( "de", updated.desc ), new SqlParameter( "ph", updated.photoLoc ),
                    new SqlParameter( "qt", updated.qty ), new SqlParameter( "pr", updated.price ), new SqlParameter( "ac", updated.active == true ? 1 : 0 ),
                    new SqlParameter( "pid1", productID ), new SqlParameter( "ty1", updated.type ), new SqlParameter( "id2", productID ), new SqlParameter( "ty2", updated.type ) } );

                using( SqlDataReader sdr = sComm.ExecuteReader() )
                {
                    if( sdr.HasRows )
                    {
                        while( sdr.Read() )
                        {
                            p.productID = (int) sdr["productID"];
                            p.name = sdr["name"].ToString();
                            p.desc = sdr["desc"].ToString();
                            p.photoLoc = sdr["photoLoc"].ToString();
                            p.qty = (int) sdr["qty"];
                            p.price = (float) sdr["price"];
                        }
                    }
                }
            }

            return p;
        }

        internal bool DeleteProduct( int number, string type )
        {
            bool productWasFound = false;

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                SqlCommand sComm = new SqlCommand( "UPDATE Product SET active=0 WHERE productID=@id && type=@type", sConn );
                sComm.Parameters.AddWithValue( "id", number );
                sComm.Parameters.AddWithValue( "type", type );
                int result = sComm.ExecuteNonQuery();
                productWasFound = result > 0 ? true : false;
            }

            return productWasFound;
        }

        internal List<InventoryRecordSimplified> GetProductQuantities(List<int> productIDs)
        {
            List<InventoryRecordSimplified> requested = new List<InventoryRecordSimplified>();
            StringBuilder sb = new StringBuilder();

            foreach( int id in productIDs)
            {
                if( sb.Length > 0 )
                    sb.Append( ", " );

                sb.Append( id );
            }

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                SqlCommand sComm = new SqlCommand( "SELECT productID, qty FROM Products WHERE productID in ( @ids )", sConn );
                sComm.Parameters.AddWithValue( "ids", sb.ToString() );

                using( SqlDataReader sdr = sComm.ExecuteReader() )
                {
                    while(sdr.Read())
                    {
                        requested.Add( new InventoryRecordSimplified() { productID = int.Parse(sdr[0].ToString()), qty = int.Parse(sdr[1].ToString()) } );
                    }
                }
            }
        
            return requested;
        }
    }
}