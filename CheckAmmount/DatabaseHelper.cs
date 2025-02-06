//using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Oracle.ManagedDataAccess.Client;

namespace CheckAmmount
{
    //public class DatabaseHelper
    //{
    //    private string _connectionString;
    //    private string query;
    //    public DatabaseHelper(string connectionString)
    //    {
    //        _connectionString = connectionString;
    //    }

    //    public DataTable ExecuteQuery(string query)
    //    {
    //        DataTable dataTable = new DataTable();
    //        using (OracleConnection connection = new OracleConnection(_connectionString))
    //        {
    //            try
    //            {
    //                connection.Open();
    //                using (OracleCommand command = new OracleCommand(query, connection))
    //                {
    //                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
    //                    {
    //                        adapter.Fill(dataTable);
    //                    }
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                throw new Exception("Ошибка выполнения запроса: " + ex.Message);
    //            }
    //        }
    //        return dataTable;
    //    }

    //    public string SetQuery()
    //    {
    //        return query = "SELECT * FROM;";
    //    }

    //    public bool TestConnection()
    //    {
    //        using (OracleConnection connection = new OracleConnection(_connectionString))
    //        {
    //            try
    //            {
    //                connection.Open();
    //                return true;
    //            }
    //            catch (Exception)
    //            {
    //                return false;
    //            }
    //        }
    //    }
    //}
}
