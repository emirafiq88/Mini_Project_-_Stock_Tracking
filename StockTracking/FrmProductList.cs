using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockTracking.DAL.DAO;
using StockTracking.DAL.DTO;
using StockTracking.BLL;

namespace StockTracking
{
    public partial class FrmProductList : Form
    {
        public FrmProductList()
        {
            InitializeComponent();
        }
        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            FrmProduct product = new FrmProduct();
            product.dto = dto;
            this.Hide();
            product.ShowDialog();
            this.Visible = true;
            dto = bll.Select();
            dataGridView1.DataSource = dto.Products;
            CleanFilter();
        }

        ProductBLL bll = new ProductBLL();
        ProductDTO dto = new ProductDTO();

        private void FrmProductList_Load(object sender, EventArgs e)
        {
            dto = bll.Select();
            cmbCategory.DataSource = dto.Categories;
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "ID";
            cmbCategory.SelectedIndex = -1;
            dataGridView1.DataSource = dto.Products;
            dataGridView1.Columns[0].HeaderText = "Product Name";
            dataGridView1.Columns[1].HeaderText = "Category Name";
            dataGridView1.Columns[2].HeaderText = "Stock Amount";
            dataGridView1.Columns[3].HeaderText = "Price";
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<ProductDetailDTO> list = dto.Products;
            if (txtProductName.Text.Trim() != null)
                list = list.Where(x => x.ProductName.Contains(txtProductName.Text)).ToList();
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
            if (txtProductStock.Text.Trim() != "")
            {
                if (rbStockEqual.Checked)
                    list = list.Where(x => x.StockAmount == Convert.ToInt32(txtProductStock.Text)).ToList();
                else if (rbStockMore.Checked)
                    list = list.Where(x => x.StockAmount > Convert.ToInt32(txtProductStock.Text)).ToList();
                else if (rbStockLess.Checked)
                    list = list.Where(x => x.StockAmount < Convert.ToInt32(txtProductStock.Text)).ToList();
                else
                    MessageBox.Show("Please select a criterion from stock group");
            }

            dataGridView1.DataSource = list;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CleanFilter();
        }

        private void CleanFilter()
        {
            txtPrice.Clear();
            txtProductName.Clear();
            txtProductStock.Clear();
            cmbCategory.SelectedIndex = -1;
            rbPriceEqual.Checked = false;
            rbPriceLess.Checked = false;
            rbPriceMore.Checked = false;
            rbStockEqual.Checked = false;
            rbStockLess.Checked = false;
            rbStockMore.Checked = false;
            dataGridView1.DataSource = dto.Products;
        }

        ProductDetailDTO detail = new ProductDetailDTO();

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            detail = new ProductDetailDTO();
            detail.ProductID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[4].Value);
            detail.CategoryID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[5].Value);
            detail.ProductName = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            detail.Price = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[3].Value);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (detail.ProductID == 0)
                MessageBox.Show("Please select a product from table");
            else
            {
                FrmProduct frm = new FrmProduct();
                frm.isUpdate = true;
                frm.detail = detail;
                frm.dto = dto;
                this.Hide();
                frm.ShowDialog();
                this.Visible = false;
                bll = new ProductBLL();
                dto = bll.Select();
                dataGridView1.DataSource = dto.Products;
                CleanFilter();
            }
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            if (detail.ProductID == 0)
                MessageBox.Show("Please select a product from table");
            else
            {
                DialogResult result = MessageBox.Show("Are you sure?", "Warning!!", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if(bll.Delete(detail))
                    {
                        MessageBox.Show("Product was deleted");
                        bll = new ProductBLL();
                        dto = bll.Select();
                        dataGridView1.DataSource= dto.Products;
                        cmbCategory.DataSource = dto.Categories;
                        CleanFilter();
                    }
                }
            }
        }
    }
}
