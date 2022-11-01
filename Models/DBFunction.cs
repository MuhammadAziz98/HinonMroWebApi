using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hinon_Mro_WebApi.Models
{
    public class DBFunction
    {
        
        DataTable dt;
        SqlParameter[] param;
       
        public int Crud(string connection,string ProcName, params SqlParameter[] param)
        {

            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlCommand cmd = new SqlCommand(ProcName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (param != null)
                {
                    foreach (SqlParameter p in param)
                    {
                        cmd.Parameters.Add(p);
                    }

                }

                con.Open();
                int count = cmd.ExecuteNonQuery();
                con.Close();
                return count;
            }

        }
        public int CrudScalar(string connection, string ProcName, params SqlParameter[] param)
        {

            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlCommand cmd = new SqlCommand(ProcName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (param != null)
                {
                    foreach (SqlParameter p in param)
                    {
                        cmd.Parameters.Add(p);
                    }

                }

                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                return count;
            }

        }

        public DataTable SelectSPWithParam(string connection, string ProcName, params SqlParameter[] param)
        {

            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlDataAdapter da = new SqlDataAdapter(ProcName, con);
                da.SelectCommand.CommandTimeout = 0;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (param != null)
                {
                    foreach (SqlParameter p in param)
                    {
                        da.SelectCommand.Parameters.Add(p);
                    }
                }
                dt = new DataTable();
                con.Open();
                da.Fill(dt);
                con.Close();
                return dt;

            }

        }
        public DataTable SelectSPWithOutParam(string connection, string ProcName)
        {

            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlDataAdapter da = new SqlDataAdapter(ProcName, con);
                da.SelectCommand.CommandTimeout = 0;
                dt = new DataTable();
                con.Open();
                da.Fill(dt);
                con.Close();
                return dt;

            }

        }

        public string CrudScalarQuery(string connection, string Query)
        {

            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlCommand cmd = new SqlCommand(Query, con);
                con.Open();
                string msg = (cmd.ExecuteScalar()).ToString();
                con.Close();
                con.Dispose();
                return msg;
            }

        }
        public DataTable SelectWithQueryDT(string connection, string Query)
        {
            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlDataAdapter da = new SqlDataAdapter(Query, con);
                da.SelectCommand.CommandTimeout = 0;
                dt = new DataTable();
                con.Open();
                da.Fill(dt);
                con.Dispose();
                con.Close();
                return dt;
            }
        }
    }
}
