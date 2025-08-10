namespace RapiMesa
{
    partial class Product
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Product));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.AddBtn = new System.Windows.Forms.Button();
            this.EditBtn = new System.Windows.Forms.Button();
            this.DeleteBtn = new System.Windows.Forms.Button();
            this.ExportBtn = new System.Windows.Forms.Button();
            this.AddStockBtn = new System.Windows.Forms.Button();
            this.HistoryBtn = new System.Windows.Forms.Button();
            this.SearchBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.labelCategory = new System.Windows.Forms.Label();
            this.comboBoxCategory = new System.Windows.Forms.ComboBox();
            this.labelMinPrice = new System.Windows.Forms.Label();
            this.numericMinPrice = new System.Windows.Forms.NumericUpDown();
            this.labelMaxPrice = new System.Windows.Forms.Label();
            this.numericMaxPrice = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // AddBtn
            // 
            this.AddBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddBtn.Image = ((System.Drawing.Image)(resources.GetObject("AddBtn.Image")));
            this.AddBtn.Location = new System.Drawing.Point(1120, 99);
            this.AddBtn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(150, 60);
            this.AddBtn.TabIndex = 0;
            this.toolTip1.SetToolTip(this.AddBtn, "Agregar");
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.Add_Click);
            // 
            // EditBtn
            // 
            this.EditBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EditBtn.Image = ((System.Drawing.Image)(resources.GetObject("EditBtn.Image")));
            this.EditBtn.Location = new System.Drawing.Point(1120, 159);
            this.EditBtn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.EditBtn.Name = "EditBtn";
            this.EditBtn.Size = new System.Drawing.Size(150, 60);
            this.EditBtn.TabIndex = 1;
            this.toolTip1.SetToolTip(this.EditBtn, "Editar");
            this.EditBtn.UseVisualStyleBackColor = true;
            this.EditBtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // DeleteBtn
            // 
            this.DeleteBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteBtn.Image = ((System.Drawing.Image)(resources.GetObject("DeleteBtn.Image")));
            this.DeleteBtn.Location = new System.Drawing.Point(1120, 221);
            this.DeleteBtn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.DeleteBtn.Name = "DeleteBtn";
            this.DeleteBtn.Size = new System.Drawing.Size(150, 60);
            this.DeleteBtn.TabIndex = 2;
            this.toolTip1.SetToolTip(this.DeleteBtn, "Eliminar");
            this.DeleteBtn.UseVisualStyleBackColor = true;
            this.DeleteBtn.Click += new System.EventHandler(this.DeleteBtn_Click);
            // 
            // ExportBtn
            // 
            this.ExportBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExportBtn.Image = ((System.Drawing.Image)(resources.GetObject("ExportBtn.Image")));
            this.ExportBtn.Location = new System.Drawing.Point(1120, 502);
            this.ExportBtn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.ExportBtn.Name = "ExportBtn";
            this.ExportBtn.Size = new System.Drawing.Size(150, 60);
            this.ExportBtn.TabIndex = 13;
            this.toolTip1.SetToolTip(this.ExportBtn, "Exportar");
            this.ExportBtn.UseVisualStyleBackColor = true;
            this.ExportBtn.Click += new System.EventHandler(this.ExportBtn_Click);
            // 
            // AddStockBtn
            // 
            this.AddStockBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddStockBtn.Image = ((System.Drawing.Image)(resources.GetObject("AddStockBtn.Image")));
            this.AddStockBtn.Location = new System.Drawing.Point(1120, 570);
            this.AddStockBtn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.AddStockBtn.Name = "AddStockBtn";
            this.AddStockBtn.Size = new System.Drawing.Size(150, 60);
            this.AddStockBtn.TabIndex = 4;
            this.toolTip1.SetToolTip(this.AddStockBtn, "Agregar stock");
            this.AddStockBtn.UseVisualStyleBackColor = true;
            this.AddStockBtn.Click += new System.EventHandler(this.AddStockBtn_Click);
            // 
            // HistoryBtn
            // 
            this.HistoryBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HistoryBtn.Image = ((System.Drawing.Image)(resources.GetObject("HistoryBtn.Image")));
            this.HistoryBtn.Location = new System.Drawing.Point(1120, 638);
            this.HistoryBtn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.HistoryBtn.Name = "HistoryBtn";
            this.HistoryBtn.Size = new System.Drawing.Size(150, 60);
            this.HistoryBtn.TabIndex = 5;
            this.toolTip1.SetToolTip(this.HistoryBtn, "Historial");
            this.HistoryBtn.UseVisualStyleBackColor = true;
            this.HistoryBtn.Click += new System.EventHandler(this.button5_Click);
            // 
            // SearchBtn
            // 
            this.SearchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchBtn.Location = new System.Drawing.Point(950, 34);
            this.SearchBtn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(125, 34);
            this.SearchBtn.TabIndex = 0;
            this.SearchBtn.Text = "BUSCAR";
            this.toolTip1.SetToolTip(this.SearchBtn, "Buscar");
            this.SearchBtn.UseVisualStyleBackColor = true;
            this.SearchBtn.Click += new System.EventHandler(this.button6_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Location = new System.Drawing.Point(20, 86);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBox1.Size = new System.Drawing.Size(1090, 628);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lista de Productos";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.MediumSeaGreen;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(22, 29);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 32;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1047, 577);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(82, 38);
            this.textBox1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(856, 27);
            this.textBox1.TabIndex = 6;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // labelCategory
            // 
            this.labelCategory.AutoSize = true;
            this.labelCategory.Location = new System.Drawing.Point(20, 10);
            this.labelCategory.Name = "labelCategory";
            this.labelCategory.Size = new System.Drawing.Size(90, 19);
            this.labelCategory.TabIndex = 7;
            this.labelCategory.Text = "Categoría:";
            // 
            // comboBoxCategory
            // 
            this.comboBoxCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCategory.Location = new System.Drawing.Point(111, 6);
            this.comboBoxCategory.Name = "comboBoxCategory";
            this.comboBoxCategory.Size = new System.Drawing.Size(160, 27);
            this.comboBoxCategory.TabIndex = 8;
            this.comboBoxCategory.SelectedIndexChanged += new System.EventHandler(this.FilterChanged);
            // 
            // labelMinPrice
            // 
            this.labelMinPrice.AutoSize = true;
            this.labelMinPrice.Location = new System.Drawing.Point(290, 10);
            this.labelMinPrice.Name = "labelMinPrice";
            this.labelMinPrice.Size = new System.Drawing.Size(100, 19);
            this.labelMinPrice.TabIndex = 9;
            this.labelMinPrice.Text = "Precio Min:";
            // 
            // numericMinPrice
            // 
            this.numericMinPrice.Location = new System.Drawing.Point(386, 8);
            this.numericMinPrice.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericMinPrice.Name = "numericMinPrice";
            this.numericMinPrice.Size = new System.Drawing.Size(90, 27);
            this.numericMinPrice.TabIndex = 10;
            this.numericMinPrice.ValueChanged += new System.EventHandler(this.FilterChanged);
            // 
            // labelMaxPrice
            // 
            this.labelMaxPrice.AutoSize = true;
            this.labelMaxPrice.Location = new System.Drawing.Point(486, 10);
            this.labelMaxPrice.Name = "labelMaxPrice";
            this.labelMaxPrice.Size = new System.Drawing.Size(103, 19);
            this.labelMaxPrice.TabIndex = 11;
            this.labelMaxPrice.Text = "Precio Max:";
            // 
            // numericMaxPrice
            // 
            this.numericMaxPrice.Location = new System.Drawing.Point(586, 8);
            this.numericMaxPrice.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericMaxPrice.Name = "numericMaxPrice";
            this.numericMaxPrice.Size = new System.Drawing.Size(90, 27);
            this.numericMaxPrice.TabIndex = 12;
            this.numericMaxPrice.ValueChanged += new System.EventHandler(this.FilterChanged);
            // 
            // Product
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1290, 732);
            this.Controls.Add(this.numericMaxPrice);
            this.Controls.Add(this.labelMaxPrice);
            this.Controls.Add(this.numericMinPrice);
            this.Controls.Add(this.labelMinPrice);
            this.Controls.Add(this.comboBoxCategory);
            this.Controls.Add(this.labelCategory);
            this.Controls.Add(this.SearchBtn);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.ExportBtn);
            this.Controls.Add(this.HistoryBtn);
            this.Controls.Add(this.AddStockBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.DeleteBtn);
            this.Controls.Add(this.EditBtn);
            this.Controls.Add(this.AddBtn);
            this.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Product";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Producto";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxPrice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.Button EditBtn;
        private System.Windows.Forms.Button DeleteBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button AddStockBtn;
        private System.Windows.Forms.Button HistoryBtn;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button SearchBtn;
        private System.Windows.Forms.Label labelCategory;
        private System.Windows.Forms.ComboBox comboBoxCategory;
        private System.Windows.Forms.Label labelMinPrice;
        private System.Windows.Forms.NumericUpDown numericMinPrice;
        private System.Windows.Forms.Label labelMaxPrice;
        private System.Windows.Forms.NumericUpDown numericMaxPrice;
        private System.Windows.Forms.Button ExportBtn;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

