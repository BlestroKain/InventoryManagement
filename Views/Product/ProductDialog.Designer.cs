namespace RapiMesa
{
    partial class ProductDialog
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
            this.components = new System.ComponentModel.Container();
            this.NameTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PriceTxt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.StockTxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.UnitTxt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CategoryCmb = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Savebtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // NameTxt
            // 
            this.NameTxt.Location = new System.Drawing.Point(78, 44);
            this.NameTxt.Name = "NameTxt";
            this.NameTxt.Size = new System.Drawing.Size(274, 20);
            this.NameTxt.TabIndex = 0;
            this.NameTxt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
  
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nombre";
            // 
            // PriceTxt
            // 
            this.PriceTxt.Location = new System.Drawing.Point(78, 89);
            this.PriceTxt.Name = "PriceTxt";
            this.PriceTxt.Size = new System.Drawing.Size(274, 20);
            this.PriceTxt.TabIndex = 2;
            this.PriceTxt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
        
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Precio";
            // 
            // StockTxt
            // 
            this.StockTxt.Location = new System.Drawing.Point(78, 132);
            this.StockTxt.Name = "StockTxt";
            this.StockTxt.Size = new System.Drawing.Size(274, 20);
            this.StockTxt.TabIndex = 4;
            this.StockTxt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox3_KeyPress);

            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Stock";
            // 
            // UnitTxt
            // 
            this.UnitTxt.Location = new System.Drawing.Point(78, 174);
            this.UnitTxt.Name = "UnitTxt";
            this.UnitTxt.Size = new System.Drawing.Size(274, 20);
            this.UnitTxt.TabIndex = 6;
            this.UnitTxt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox4_KeyPress);
    
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Unidad";
            // 
            // CategoryCmb
            // 
            this.CategoryCmb.FormattingEnabled = true;
            this.CategoryCmb.Location = new System.Drawing.Point(78, 219);
            this.CategoryCmb.Name = "CategoryCmb";
            this.CategoryCmb.Size = new System.Drawing.Size(274, 21);
            this.CategoryCmb.TabIndex = 8;
            this.CategoryCmb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox1_KeyPress);

            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Categoría";
            // 
            // Savebtn
            // 
            this.Savebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Savebtn.Location = new System.Drawing.Point(52, 282);
            this.Savebtn.Name = "Savebtn";
            this.Savebtn.Size = new System.Drawing.Size(111, 41);
            this.Savebtn.TabIndex = 10;
            this.Savebtn.Text = "GUARDAR";
            this.Savebtn.UseVisualStyleBackColor = false;
            this.Savebtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.CancelBtn.Location = new System.Drawing.Point(225, 282);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(111, 41);
            this.CancelBtn.TabIndex = 11;
            this.CancelBtn.Text = "CANCELAR";
            this.CancelBtn.UseVisualStyleBackColor = false;
            this.CancelBtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(369, 344);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
            // 
            // ProductDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 368);
            this.ControlBox = false;
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.Savebtn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.CategoryCmb);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.UnitTxt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.StockTxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PriceTxt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NameTxt);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ProductDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Producto";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox NameTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PriceTxt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox StockTxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox UnitTxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox CategoryCmb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button Savebtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}