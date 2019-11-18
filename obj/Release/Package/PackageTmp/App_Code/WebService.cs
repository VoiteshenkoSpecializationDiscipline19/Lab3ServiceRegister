using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using MySql.Data.MySqlClient;

[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[WebService(Description = "Lab3", Namespace = XmlNS)]
public class WebService : System.Web.Services.WebService
{
    public const string XmlNS = "http://asmx.lab3.com/";

    //public const string connectionString = "Data Source=DESKTOP-D85MEBM;Initial Catalog=service;Integrated Security=True;Pooling=False";
    const string connectionString = "server=remotemysql.com;port=3306;database=3DtTySuocC;Uid=3DtTySuocC;Pwd=RBWFBrY0NC";
    public WebService()
    {
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, XmlSerializeString = true)]
    public Boolean IsServiceExistsID(int serviceId)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string queryString = "SELECT COUNT(1) FROM service WHERE id = " + serviceId.ToString();
            MySqlCommand command = new MySqlCommand(queryString, connection);
            try
            {
                command.Connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) == 1;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, XmlSerializeString = true)]
    public Boolean IsServiceExistsName(string serviceName)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string queryString = "SELECT COUNT(1) FROM service WHERE name = \'" + serviceName + "\';";
            MySqlCommand command = new MySqlCommand(queryString, connection);
            try
            {
                command.Connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) == 1;
            }
            catch
            {
                return false;
            }
        }
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public void IsMethodExistsName(string serviceName, string methodName)
    {
        Context.Response.Clear();
        Context.Response.ContentType = "application/json";
        
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string queryString = "SELECT COUNT(1) FROM service inner join methods on service.id = methods.serviceid WHERE service.name = \'" + serviceName + "\' AND methods.methodname = \'" + methodName +"\';";
            MySqlCommand command = new MySqlCommand(queryString, connection);
            try
            {
                command.Connection.Open();


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                HelloWorldData data = new HelloWorldData();
                data.Message = Convert.ToInt32(command.ExecuteScalar()) == 1;
                Context.Response.Write(js.Serialize(data));             
                return;
            }
            catch
            {
                return;
            }
        }
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, XmlSerializeString = true)]
    public DataTable GetServiceList()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string queryString = "SELECT * FROM service;";
            MySqlCommand command = new MySqlCommand(queryString, connection);
            Dictionary<string, string> column;

            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("address", typeof(string));
            dt.TableName = "Services";
            try
            {
                command.Connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                
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
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, XmlSerializeString = true)]
    public String GetServiceListString()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string queryString = "SELECT service.id, service.name, service.address, methods.methodname FROM service inner join methods on service.id = methods.serviceid;";
            MySqlCommand command = new MySqlCommand(queryString, connection);
            Dictionary<string, string> column;
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

          
            try
            {
                command.Connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

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
            catch(Exception ex)
            {
                return ex.Message;
                //return null;
            }
        }
    }
}

class HelloWorldData
{
    public Boolean Message;
}
