using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ProgrammingTask
{


    public partial class Form1 : Form
    {

        private bool IsValueChanged = false;

        private string filename;

        ProductList productList;

        DataGridViewRow selectedRow = null;

        public Form1()
        {
            InitializeComponent();
        }

        #region OpenAndSavefile
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (thread == null || !thread.IsAlive)
            {
                OpenFileDialog dlgOpenFile = new OpenFileDialog();
                dlgOpenFile.Filter = "csv files (*.csv)|*.csv";
                dlgOpenFile.Title = "Open CSV file";
                if (dlgOpenFile.ShowDialog() == DialogResult.OK)
                {
                    filename = dlgOpenFile.FileName;
                    productList = new ProductList();
                    productList.LoadFromFile(dlgOpenFile.FileName);
                    DataGrid.DataSource = productList.Products;
                    DataGrid.AllowUserToAddRows = true;
                    this.Text = filename;
                }
            }
            else
            {
                MessageBox.Show("Calculation in procces", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (productList != null)
            {
                this.Text = filename;
                IsValueChanged = false;
                productList.SaveAsCsv(filename);
            }
            else
            {
                MessageBox.Show("No data available", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (productList != null)
            {
                SaveFileDialog dlgSaveFile = new SaveFileDialog();
                dlgSaveFile.Filter = "csv files (*.csv)|*.csv|xml files (*.xml)|*.xml";
                dlgSaveFile.Title = "Save as";
                if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                {
                    if (dlgSaveFile.FilterIndex == 1)
                    {
                        this.Text = filename;
                        IsValueChanged = false;
                        productList.SaveAsCsv(dlgSaveFile.FileName);
                    }
                    if (dlgSaveFile.FilterIndex == 2)
                    {
                        productList.SaveAsXml(dlgSaveFile.FileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("No data available", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region ProgressBar
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            DataGrid.Size = new Size(this.Size.Width - 40, this.Size.Height - 95);
            progressBar.Size = new Size(this.Size.Width - 160, this.progressBar.Size.Height);
            button1.Location = new Point(button1.Location.X, DataGrid.Size.Height + 33);
            button2.Location = new Point(button2.Location.X, DataGrid.Size.Height + 33);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataGrid.Size = new Size(this.Size.Width - 40, this.Size.Height - 95);
            progressBar.Size = new Size(this.Size.Width - 160, this.progressBar.Size.Height);
            KeyPreview = true;
            IsValueChanged = false;
            button1.Location = new Point(button1.Location.X, DataGrid.Size.Height + 33);
            button2.Location = new Point(button2.Location.X, DataGrid.Size.Height + 33);
        }

        Thread thread;

        private delegate void progBar(int value, ProgressBar progressBar);

        private delegate void visibleBar(Control progressBar);

        private delegate void readOnlyDataGrid(DataGridView dg, bool value);

        void ChangeValue(int value, ProgressBar progressBar)
        {
            progressBar.Value = value;
        }
        void VisibleBar(Control progressBar)
        {
            progressBar.Visible = false;
        }
        void readOnlyGrid(DataGridView dg, bool val)
        {
            dg.ReadOnly = val;
        }
        private void calcStaticticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            if (thread == null || !thread.IsAlive)
            {
                thread = new Thread(() =>
                {
                    if (DataGrid.InvokeRequired)
                    {
                        Invoke(new readOnlyDataGrid(readOnlyGrid), new object[] { DataGrid, true });
                    }
                    else
                    {
                        DataGrid.ReadOnly = true;
                    }
                    for (int i = progressBar.Minimum; i <= progressBar.Maximum; i++)
                    {
                        if (progressBar.InvokeRequired)
                        {
                            Invoke(new progBar(ChangeValue), new object[] { i, progressBar });
                        }
                        else
                        {
                            progressBar.Value = i;
                        }
                        Thread.Sleep(200);
                    }
                    Thread.Sleep(600);
                    if (progressBar.InvokeRequired)
                    {
                        Invoke(new visibleBar(VisibleBar), new object[] { progressBar });
                    }
                    else
                    {
                        progressBar.Visible = false;
                    }
                    if (DataGrid.InvokeRequired)
                    {
                        Invoke(new readOnlyDataGrid(readOnlyGrid), new object[] { DataGrid, false });
                    }
                    else
                    {
                        DataGrid.ReadOnly = false;
                    }
                });
                thread.Start();
            }

        }

        #endregion
        //coment

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (productList != null && IsValueChanged == true)
            {
                DialogResult dlgRes = MessageBox.Show(String.Format("Do you want to save a file {0}", filename),
                    "Message", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dlgRes == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(new object(), new EventArgs());
                }
            }
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
        }

        private void DataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            IsValueChanged = true;
            this.Text = filename + " *";
        }

        void UpdateGrid()
        {
            int i = 0;
            if (selectedRow != null)
            {
                i = selectedRow.Index;
            }
            DataGrid.DataSource = typeof(List<Product>);
            DataGrid.DataSource = productList.Products;
            selectedRow = DataGrid.Rows[i];
        }

        private void button1_Click(object sender, EventArgs e)//editSected
        {
            if (thread == null || !thread.IsAlive)
            {
                if (selectedRow != null)
                {
                    EditDialog editDialog = new EditDialog(selectedRow);
                    if (editDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (editDialog.product != null) 
                        {
                            productList.Products[selectedRow.Index] = editDialog.product;
                            DataGrid.Refresh();
                            IsValueChanged = true;
                            this.Text = filename + " *";
                        }
                        else
                        {
                            MessageBox.Show("Data not valid");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Calculation in procces", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)// addnew
        {
            if (productList != null)
            {
                if (thread == null || !thread.IsAlive)
                {
                    EditDialog editDialog = new EditDialog();
                    if (editDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (editDialog.product != null)
                        {
                            productList.Products.Add(editDialog.product);
                            UpdateGrid();
                            IsValueChanged = true;
                            this.Text = filename + " *";
                        }
                        else
                        {
                            MessageBox.Show("Data not valid");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Calculation in procces", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedRow = DataGrid.Rows[e.RowIndex];
            }
            else
            {
                selectedRow = null;
            }
        }

        private void saveToMysqlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("server=localhost;user=root;database=mydb;port=3306;");
            conn.Open();
            string sql = string.Format("select * from product");
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            productList = new ProductList();
            productList.Products = new List<Product>();
            while(rdr.Read())
            {
            productList.Products.Add(new Product
            {
                Name = rdr[0].ToString(),
                Price = Convert.ToDouble(rdr[1].ToString()),
                Produser = rdr[2].ToString()
            });
            }
            rdr.Close();
            
            conn.Close();
            DataGrid.DataSource = productList.Products;
        }

        private void saveToMySqlToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("server=localhost;user=root;database=mydb;port=3306;");
            conn.Open();
            foreach (Product product in productList.Products)
            {
                 string sql = string.Format("insert into product values('{0}', {1}, '{2}')", product.Name, product.Price, product.Produser);
                 MySqlCommand cmd = new MySqlCommand(sql, conn);
                 MySqlDataReader rdr = cmd.ExecuteReader();
                 productList = new ProductList();
                 productList.Products = new List<Product>();
                 while (rdr.Read())
                 {

                 }
                 rdr.Close();
            }

            conn.Close();
        }

        private void clearMySqlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("server=localhost;user=root;database=mydb;port=3306;");
            conn.Open();
            string sql = "delete from product";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {

            }
            rdr.Close();
            

            conn.Close();
        }
    }
}
