﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ImageAnalysisDAL
{
    public class Db
    {
        public Db(string connectionString) { this.connectionString = connectionString; }

        protected string connectionString;

        public ConnectionStringSettingsCollection connections { get; set; } = ConfigurationManager.ConnectionStrings;

        public DataTable DataTableFromProc(string procName, string stcon)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(stcon))
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

        public DataTable DataTableFromProc(string procName, string stcon, List<SqlParameter> paras)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(stcon))
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

        public DataTable DataTableFromProc(string procName, string stcon, SqlParameter para)
        {
            List<SqlParameter> paras = new List<SqlParameter>();
            paras.Add(para);
            return DataTableFromProc(procName, stcon, paras);
        }

        public DataTable DataTableFromView(string viewName, string stcon)
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM " + viewName, stcon);

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public string RunProcWithReturn(string procName, string stcon, SqlParameter p)
        {
            string st = string.Empty;

            SqlConnection con = new SqlConnection(stcon);
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

            return st;
        }

    }
}