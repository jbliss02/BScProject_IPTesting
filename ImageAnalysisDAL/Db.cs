using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ImageAnalysisDAL
{
    public abstract class Db
    {
        protected Db(string connectionString) { this.connectionString = connectionString; }

        protected string connectionString;

        protected ConnectionStringSettingsCollection connections { get; set; } = ConfigurationManager.ConnectionStrings;

        protected DataTable DataTableFromProc(string procName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);

                con.Close();
            }

            return dt;
        }

        protected DataTable DataTableFromProc(string procName, List<SqlParameter> paras)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                foreach (SqlParameter p in paras)
                {
                    cmd.Parameters.AddWithValue(p.ParameterName, p.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;               
                da.Fill(dt);
                con.Close();
            }
                         
            return dt;
        }

        protected DataTable DataTableFromProc(string procName, SqlParameter para)
        {
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(para);
            return DataTableFromProc(procName, paras);
        }

        protected DataTable DataTableFromView(string viewName)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM " + viewName, connectionString))
            {
                da.Fill(dt);

            }

            return dt;
               
        }

        protected string RunProcWithReturn(string procName, SqlParameter p)
        {
            string st = string.Empty;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;


                cmd.Parameters.AddWithValue(p.ParameterName, p.Value);

                //add the return value
                SqlParameter retval = cmd.Parameters.Add("@ret", SqlDbType.VarChar);
                retval.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                st = (string)cmd.Parameters["@ret"].Value.ToString();

                con.Close();
            }


            return st;
        }

        protected void RunProc(string procName, List<SqlParameter> paras)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;

                foreach (SqlParameter p in paras)
                {
                    cmd.Parameters.AddWithValue(p.ParameterName, p.Value);
                }

                cmd.ExecuteNonQuery();
                con.Close();
            }


        }

    }
}
