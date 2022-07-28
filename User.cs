using System.Data.SqlClient;

namespace KRealm_WebApi.Models
{
    public class User
    {
        private string connStr { get; set; }
        public int userID { get; set; }
        public string username { get; set; }
        public string password {get;set;}
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }

        public User() { connStr = System.Configuration.ConfigurationManager.ConnectionStrings["db"].ConnectionString; }

        internal User AddUser( User newUser )
        {
            User us = null;

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();

                //no duplicate usernames
                if( DoesUserExist( newUser.username ) )
                {
                    return null;
                }

                string command = "INSERT INTO User (username,password,address,city,state,zipcode,active) VALUES (@us,@pa,@ad,@ci,@st,@zi,1);" +
                                 "SELECT userID,username,password,address,city,state,zipcode FROM Users WHERE username=@na";
                SqlCommand sComm = new SqlCommand( command, sConn );
                sComm.Parameters.AddRange( new SqlParameter[] {
                    new SqlParameter( "us", newUser.username ), new SqlParameter( "pa", Utility.Encrypt(newUser.password) ), new SqlParameter( "ad",newUser.address ),
                    new SqlParameter( "ci", newUser.city ), new SqlParameter( "st", newUser.state ), new SqlParameter( "zi", newUser.zipcode ),
                    new SqlParameter( "na", newUser.username ) } );

                using( SqlDataReader sdr = sComm.ExecuteReader() )
                {
                    if( sdr.HasRows )
                    {
                        while( sdr.Read() )
                        {
                            us.userID = (int) sdr["userID"];
                            us.username = sdr["username"].ToString();
                            us.password = Utility.Decrypt( sdr["password"].ToString() );
                            us.address = sdr["address"].ToString();
                            us.city = sdr["city"].ToString();
                            us.state = sdr["state"].ToString();
                            us.zipcode = sdr["zipcode"].ToString();
                        }
                    }
                }
            }

            return us;
        }

        private bool DoesUserExist(string username)
        {
            bool wasUserFound = true;

            using(SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                SqlCommand sComm = new SqlCommand( "SELECT * FROM User WHERE username=@us" );
                sComm.Parameters.AddWithValue( "us", username );
                int rowCount = sComm.ExecuteNonQuery();
                wasUserFound = rowCount > 0 ? true : false;
            }

            return wasUserFound;
        }
        
        internal User GetUser(string name)
        {
            User us = null;

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                SqlCommand sComm = new SqlCommand( "SELECT userID,username,password,address,city,state,zipcode FROM Users WHERE name=@name && active=1", sConn );
                sComm.Parameters.AddWithValue( "name", name );

                using( SqlDataReader sdr = sComm.ExecuteReader() )
                {
                    if( sdr.HasRows )
                    {
                        while( sdr.Read() )
                        {
                            us.userID = (int) sdr["userID"];
                            us.username = sdr["username"].ToString();
                            us.password = Utility.Decrypt( sdr["password"].ToString() );
                            us.address = sdr["address"].ToString();
                            us.city = sdr["city"].ToString();
                            us.state = sdr["state"].ToString();
                            us.zipcode = sdr["zipcode"].ToString();
                        }
                    }
                }
            }

            return us;
        }

        internal LoginAttempt CheckLogin( LoginAttempt login )
        {
            LoginAttempt result = login;
            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                SqlCommand sComm = new SqlCommand( "SELECT userID,username,password,address,city,state,zipcode FROM Users WHERE username=@na && active=1", sConn );
                sComm.Parameters.AddWithValue( "na", login.username );
                string encryptedPass = Utility.Encrypt( login.password );

                using( SqlDataReader sdr = sComm.ExecuteReader() )
                {
                    if( sdr.HasRows )
                    {
                        sdr.Read();
                        if( sdr["password"].ToString() == encryptedPass )
                            result.attemptResult = "Success";
                        else
                            result.attemptResult = "Incorrect Password";
                    }
                    else
                    {
                        result.attemptResult = "User not found";
                    }
                }
            }

            return result;
        }

        internal User UpdateUser(int id, User updated)
        {
            User us = null;

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                string command = "UPDATE User SET username=@un,password=@pa,address=@ad,city=@ci,state=@st,zipcode=@zi WHERE userID=@id1;" +
                                 "SELECT userID,username,password,address,city,state,zipcode FROM Users WHERE userID=@id2 && active=1";
                SqlCommand sComm = new SqlCommand( command, sConn );
                sComm.Parameters.AddRange( new SqlParameter[] {
                    new SqlParameter( "us", updated.username ), new SqlParameter( "pa", Utility.Encrypt(updated.password) ), new SqlParameter( "ad",updated.address ),
                    new SqlParameter( "ci", updated.city ), new SqlParameter( "st", updated.state ), new SqlParameter( "zi", updated.zipcode ), new SqlParameter( "id1", id ),
                    new SqlParameter( "id2", id ) } );
                
                using( SqlDataReader sdr = sComm.ExecuteReader() )
                {
                    if( sdr.HasRows )
                    {
                        while( sdr.Read() )
                        {
                            us.userID = (int) sdr["userID"];
                            us.username = sdr["username"].ToString();
                            us.password = Utility.Decrypt( sdr["password"].ToString() );
                            us.address = sdr["address"].ToString();
                            us.city = sdr["city"].ToString();
                            us.state = sdr["state"].ToString();
                            us.zipcode = sdr["zipcode"].ToString();
                        }
                    }
                }
            }

            return us;
        }

        internal bool DeleteUser(int id)
        {
            bool wasSuccess = false;

            using( SqlConnection sConn = new SqlConnection( connStr ) )
            {
                sConn.Open();
                SqlCommand sComm = new SqlCommand( "UPDATE User SET active=0 WHERE userID=@id" );
                sComm.Parameters.AddWithValue( "id", id );

                int result = sComm.ExecuteNonQuery();
                wasSuccess = (result > 0) ? true : false;
            }

            return wasSuccess;
        }
    }
}