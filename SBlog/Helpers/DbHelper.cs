using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace SBlog.Helpers
{
    public class DbHelper
    {
        private string _connectionStr { get { return ConfigurationManager.AppSettings["ConnectionStr"]; } }

        public DbHelper()
        {
            var conn = new SqlConnection(_connectionStr);
            conn.Open();
        }

        
    }
}