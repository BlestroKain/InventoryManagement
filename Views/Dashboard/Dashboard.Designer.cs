namespace RapiMesa.Views.Dashboard
{
    partial class Dashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tableKpis = new System.Windows.Forms.TableLayoutPanel();
            this.grpSalesToday = new System.Windows.Forms.GroupBox();
            this.lblSalesToday = new System.Windows.Forms.Label();
            this.grpSalesMonth = new System.Windows.Forms.GroupBox();
            this.lblSalesMonth = new System.Windows.Forms.Label();
            this.grpAvgTicket = new System.Windows.Forms.GroupBox();
            this.lblAvgTicket = new System.Windows.Forms.Label();
            this.grpLowStock = new System.Windows.Forms.GroupBox();
            this.lblLowStock = new System.Windows.Forms.Label();
            this.grpInventoryValue = new System.Windows.Forms.GroupBox();
            this.lblInventoryValue = new System.Windows.Forms.Label();
            this.grpTopProduct = new System.Windows.Forms.GroupBox();
            this.lblTopProduct = new System.Windows.Forms.Label();
            this.splitCharts = new System.Windows.Forms.SplitContainer();
            this.chartSales7d = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartTop5 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblLastSync = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btn7d = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btn30d = new System.Windows.Forms.Button();
            this.btnMes = new System.Windows.Forms.Button();
            this.tableKpis.SuspendLayout();
            this.grpSalesToday.SuspendLayout();
            this.grpSalesMonth.SuspendLayout();
            this.grpAvgTicket.SuspendLayout();
            this.grpLowStock.SuspendLayout();
            this.grpInventoryValue.SuspendLayout();
            this.grpTopProduct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitCharts)).BeginInit();
            this.splitCharts.Panel1.SuspendLayout();
            this.splitCharts.Panel2.SuspendLayout();
            this.splitCharts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartSales7d)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTop5)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableKpis
            // 
            this.tableKpis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableKpis.ColumnCount = 6;
            this.tableKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.7F));
            this.tableKpis.Controls.Add(this.grpSalesToday, 0, 0);
            this.tableKpis.Controls.Add(this.grpSalesMonth, 1, 0);
            this.tableKpis.Controls.Add(this.grpAvgTicket, 2, 0);
            this.tableKpis.Controls.Add(this.grpLowStock, 3, 0);
            this.tableKpis.Controls.Add(this.grpInventoryValue, 4, 0);
            this.tableKpis.Controls.Add(this.grpTopProduct, 5, 0);
            this.tableKpis.Location = new System.Drawing.Point(10, 42);
            this.tableKpis.Margin = new System.Windows.Forms.Padding(3, 3, 3, 9);
            this.tableKpis.Name = "tableKpis";
            this.tableKpis.RowCount = 1;
            this.tableKpis.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableKpis.Size = new System.Drawing.Size(994, 95);
            this.tableKpis.TabIndex = 0;
            // 
            // grpSalesToday
            // 
            this.grpSalesToday.Controls.Add(this.lblSalesToday);
            this.grpSalesToday.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSalesToday.Location = new System.Drawing.Point(3, 3);
            this.grpSalesToday.Name = "grpSalesToday";
            this.grpSalesToday.Size = new System.Drawing.Size(159, 89);
            this.grpSalesToday.TabIndex = 0;
            this.grpSalesToday.TabStop = false;
            this.grpSalesToday.Text = "Sales Today";
            // 
            // lblSalesToday
            // 
            this.lblSalesToday.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSalesToday.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.lblSalesToday.Location = new System.Drawing.Point(3, 16);
            this.lblSalesToday.Name = "lblSalesToday";
            this.lblSalesToday.Size = new System.Drawing.Size(153, 70);
            this.lblSalesToday.TabIndex = 0;
            this.lblSalesToday.Text = "0";
            this.lblSalesToday.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpSalesMonth
            // 
            this.grpSalesMonth.Controls.Add(this.lblSalesMonth);
            this.grpSalesMonth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSalesMonth.Location = new System.Drawing.Point(168, 3);
            this.grpSalesMonth.Name = "grpSalesMonth";
            this.grpSalesMonth.Size = new System.Drawing.Size(159, 89);
            this.grpSalesMonth.TabIndex = 1;
            this.grpSalesMonth.TabStop = false;
            this.grpSalesMonth.Text = "Sales (This Month)";
            // 
            // lblSalesMonth
            // 
            this.lblSalesMonth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSalesMonth.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.lblSalesMonth.Location = new System.Drawing.Point(3, 16);
            this.lblSalesMonth.Name = "lblSalesMonth";
            this.lblSalesMonth.Size = new System.Drawing.Size(153, 70);
            this.lblSalesMonth.TabIndex = 0;
            this.lblSalesMonth.Text = "0";
            this.lblSalesMonth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpAvgTicket
            // 
            this.grpAvgTicket.Controls.Add(this.lblAvgTicket);
            this.grpAvgTicket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAvgTicket.Location = new System.Drawing.Point(333, 3);
            this.grpAvgTicket.Name = "grpAvgTicket";
            this.grpAvgTicket.Size = new System.Drawing.Size(159, 89);
            this.grpAvgTicket.TabIndex = 2;
            this.grpAvgTicket.TabStop = false;
            this.grpAvgTicket.Text = "Avg. Ticket";
            // 
            // lblAvgTicket
            // 
            this.lblAvgTicket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAvgTicket.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.lblAvgTicket.Location = new System.Drawing.Point(3, 16);
            this.lblAvgTicket.Name = "lblAvgTicket";
            this.lblAvgTicket.Size = new System.Drawing.Size(153, 70);
            this.lblAvgTicket.TabIndex = 0;
            this.lblAvgTicket.Text = "0";
            this.lblAvgTicket.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpLowStock
            // 
            this.grpLowStock.Controls.Add(this.lblLowStock);
            this.grpLowStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLowStock.Location = new System.Drawing.Point(498, 3);
            this.grpLowStock.Name = "grpLowStock";
            this.grpLowStock.Size = new System.Drawing.Size(159, 89);
            this.grpLowStock.TabIndex = 3;
            this.grpLowStock.TabStop = false;
            this.grpLowStock.Text = "Low Stock (≤5)";
            // 
            // lblLowStock
            // 
            this.lblLowStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLowStock.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.lblLowStock.Location = new System.Drawing.Point(3, 16);
            this.lblLowStock.Name = "lblLowStock";
            this.lblLowStock.Size = new System.Drawing.Size(153, 70);
            this.lblLowStock.TabIndex = 0;
            this.lblLowStock.Text = "0";
            this.lblLowStock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpInventoryValue
            // 
            this.grpInventoryValue.Controls.Add(this.lblInventoryValue);
            this.grpInventoryValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpInventoryValue.Location = new System.Drawing.Point(663, 3);
            this.grpInventoryValue.Name = "grpInventoryValue";
            this.grpInventoryValue.Size = new System.Drawing.Size(159, 89);
            this.grpInventoryValue.TabIndex = 4;
            this.grpInventoryValue.TabStop = false;
            this.grpInventoryValue.Text = "Inventory Value";
            // 
            // lblInventoryValue
            // 
            this.lblInventoryValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInventoryValue.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.lblInventoryValue.Location = new System.Drawing.Point(3, 16);
            this.lblInventoryValue.Name = "lblInventoryValue";
            this.lblInventoryValue.Size = new System.Drawing.Size(153, 70);
            this.lblInventoryValue.TabIndex = 0;
            this.lblInventoryValue.Text = "0";
            this.lblInventoryValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpTopProduct
            // 
            this.grpTopProduct.Controls.Add(this.lblTopProduct);
            this.grpTopProduct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTopProduct.Location = new System.Drawing.Point(828, 3);
            this.grpTopProduct.Name = "grpTopProduct";
            this.grpTopProduct.Size = new System.Drawing.Size(163, 89);
            this.grpTopProduct.TabIndex = 5;
            this.grpTopProduct.TabStop = false;
            this.grpTopProduct.Text = "Top Product";
            // 
            // lblTopProduct
            // 
            this.lblTopProduct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTopProduct.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);
            this.lblTopProduct.Location = new System.Drawing.Point(3, 16);
            this.lblTopProduct.Name = "lblTopProduct";
            this.lblTopProduct.Size = new System.Drawing.Size(157, 70);
            this.lblTopProduct.TabIndex = 0;
            this.lblTopProduct.Text = "—";
            this.lblTopProduct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitCharts
            // 
            this.splitCharts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitCharts.Location = new System.Drawing.Point(10, 149);
            this.splitCharts.Name = "splitCharts";
            // 
            // splitCharts.Panel1
            // 
            this.splitCharts.Panel1.Controls.Add(this.chartSales7d);
            // 
            // splitCharts.Panel2
            // 
            this.splitCharts.Panel2.Controls.Add(this.chartTop5);
            this.splitCharts.Size = new System.Drawing.Size(994, 348);
            this.splitCharts.SplitterDistance = 497;
            this.splitCharts.SplitterWidth = 3;
            this.splitCharts.TabIndex = 1;
            // 
            // chartSales7d
            // 
            this.chartSales7d.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartSales7d.Location = new System.Drawing.Point(0, 0);
            this.chartSales7d.Name = "chartSales7d";
            this.chartSales7d.Size = new System.Drawing.Size(497, 348);
            this.chartSales7d.TabIndex = 0;
            this.chartSales7d.Text = "chart1";
            // 
            // chartTop5
            // 
            this.chartTop5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartTop5.Location = new System.Drawing.Point(0, 0);
            this.chartTop5.Name = "chartTop5";
            this.chartTop5.Size = new System.Drawing.Size(494, 348);
            this.chartTop5.TabIndex = 0;
            this.chartTop5.Text = "chart2";
            // 
            // lblLastSync
            // 
            this.lblLastSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastSync.AutoSize = true;
            this.lblLastSync.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblLastSync.Location = new System.Drawing.Point(1011, 607);
            this.lblLastSync.Name = "lblLastSync";
            this.lblLastSync.Size = new System.Drawing.Size(73, 13);
            this.lblLastSync.TabIndex = 104;
            this.lblLastSync.Text = "Última sync: —";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 25;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btn7d, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn30d, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRefresh, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnMes, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 1);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 9);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(994, 34);
            this.tableLayoutPanel1.TabIndex = 105;
            // 
            // btn7d
            // 
            this.btn7d.Location = new System.Drawing.Point(3, 3);
            this.btn7d.Name = "btn7d";
            this.btn7d.Size = new System.Drawing.Size(85, 28);
            this.btn7d.TabIndex = 105;
            this.btn7d.Text = "7d";
            this.btn7d.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(467, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(102, 28);
            this.btnRefresh.TabIndex = 104;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btn30d
            // 
            this.btn30d.Location = new System.Drawing.Point(146, 3);
            this.btn30d.Name = "btn30d";
            this.btn30d.Size = new System.Drawing.Size(85, 28);
            this.btn30d.TabIndex = 106;
            this.btn30d.Text = "30d";
            this.btn30d.UseVisualStyleBackColor = true;
            // 
            // btnMes
            // 
            this.btnMes.Location = new System.Drawing.Point(289, 3);
            this.btnMes.Name = "btnMes";
            this.btnMes.Size = new System.Drawing.Size(85, 28);
            this.btnMes.TabIndex = 107;
            this.btnMes.Text = "Mes";
            this.btnMes.UseVisualStyleBackColor = true;
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 500);
            this.Controls.Add(this.lblLastSync);
            this.Controls.Add(this.splitCharts);
            this.Controls.Add(this.tableKpis);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("News706 BT", 12F, System.Drawing.FontStyle.Bold);

            this.Name = "Dashboard";
            this.Text = "Dashboard";
            this.tableKpis.ResumeLayout(false);
            this.grpSalesToday.ResumeLayout(false);
            this.grpSalesMonth.ResumeLayout(false);
            this.grpAvgTicket.ResumeLayout(false);
            this.grpLowStock.ResumeLayout(false);
            this.grpInventoryValue.ResumeLayout(false);
            this.grpTopProduct.ResumeLayout(false);
            this.splitCharts.Panel1.ResumeLayout(false);
            this.splitCharts.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitCharts)).EndInit();
            this.splitCharts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartSales7d)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTop5)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableKpis;
        private System.Windows.Forms.GroupBox grpSalesToday;
        private System.Windows.Forms.Label lblSalesToday;
        private System.Windows.Forms.GroupBox grpSalesMonth;
        private System.Windows.Forms.Label lblSalesMonth;
        private System.Windows.Forms.GroupBox grpAvgTicket;
        private System.Windows.Forms.Label lblAvgTicket;
        private System.Windows.Forms.GroupBox grpLowStock;
        private System.Windows.Forms.Label lblLowStock;
        private System.Windows.Forms.GroupBox grpInventoryValue;
        private System.Windows.Forms.Label lblInventoryValue;
        private System.Windows.Forms.GroupBox grpTopProduct;
        private System.Windows.Forms.Label lblTopProduct;
        private System.Windows.Forms.SplitContainer splitCharts;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSales7d;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTop5;
        private System.Windows.Forms.Label lblLastSync;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btn7d;
        private System.Windows.Forms.Button btnMes;
        private System.Windows.Forms.Button btn30d;
        private System.Windows.Forms.Button btnRefresh;
    }
}
