using System;
using System.Data.SqlClient;

namespace KRealm_WebApi.Models
{
    public class Order
    {
        private string connStr { get; set; }
        private string username { get; set; }
        private string orderNumber { get; set; }
        private int totalCostOfProductsSold { get; set; }
        private int totalProductsSold { get; set; }
        private float totalOrderAmount { get; set; }
        private float shippingCost { get; set; }
        private int shippingType { get; set; }
        private DateTime orderDate { get; set; }

        public Order() { connStr = System.Configuration.ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString; }

        private Order ProcessOrder( Order requestedOrder )
        {
            return new Order();
        }

        internal bool InsertOrder(Order o)
        {
            int result = -1;

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                string commandText = "INSERT INTO Orders ( orderNumber, orderTotal, orderDate, orderStatus ) VALUES (@on, @ot, @od, @os)";
                SqlCommand sComm = new SqlCommand(commandText,sConn);
                sComm.Parameters.AddRange( new SqlParameter[] { new SqlParameter( "on", Utility.GenerateRandomOrderNumber() ), new SqlParameter("ot", o.totalCostOfProductsSold),
                    new SqlParameter("od", new DateTime()), new SqlParameter("os","Submitted") } );
                result = sComm.ExecuteNonQuery();
            }

            return result > 0 ? true: false;
        }
    }
}