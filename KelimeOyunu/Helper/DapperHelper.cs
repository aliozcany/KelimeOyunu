using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace KelimeOyunu.Helper
{
    public class DapperHelper
    {
        public DapperHelper() { }
        private readonly string ConnectionString = "Server=DESKTOP-EMVU51B;Database=YazilimYapimiProjesi;Integrated Security=true;TrustServerCertificate=true;";

        public SqlConnection GetConnection() { return new SqlConnection(ConnectionString); }

       



        public int Execute(string sql, object param = null)
        {
            var conn = GetConnection();
            try
            {
                var data = conn.Execute(sql, param);
                return data;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public T ExecuteScalar<T>(string sql, object data = null)
        {
            var conn = GetConnection();
            try
            {
                conn.Open();
                return conn.ExecuteScalar<T>(sql, data);

            }
            catch (Exception e)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

        }
        public object Query(string sql, object value = null)
        {
            var conn = GetConnection();
            try
            {
                conn.Open();
                return conn.Query(sql, value);

            }
            catch (Exception e)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        

        public T QueryFirstOrDefault<T>(string sql, object value = null)
        {
            var conn = GetConnection();
            try
            {
                conn.Open();
                return conn.QueryFirstOrDefault<T>(sql, value);

            }
            catch (Exception e)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }
        }
        public IEnumerable<T> Query<T>(string sql, object data = null)
        {
            var conn = GetConnection();
            try
            {
                conn.Open();
                return conn.Query<T>(sql, data);

            }
            catch (Exception e)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

        }

        public T QuerySingleOrDefault<T>(string sql, object value = null)
        {
            var conn = GetConnection();
            try
            {
                conn.Open();
                return conn.ExecuteScalar<T>(sql, value);

            }
            catch (Exception e)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }
        }



        internal void Execute<T>()
        {
            try
            {

            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}