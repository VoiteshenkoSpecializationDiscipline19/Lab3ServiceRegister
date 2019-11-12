using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.Web.Script.Services;

[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[WebService(Description = "Lab2", Namespace = XmlNS)]
public class WebService : System.Web.Services.WebService
{
    public const string XmlNS = "http://asmx.lab2.com/";

    public WebService()
    {
        //InitializeComponent(); 
    }

    [WebMethod]
    public Boolean IsServiceExistsID(int serviceId)
    {
        using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-D85MEBM;Initial Catalog=service;Integrated Security=True;Pooling=False"))
        {
            string queryString = "SELECT COUNT(1) FROM service WHERE id = " + serviceId.ToString();
            SqlCommand command = new SqlCommand(queryString, connection);
            try
            {
                command.Connection.Open();
                return (Int32)command.ExecuteScalar() == 1;
            }
            catch
            {
                return false;
            }
        }
    }

    [WebMethod]
    public Boolean IsServiceExistsName(string serviceName)
    {
        using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-D85MEBM;Initial Catalog=service;Integrated Security=True;Pooling=False"))
        {
            string queryString = "SELECT COUNT(1) FROM service WHERE name = \'" + serviceName + "\';";
            SqlCommand command = new SqlCommand(queryString, connection);
            try
            {
                command.Connection.Open();
                return (Int32)command.ExecuteScalar() == 1;
            }
            catch
            {
                return false;
            }
        }
    }

    [WebMethod]
    public DataTable GetServiceList()
    {
        using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-D85MEBM;Initial Catalog=service;Integrated Security=True;Pooling=False"))
        {
            string queryString = "SELECT * FROM service;";
            SqlCommand command = new SqlCommand(queryString, connection);
            Dictionary<string, string> column;

            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("address", typeof(string));
            dt.TableName = "Services";
            try
            {
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {    
                    column = new Dictionary<string, string>();

                    column["id"] = reader["id"].ToString();
                    column["name"] = reader["name"].ToString();
                    column["address"] = reader["address"].ToString();
                    dt.Rows.Add(column);
                }
                reader.Close();
                return dt;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    [WebMethod]
    public String GetServiceListString()
    {
        using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-D85MEBM;Initial Catalog=service;Integrated Security=True;Pooling=False"))
        {
            string queryString = "SELECT service.id, service.name, service.address, methods.methodname FROM service inner join methods on service.id = methods.serviceid;";
            SqlCommand command = new SqlCommand(queryString, connection);
            Dictionary<string, string> column;
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

          
            try
            {
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    column = new Dictionary<string, string>();

                    column["id"] = reader["id"].ToString();
                    column["name"] = reader["name"].ToString();
                    column["address"] = reader["address"].ToString();
                    column["methodname"] = reader["methodname"].ToString();
                    rows.Add(column);
                }
                reader.Close();
                string result = "";

                foreach (Dictionary<String, String> row in rows)
                {
                    result += string.Join(";", row.Select(x => x.Key + ":" + x.Value).ToArray()) + "\n";
                }

                return result;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }
}
