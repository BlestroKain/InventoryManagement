namespace RapiMesa
{
    partial class Checkout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.CashTxt = new System.Windows.Forms.TextBox();
            this.CashLbl = new System.Windows.Forms.Label();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.PayBtn = new System.Windows.Forms.Button();
            this.CheckoutGrp = new System.Windows.Forms.GroupBox();
            this.ChangeLbl = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.DiscountCmb = new System.Windows.Forms.ComboBox();
            this.TotalLbl = new System.Windows.Forms.Label();
            this.DiscountLbl = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SubtotalLbl = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.CheckoutGrp.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(24, 303);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Total:";
            // 
            // CashTxt
            // 
            this.CashTxt.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.CashTxt.Location = new System.Drawing.Point(103, 143);
            this.CashTxt.Name = "CashTxt";
            this.CashTxt.Size = new System.Drawing.Size(237, 27);
            this.CashTxt.TabIndex = 2;
            this.CashTxt.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // CashLbl
            // 
            this.CashLbl.AutoSize = true;
            this.CashLbl.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.CashLbl.Location = new System.Drawing.Point(25, 146);
            this.CashLbl.Name = "CashLbl";
            this.CashLbl.Size = new System.Drawing.Size(53, 19);
            this.CashLbl.TabIndex = 3;
            this.CashLbl.Text = "Cash:";
            // 
            // CancelBtn
            // 
            this.CancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.CancelBtn.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.CancelBtn.Location = new System.Drawing.Point(52, 392);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(115, 41);
            this.CancelBtn.TabIndex = 10;
            this.CancelBtn.Text = "CANCELAR";
            this.CancelBtn.UseVisualStyleBackColor = false;
            this.CancelBtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // PayBtn
            // 
            this.PayBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.PayBtn.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.PayBtn.Location = new System.Drawing.Point(214, 380);
            this.PayBtn.Name = "PayBtn";
            this.PayBtn.Size = new System.Drawing.Size(111, 41);
            this.PayBtn.TabIndex = 11;
            this.PayBtn.Text = "PAGAR";
            this.PayBtn.UseVisualStyleBackColor = false;
            this.PayBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // CheckoutGrp
            // 
            this.CheckoutGrp.BackColor = System.Drawing.SystemColors.Control;
            this.CheckoutGrp.Controls.Add(this.ChangeLbl);
            this.CheckoutGrp.Controls.Add(this.label9);
            this.CheckoutGrp.Controls.Add(this.DiscountCmb);
            this.CheckoutGrp.Controls.Add(this.TotalLbl);
            this.CheckoutGrp.Controls.Add(this.DiscountLbl);
            this.CheckoutGrp.Controls.Add(this.label6);
            this.CheckoutGrp.Controls.Add(this.label5);
            this.CheckoutGrp.Controls.Add(this.listBox1);
            this.CheckoutGrp.Controls.Add(this.SubtotalLbl);
            this.CheckoutGrp.Controls.Add(this.label1);
            this.CheckoutGrp.Controls.Add(this.CashLbl);
            this.CheckoutGrp.Controls.Add(this.label4);
            this.CheckoutGrp.Controls.Add(this.CashTxt);
            this.CheckoutGrp.Controls.Add(this.PayBtn);
            this.CheckoutGrp.Location = new System.Drawing.Point(12, 12);
            this.CheckoutGrp.Name = "CheckoutGrp";
            this.CheckoutGrp.Size = new System.Drawing.Size(369, 449);
            this.CheckoutGrp.TabIndex = 12;
            this.CheckoutGrp.TabStop = false;
            // 
            // ChangeLbl
            // 
            this.ChangeLbl.AutoSize = true;
            this.ChangeLbl.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.ChangeLbl.Location = new System.Drawing.Point(249, 341);
            this.ChangeLbl.Name = "ChangeLbl";
            this.ChangeLbl.Size = new System.Drawing.Size(17, 19);
            this.ChangeLbl.TabIndex = 22;
            this.ChangeLbl.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(24, 341);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 19);
            this.label9.TabIndex = 21;
            this.label9.Text = "Cambio";
            // 
            // DiscountCmb
            // 
            this.DiscountCmb.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.DiscountCmb.FormattingEnabled = true;
            this.DiscountCmb.Location = new System.Drawing.Point(104, 179);
            this.DiscountCmb.Name = "DiscountCmb";
            this.DiscountCmb.Size = new System.Drawing.Size(236, 27);
            this.DiscountCmb.TabIndex = 20;
            this.DiscountCmb.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // TotalLbl
            // 
            this.TotalLbl.AutoSize = true;
            this.TotalLbl.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.TotalLbl.Location = new System.Drawing.Point(249, 303);
            this.TotalLbl.Name = "TotalLbl";
            this.TotalLbl.Size = new System.Drawing.Size(17, 19);
            this.TotalLbl.TabIndex = 19;
            this.TotalLbl.Text = "0";
            this.TotalLbl.TextChanged += new System.EventHandler(this.label8_TextChanged);
            // 
            // DiscountLbl
            // 
            this.DiscountLbl.AutoSize = true;
            this.DiscountLbl.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.DiscountLbl.Location = new System.Drawing.Point(249, 264);
            this.DiscountLbl.Name = "DiscountLbl";
            this.DiscountLbl.Size = new System.Drawing.Size(17, 19);
            this.DiscountLbl.TabIndex = 18;
            this.DiscountLbl.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(25, 264);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 19);
            this.label6.TabIndex = 17;
            this.label6.Text = "Descuento:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(24, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 19);
            this.label5.TabIndex = 16;
            this.label5.Text = "Subtotal:";
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 19;
            this.listBox1.Location = new System.Drawing.Point(6, 20);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(357, 99);
            this.listBox1.TabIndex = 15;
            // 
            // SubtotalLbl
            // 
            this.SubtotalLbl.AutoSize = true;
            this.SubtotalLbl.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.SubtotalLbl.Location = new System.Drawing.Point(249, 225);
            this.SubtotalLbl.Name = "SubtotalLbl";
            this.SubtotalLbl.Size = new System.Drawing.Size(17, 19);
            this.SubtotalLbl.TabIndex = 0;
            this.SubtotalLbl.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(24, 182);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 19);
            this.label4.TabIndex = 14;
            this.label4.Text = "Promo:";
            // 
            // Checkout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 473);
            this.ControlBox = false;
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.CheckoutGrp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Checkout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Checkout";
            this.CheckoutGrp.ResumeLayout(false);
            this.CheckoutGrp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CashTxt;
        private System.Windows.Forms.Label CashLbl;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button PayBtn;
        private System.Windows.Forms.GroupBox CheckoutGrp;
        private System.Windows.Forms.Label SubtotalLbl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label TotalLbl;
        private System.Windows.Forms.Label DiscountLbl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ComboBox DiscountCmb;
        private System.Windows.Forms.Label ChangeLbl;
        private System.Windows.Forms.Label label9;
    }
}