﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Common;

namespace TMS.DAL
{
    public class CustomerDAL
    {
        public string AddCustomer(Customer c, List<string> orderName)
        {
            try
            {
                SqlConnection con = new SqlConnection(HelperDB.ConnectionString);
                con.Open();

                String query = "INSERT INTO Customers VALUES (@name, @cninc, @number, @address, @remarks)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@name", c.Name);
                cmd.Parameters.AddWithValue("@cninc", c.CNIC);
                cmd.Parameters.AddWithValue("@number", c.ContactNumber);
                cmd.Parameters.AddWithValue("@address", c.Address);
                cmd.Parameters.AddWithValue("@remarks", c.Remarks);

                int res = cmd.ExecuteNonQuery();

                if (res < 0)
                    return "error";

                query = "SELECT * FROM Customers ";

                cmd = new SqlCommand(query, con);
                int id = 0;
                SqlDataReader read = cmd.ExecuteReader();
                while(read.Read())
                {
                    if(read["CNIC"].ToString() == c.CNIC)
                    {
                        id = (int)read["ID"];
                    }
                }
                read.Close();
                for (int i = 0; i < orderName.Count; i++)
                {
                    string tableName = orderName.ElementAt(i).Trim().ToUpper();
                    Dictionary<String, int> d = c.Order.ElementAt(i);

                    List<string> colName = new List<string>(d.Keys);
                    List<int> colValue = new List<int>(d.Values);

                    query = "INSERT INTO Measurement_" + tableName + " ( Name, CustomerID, ";
                    //GET COLUMN NAMES
                    for (int j = 0; j < colName.Count; j++)
                    {
                        query += colName.ElementAt(j);
                        if (j != colName.Count - 1)
                            query += ", ";
                    }
                    query += ") VALUES ('"+ tableName.ToLower() +"', " + id.ToString() + ", ";
                    //GET COLUMN VALUES
                    for (int j = 0; j < colValue.Count; j++)
                    {
                        query += colValue.ElementAt(j);
                        if (j != colValue.Count - 1)
                            query += ", ";
                    }

                    query += ")";

                    cmd = new SqlCommand(query, con);
                    res = cmd.ExecuteNonQuery();

                    if (res < 0)
                        return "error";


                    Order o = new Order();
                    o.CustomerID = id;
                    o.OrderName = orderName.ElementAt(i).Trim();
                    o.Status = false;

                    new OrderDAL().AddNewOrder(o);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw ex;
            }

            return "";
        }

        public List<Customer> GellAllCustomers()
        {
            SqlConnection con = new SqlConnection(HelperDB.ConnectionString);
            List<Customer> list = new List<Customer>();
            try
            {
                con.Open();
                String query = "SELECT * FROM Customers";

                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    Customer c = new Customer();
                    c.ID = (int) reader["ID"];
                    c.Name = reader["Name"].ToString();
                    c.CNIC = reader["CNIC"].ToString();
                    c.ContactNumber = reader["ContactNumber"].ToString();
                    c.Address = reader["Address"].ToString();
                    c.Remarks = reader["Remarks"].ToString();
                    list.Add(c);
                }
                reader.Close();
            }
            catch (Exception)
            {

                throw;
            }
            return list;
        }
    }
}
