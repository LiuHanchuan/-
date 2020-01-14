using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Apache.Models
{
    public class HosDataConnect
    {
        protected SqlConnection conn;

        public void OpenConnection()
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["HosDataConnection"].ConnectionString);
            
            try
            {
                if (conn.State.ToString() != "Open")
                {
                    conn.Open();
                }
            }
            catch (SqlException ex)
            {

                throw ex;
            }
        }

        public void CloseConnection()
        {
            try
            {
                conn.Close();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }


        //Insert
        public int InsertData(string sql)
        {
            int i = 0;
            try
            {
                if (conn.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //serach
        public DataSet List(string sql)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Detail
        public DataTable Detail(string sql)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //Delete
        public int Delete(string sql)
        {
            try
            {
                int result = 0;
                SqlCommand cmd = new SqlCommand(sql, conn);
                result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Update
        public int Update(string sql)
        {
            try
            {
                int result = 0;
                SqlCommand cmd = new SqlCommand(sql, conn);
                result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}