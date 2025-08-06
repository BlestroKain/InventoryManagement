using System;
using InventoryApp.Data;
using System.Windows.Forms;
using InventoryApp.Utility;

namespace InventoryApp
{
    public partial class Checkout : Form
    {
        private readonly PointOfSale pointOfSale;
        public Checkout(decimal totalPrice)
        {
            InitializeComponent();

            pointOfSale = new PointOfSale();
            CartManager cartManager = new CartManager();

            SubtotalLbl.Text = totalPrice.ToString();

            pointOfSale.InitializeComboBox(DiscountCmb);
            pointOfSale.CalculateDiscount(SubtotalLbl.Text, DiscountCmb.SelectedItem, DiscountLbl, TotalLbl);
            cartManager.LoadCartItems(listBox1);
        }

        // ON TEXT CHANGED
        public void ChangeEventHandler()
        {
            if (decimal.TryParse(CashTxt.Text, out _))
            {
                pointOfSale.CalculateChange(TotalLbl, CashTxt, ChangeLbl);
            }
        }

        // COMBOBOX EVENT
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pointOfSale.CalculateDiscount(SubtotalLbl.Text, DiscountCmb.SelectedItem, DiscountLbl, TotalLbl);
        }

        // TEXTBOX EVENT
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ChangeEventHandler();
        }

        // LABEL8 EVENT
        private void label8_TextChanged(object sender, EventArgs e)
        {
            ChangeEventHandler();
        }

        // INSERT STOCK BUTTON
        private void button1_Click(object sender, EventArgs e)
        {
            TransactionIdGenerator transactionIdGenerator = new TransactionIdGenerator();
            string transactionId = transactionIdGenerator.GenerateTransactionId();

            if (pointOfSale.ProcessTransaction(SubtotalLbl.Text, CashTxt.Text, DiscountCmb.SelectedItem, transactionId, listBox1))
            {
                DialogResult = DialogResult.OK;
            }
        }

        // CANCEL BUTTON
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
