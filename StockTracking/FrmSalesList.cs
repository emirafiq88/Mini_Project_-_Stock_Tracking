﻿using StockTracking.BLL;
using StockTracking.DAL.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockTracking
{
    public partial class FrmSalesList : Form
    {
        public FrmSalesList()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        private void txtSalesAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            FrmSales frm = new FrmSales();
            frm.dto = dto;
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;

            dto = bll.Select();
            dataGridView1.DataSource = dto.Sales;
            ClearFilters();
        }

        SalesBLL bll = new SalesBLL();
        SalesDTO dto = new SalesDTO();
        SalesDetailDTO detail = new SalesDetailDTO();

        private void FrmSalesList_Load(object sender, EventArgs e)
        {
            dto = bll.Select();
            dataGridView1.DataSource = dto.Sales;
            dataGridView1.Columns[0].HeaderText = "Customer Name";
            dataGridView1.Columns[1].HeaderText = "Product Name";
            dataGridView1.Columns[2].HeaderText = "Category Name";
            dataGridView1.Columns[6].HeaderText = "Sales Amount";
            dataGridView1.Columns[7].HeaderText = "Price";
            dataGridView1.Columns[8].HeaderText = "Sales Date";
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[9].Visible = false;
            dataGridView1.Columns[10].Visible = false;
            dataGridView1.Columns[11].Visible = false;
            dataGridView1.Columns[12].Visible = false;
            dataGridView1.Columns[13].Visible = false;

            cmbCategory.DataSource = dto.Categories;
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "ID";
            cmbCategory.SelectedIndex = -1;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<SalesDetailDTO> list = dto.Sales;
            if (txtProductName.Text.Trim() != "")
                list = list.Where(x => x.ProductName.Contains(txtProductName.Text)).ToList();
            if (txtCustomerName.Text.Trim() != "")
                list = list.Where(x => x.CustomerName.Contains(txtCustomerName.Text)).ToList();
            if (cmbCategory.SelectedIndex != -1)
                list = list.Where(x => x.CategoryID == Convert.ToInt32(cmbCategory.SelectedValue)).ToList();
            if (txtPrice.Text.Trim() != "")
            {
                if (rbPriceEqual.Checked)
                    list = list.Where(x => x.Price == Convert.ToInt32(txtPrice.Text)).ToList();
                else if (rbPriceMore.Checked)
                    list = list.Where(x => x.Price > Convert.ToInt32(txtPrice.Text)).ToList();
                else if (rbPriceLess.Checked)
                    list = list.Where(x => x.Price < Convert.ToInt32(txtPrice.Text)).ToList();
                else
                    MessageBox.Show("Please select a criterion from price group");
            }
            if (txtSalesAmount.Text.Trim() != "")
            {
                if (rbSaleEqual.Checked)
                    list = list.Where(x => x.SalesAmount == Convert.ToInt32(txtSalesAmount.Text)).ToList();
                else if (rbSaleMore.Checked)
                    list = list.Where(x => x.SalesAmount > Convert.ToInt32(txtSalesAmount.Text)).ToList();
                else if (rbSaleLess.Checked)
                    list = list.Where(x => x.SalesAmount < Convert.ToInt32(txtSalesAmount.Text)).ToList();
                else
                    MessageBox.Show("Please select a criterion from sale amount group");
            }
            if (chkDate.Checked)
                list = list.Where(x => x.SalesDate > dpStart.Value && x.SalesDate < dpEnd.Value).ToList();

            dataGridView1.DataSource = list;

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFilters();
        }

        private void ClearFilters()
        {
            txtPrice.Clear();
            txtCustomerName.Clear();
            txtProductName.Clear();
            txtSalesAmount.Clear();
            rbPriceEqual.Checked = false;
            rbSaleEqual.Checked = false;
            rbSaleMore.Checked = false;
            rbPriceMore.Checked = false;
            rbSaleLess.Checked = false;
            rbPriceLess.Checked = false;
            dpStart.Value = DateTime.Today;
            dpEnd.Value = DateTime.Today;
            chkDate.Checked = false;
            cmbCategory.SelectedIndex = -1;
            dataGridView1.DataSource = dto.Sales;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            detail = new SalesDetailDTO();
            detail.SalesID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[10].Value);
            detail.ProductID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[4].Value);
            detail.CustomerName = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            detail.ProductName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            detail.Price = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[7].Value);
            detail.SalesAmount = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[6].Value);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (detail.SalesID == 0)
                MessageBox.Show("Please slect a sales frm table");
            else
            {
                FrmSales frm = new FrmSales();
                frm.isUpdate = true;
                frm.detail = detail;
                frm.dto = dto;
                this.Hide();
                frm.ShowDialog();
                this.Visible = true;

                bll = new SalesBLL();
                dto = bll.Select();
                dataGridView1.DataSource = dto.Sales;
                ClearFilters();

            }

        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            if (detail.SalesID == 0)
                MessageBox.Show("Please select a sales from table");
            else
            {
                DialogResult result = MessageBox.Show("Are you sure?", "Warning!!", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (bll.Delete(detail))
                    {
                        MessageBox.Show("Sales was deleted");
                        bll = new SalesBLL();
                        dto = bll.Select();
                        dataGridView1.DataSource= dto.Sales;
                        ClearFilters();
                    }
                }
            }
        }
    }
}
