using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgrammingTask
{
    public partial class EditDialog : Form
    {
        DataGridViewRow currentrow;

        public Product product { get; private set; }

        public EditDialog()
        {
            InitializeComponent();
            okButton.DialogResult = DialogResult.OK;
            cancelButton.DialogResult = DialogResult.Cancel;
        }

        public EditDialog(DataGridViewRow row)
        {
            InitializeComponent();
            currentrow = row;
            okButton.DialogResult = DialogResult.OK;
            cancelButton.DialogResult = DialogResult.Cancel;
            nameBox.Text = row.Cells["Name"].Value.ToString();
            producerBox.Text = row.Cells["Produser"].Value.ToString();
            PriceBox.Text = row.Cells["Price"].Value.ToString();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try 
            {
                double price = Convert.ToDouble(PriceBox.Text);
                if (price < 0)
                {
                    product = null;
                }
                else
                {
                    product = new Product(nameBox.Text, producerBox.Text, price);
                }
            }
            catch(Exception)
            {
                product = null;
            }
         }
    }
}
