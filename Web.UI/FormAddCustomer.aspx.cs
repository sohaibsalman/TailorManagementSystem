﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TMS.BLL;
using TMS.Common;

namespace Web.UI
{
    public partial class FormAddCustomer : System.Web.UI.Page
    {
        private static List<Dictionary<string, int>> measurementList = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                measurementList = new List<Dictionary<string, int>>();
                String path = @"~/files";
                String ph_path = Server.MapPath(path);

                string[] files = new MeasurementsBLL().GetFileNamesWeb(ph_path);

                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = RemovePath(files[i]);
                }

                ddlMeasurementType.DataSource = files;
                ddlMeasurementType.SelectedIndex = 0;
                ddlMeasurementType.DataBind();
            }
        }

        private string RemovePath(string path)
        {
            int index = path.LastIndexOf("\\");
            path = path.Remove(0, index + 1);
            path = path.Replace(".txt", "");
            return path;
        }

        protected void ddlMeasurementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBindGrid();
        }

        protected void gridMeasurements_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gridMeasurements.EditIndex = e.NewEditIndex;
            DataBindGrid();
        }

        protected void gridMeasurements_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gridMeasurements.EditIndex = -1;
            DataBindGrid();
        }

        private void DataBindGrid()
        {
            string fileName = ddlMeasurementType.SelectedValue + ".txt";
            String path = "~/files/";
            String ph_path = Server.MapPath(path);
            ph_path += fileName;
            DataTable dt = new MeasurementsBLL().GetMeasurementsFromFileWeb(ph_path);
            gridMeasurements.DataSource = dt;
            gridMeasurements.DataBind();
        }

        protected void gridMeasurements_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gridMeasurements.Rows[e.RowIndex];
            TextBox txtMeasurementValues = (TextBox) row.FindControl("txtMeasurementValue");

            gridMeasurements.Rows[e.RowIndex].Cells[1].Text = txtMeasurementValues.Text;
            gridMeasurements.EditIndex = -1;

            DataBindGrid();
        }

        protected void btnAddMeasurement_Click(object sender, EventArgs e)
        {
            Dictionary<String, int> dict = new Dictionary<string, int>();
            for (int i = 0; i < gridMeasurements.Rows.Count; i++)
            {
                string key = ((Label)gridMeasurements.Rows[i].FindControl("txtMeasurementName")).Text;
                int value = Convert.ToInt32(((TextBox)gridMeasurements.Rows[i].FindControl("txtMeasurementValue")).Text);
                dict.Add(key, value);
            }

            measurementList.Add(dict);

            String fileName = ddlMeasurementType.SelectedValue;
            listMeasurements.Items.Add(fileName);



        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Customer c = new Customer();

            c.Name = txtName.Text;
            c.CNIC = txtCNIC.Text;
            c.ContactNumber = txtNumber.Text;
            c.Address = txtAddress.Text;
            c.Order = measurementList;
            c.Remarks = txtRemarks.Text;

            List<string> orderName = new List<string>();

            for (int i = 0; i < listMeasurements.Items.Count; i++)
            {
                listMeasurements.SelectedIndex = i;
                orderName.Add(listMeasurements.SelectedValue);
            }

            String msg = new CustomerBLL().AddCustomer(c, orderName);
            if (msg.Trim() == "")
            {
                Response.Write("<script><alert>Customer Added!</alert></script>");
            }
            else
                Response.Write("<script><alert>Customer NOT Added!</alert></script>");
        }
    }
}