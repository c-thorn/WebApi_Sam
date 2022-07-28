using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace KRealm_WebApi.Models
{
    public class Utility
    {
        private string connStr { get; set; }
        private static readonly string key = "QO86enkWeV1qfOmc90h9qZog";
        private static char[] lowers = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static char[] uppers = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static int[] numbers = { 2, 7, 1, 9, 4, 3, 8, 5, 0, 6 };

        public Utility() {  }

        internal static string Encrypt( string input )
        {
            byte[] inputArray = Encoding.UTF8.GetBytes( input );
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = Encoding.UTF8.GetBytes( key );
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock( inputArray, 0, inputArray.Length );
            tripleDES.Clear();
            return Convert.ToBase64String( resultArray, 0, resultArray.Length );
        }

        internal static string Decrypt( string input )
        {
            byte[] inputArray = Convert.FromBase64String( input );
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = Encoding.UTF8.GetBytes( key );
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock( inputArray, 0, inputArray.Length );
            tripleDES.Clear();
            return Encoding.UTF8.GetString( resultArray );
        }

        public void sendEmail(string from, string title, string body)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress( from );
            message.To.Add( new MailAddress( "ToMailAddress" ) );
            message.Subject = title;
            message.Body = body;

            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com"; //for gmail host  
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.SendAsync(message, null);
        }

        public static string GenerateRandomOrderNumber()
        {
            StringBuilder orderNumber = new StringBuilder();

            Random random = new Random();
            int totalSize = random.Next( 10, 19 );

            for(int i = 0; i < totalSize; i++ )
            {
                switch( random.Next( 3 ) )
                {
                    case 0:
                        orderNumber.Append( lowers[random.Next( lowers.Length )] );
                        break;
                    case 1:
                        orderNumber.Append( uppers[random.Next( uppers.Length )] );
                        break;
                    case 2:
                        orderNumber.Append( numbers[random.Next( numbers.Length )] );
                        break;
                    default:
                        orderNumber.Append( uppers[random.Next( uppers.Length )] );
                        break;
                }
            }

            return orderNumber.ToString();
        }

        //this will probably be changed...
        internal void UpdateInventory( List<Product> ordered )
        {
            connStr = System.Configuration.ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            StringBuilder fullUpdateString = new StringBuilder();
            List<SqlParameter> allParams = new List<SqlParameter>();
            List<int> productIDs = new List<int>();

            for( int i = 0; i < ordered.Count; i++ )
            {
                productIDs.Add( ordered[i].productID );
                fullUpdateString.Append( "UPDATE Products SET qty=@q" + i + " WHERE productID=@pi"+ i +";" );
                allParams.Add( new SqlParameter( "pi" + i, ordered[i].productID ) );
                allParams.Add( new SqlParameter( "q" + i, ordered[i].qty ) );
            }

            Product p = new Product();
            List<InventoryRecordSimplified> GetProductInventoryRecords = p.GetProductQuantities( productIDs );
            for( int k = 0; k < GetProductInventoryRecords.Count; k++ )
            {
                int location = -1;

                for(int l = 0; l < allParams.Count; l++ )
                {
                    if( allParams[l].ParameterName.StartsWith("pi") )
                    {
                        if( (int) allParams[l].Value == GetProductInventoryRecords[k].productID )
                        {
                            location = l;
                            break;
                        }
                    }
                }
                location++;

                int newQty = GetProductInventoryRecords[k].qty - (int) allParams[location].Value;
                allParams[location].Value = newQty;
            }

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                SqlCommand sComm = new SqlCommand( fullUpdateString.ToString(), sConn );
                sComm.Parameters.AddRange( allParams.ToArray() );
                sComm.ExecuteNonQuery();
            }
        }
    }
}