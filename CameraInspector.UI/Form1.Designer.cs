namespace CameraInspector.UI;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.toolStrip1 = new System.Windows.Forms.ToolStrip();
        this.btnRefresh = new System.Windows.Forms.ToolStripButton();
        this.btnExport = new System.Windows.Forms.ToolStripButton();
        this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        this.dgvDevices = new System.Windows.Forms.DataGridView();
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabInfo = new System.Windows.Forms.TabPage();
        this.dgvInfo = new System.Windows.Forms.DataGridView();
        this.tabIntrinsics = new System.Windows.Forms.TabPage();
        this.dgvIntrinsics = new System.Windows.Forms.DataGridView();
        this.tabStreams = new System.Windows.Forms.TabPage();
        this.dgvStreams = new System.Windows.Forms.DataGridView();
        this.tabSettings = new System.Windows.Forms.TabPage();
        this.dgvSettings = new System.Windows.Forms.DataGridView();
        this.tabDiagnostics = new System.Windows.Forms.TabPage();
        this.dgvDiagnostics = new System.Windows.Forms.DataGridView();
        this.tabJson = new System.Windows.Forms.TabPage();
        this.txtJson = new System.Windows.Forms.TextBox();

        // ToolStrip
        this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.btnRefresh, this.btnExport });
        this.btnRefresh.Text = "Refresh Devices";
        this.btnExport.Text = "Export JSON";

        // SplitContainer
        this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer1.SplitterDistance = 300;

        // DgvDevices
        this.dgvDevices.Dock = System.Windows.Forms.DockStyle.Fill;
        this.dgvDevices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvDevices.MultiSelect = false;
        this.dgvDevices.AllowUserToAddRows = false;
        this.dgvDevices.ReadOnly = true;

        // TabControl
        this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabControl1.Controls.Add(this.tabInfo);
        this.tabControl1.Controls.Add(this.tabIntrinsics);
        this.tabControl1.Controls.Add(this.tabStreams);
        this.tabControl1.Controls.Add(this.tabSettings);
        this.tabControl1.Controls.Add(this.tabDiagnostics);
        this.tabControl1.Controls.Add(this.tabJson);

        // Init Tabs
        InitTab(this.tabInfo, "Info", this.dgvInfo);
        InitTab(this.tabIntrinsics, "Intrinsics", this.dgvIntrinsics);
        InitTab(this.tabStreams, "Streams", this.dgvStreams);
        InitTab(this.tabSettings, "Settings", this.dgvSettings);
        InitTab(this.tabDiagnostics, "Diagnostics", this.dgvDiagnostics);

        // Json Tab
        this.tabJson.Text = "Raw JSON";
        this.txtJson.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtJson.Multiline = true;
        this.txtJson.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.tabJson.Controls.Add(this.txtJson);

        // Form
        this.Controls.Add(this.splitContainer1);
        this.Controls.Add(this.toolStrip1);
        this.splitContainer1.Panel1.Controls.Add(this.dgvDevices);
        this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
        this.Size = new System.Drawing.Size(1000, 700);
        this.Text = "Camera Inspector (Zivid + RealSense)";
    }

    private void InitTab(System.Windows.Forms.TabPage page, string title, System.Windows.Forms.DataGridView dgv)
    {
        page.Text = title;
        dgv.Dock = System.Windows.Forms.DockStyle.Fill;
        dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        dgv.AllowUserToAddRows = false;
        dgv.ReadOnly = true;
        dgv.ColumnHeadersVisible = true;
        dgv.RowHeadersVisible = false;
        page.Controls.Add(dgv);
    }

    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripButton btnRefresh;
    private System.Windows.Forms.ToolStripButton btnExport;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.DataGridView dgvDevices;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabInfo;
    private System.Windows.Forms.DataGridView dgvInfo;
    private System.Windows.Forms.TabPage tabIntrinsics;
    private System.Windows.Forms.DataGridView dgvIntrinsics;
    private System.Windows.Forms.TabPage tabStreams;
    private System.Windows.Forms.DataGridView dgvStreams;
    private System.Windows.Forms.TabPage tabSettings;
    private System.Windows.Forms.DataGridView dgvSettings;
    private System.Windows.Forms.TabPage tabDiagnostics;
    private System.Windows.Forms.DataGridView dgvDiagnostics;
    private System.Windows.Forms.TabPage tabJson;
    private System.Windows.Forms.TextBox txtJson;
}
