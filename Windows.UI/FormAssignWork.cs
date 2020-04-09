﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMS.BLL;
using TMS.Common;

namespace Windows.UI
{
    public partial class FormAssignWork : Form
    {
        public FormAssignWork()
        {
            InitializeComponent();
        }

        private void FormAssignWork_Load(object sender, EventArgs e)
        {
            List<Customer> customerList = new CustomerBLL().GetAllCustomers();
            cbxCustomer.DataSource = customerList;
            cbxCustomer.DisplayMember = "Name";

            List<Worker> workerList = new WorkerBLL().GetAllWorkers();
            cbxWorker.DataSource = workerList;
            cbxWorker.DisplayMember = "Name";

            DataTable table = new WorkerBLL().GetWorkersStats(workerList);
            tableWorkerInfo.DataSource = table;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbxCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            Customer c = (Customer) cbxCustomer.SelectedValue;
            List<String> list = new OrderBLL().GetTypeOfMeasurement(c.ID);

            cbxOrderName.DataSource = list;
            cbxOrderName.DisplayMember = "OrderName";
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            Worker selectedWorker = (Worker)cbxWorker.SelectedValue;
            Customer selectedCustomer = (Customer)cbxCustomer.SelectedValue;
            String orderName = cbxOrderName.SelectedValue.ToString();

            String msg = new OrderBLL().AssingToWorker(selectedCustomer, selectedWorker, orderName);
            if(msg.Trim() == "")
            {
                MessageBox.Show("Task Assigned", "Success");
            }
            else
            {
                MessageBox.Show("Unable to Assign Task", "Error");
            }
        }
    }
}
