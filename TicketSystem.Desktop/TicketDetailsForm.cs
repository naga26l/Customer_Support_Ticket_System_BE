using System;
using System.Windows.Forms;
using TicketSystem.Desktop.Models;
using TicketSystem.Desktop.Services;

namespace TicketSystem.Desktop
{
    public partial class TicketDetailsForm : Form
    {
        private Ticket _ticket;
        private readonly User _user;
        private readonly ApiService _apiService;
        private Label lblInfo;
        private TextBox txtDesc;
        
        // Admin Controls
        private GroupBox grpAdmin;
        private ComboBox cmbAssign; // Changed from TextBox
        private Button btnAssign;
        private ComboBox cmbStatus;
        private Button btnUpdateStatus;

        // History & Comments
        private DataGridView dgvHistory;
        private TextBox txtComment;
        private Button btnAddComment;

        public TicketDetailsForm(Ticket ticket, User user)
        {
            _ticket = ticket;
            _user = user;
            _apiService = new ApiService();
            InitializeComponent();
            PopulateData();
            LoadHistory();
            if (_user.Role == "Admin") LoadAdmins();
        }

        private void InitializeComponent()
        {
            this.lblInfo = new Label();
            this.txtDesc = new TextBox();
            this.grpAdmin = new GroupBox();
            this.cmbAssign = new ComboBox();
            this.btnAssign = new Button();
            this.cmbStatus = new ComboBox();
            this.btnUpdateStatus = new Button();
            
            // History
            this.dgvHistory = new DataGridView();
            this.txtComment = new TextBox();
            this.btnAddComment = new Button();

            this.SuspendLayout();

            // Info Label
            lblInfo.AutoSize = true;
            lblInfo.Location = new System.Drawing.Point(20, 20);
            
            // Description
            txtDesc.Location = new System.Drawing.Point(20, 100);
            txtDesc.Size = new System.Drawing.Size(340, 60); // Reduced height
            txtDesc.Multiline = true;
            txtDesc.ReadOnly = true;
            txtDesc.ScrollBars = ScrollBars.Vertical;

            // Admin Group
            grpAdmin.Text = "Admin Actions";
            grpAdmin.Location = new System.Drawing.Point(20, 170);
            grpAdmin.Size = new System.Drawing.Size(340, 100);
            
            var lblAssign = new Label { Text = "Assign To:", Location = new System.Drawing.Point(10, 30), AutoSize = true };
            cmbAssign.Location = new System.Drawing.Point(80, 27); // Replaced TextBox
            cmbAssign.Size = new System.Drawing.Size(120, 20);
            cmbAssign.DropDownStyle = ComboBoxStyle.DropDownList;
            
            btnAssign.Text = "Assign";
            btnAssign.Location = new System.Drawing.Point(210, 25);
            btnAssign.Click += btnAssign_Click;

            var lblStatus = new Label { Text = "Status:", Location = new System.Drawing.Point(10, 65), AutoSize = true };
            cmbStatus.Items.AddRange(new object[] { "Open", "In Progress", "Closed" });
            cmbStatus.Location = new System.Drawing.Point(80, 62);
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            
            btnUpdateStatus.Text = "Update";
            btnUpdateStatus.Location = new System.Drawing.Point(210, 60);
            btnUpdateStatus.Click += btnUpdateStatus_Click;

            grpAdmin.Controls.Add(lblAssign);
            grpAdmin.Controls.Add(cmbAssign);
            grpAdmin.Controls.Add(btnAssign);
            grpAdmin.Controls.Add(lblStatus);
            grpAdmin.Controls.Add(cmbStatus);
            grpAdmin.Controls.Add(btnUpdateStatus);

            // History Grid
            var lblHistory = new Label { Text = "History & Comments:", Location = new System.Drawing.Point(20, 280), AutoSize = true };
            dgvHistory.Location = new System.Drawing.Point(20, 300);
            dgvHistory.Size = new System.Drawing.Size(340, 150);
            dgvHistory.ReadOnly = true;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // Columns
             dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ChangedAt", HeaderText = "Date", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } });
             dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ChangedBy", HeaderText = "User", Width = 80 });
             dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Content", HeaderText = "Details" });

            // Add Comment
            txtComment.Location = new System.Drawing.Point(20, 460);
            txtComment.Size = new System.Drawing.Size(250, 40);
            txtComment.Multiline = true;
            
            btnAddComment.Text = "Add Comment";
            btnAddComment.Location = new System.Drawing.Point(280, 460);
            btnAddComment.Size = new System.Drawing.Size(80, 40);
            btnAddComment.Click += btnAddComment_Click;

            this.ClientSize = new System.Drawing.Size(380, 520); // Resized
            this.Controls.Add(lblInfo);
            this.Controls.Add(txtDesc);
            this.Controls.Add(grpAdmin);
            this.Controls.Add(lblHistory);
            this.Controls.Add(dgvHistory);
            this.Controls.Add(txtComment);
            this.Controls.Add(btnAddComment);
            
            this.Text = "Ticket Details";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void PopulateData()
        {
            lblInfo.Text = $"Ticket #: {_ticket.TicketNumber}\n" +
                           $"Subject: {_ticket.Subject}\n" +
                           $"Priority: {_ticket.Priority}\n" +
                           $"Status: {_ticket.Status}\n" +
                           $"Assigned To: {_ticket.AssignedToUsername ?? "Unassigned"}";
            
            txtDesc.Text = _ticket.Description;
            grpAdmin.Visible = _user.Role == "Admin";
            cmbStatus.SelectedItem = _ticket.Status;
        }

        private async void LoadHistory()
        {
            try
            {
                var history = await _apiService.GetTicketHistoryAsync(_ticket.Id);
                dgvHistory.DataSource = history;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error loading history: {ex.Message}");
            }
        }

        private async void LoadAdmins()
        {
             try
            {
                var admins = await _apiService.GetAdminsAsync();
                cmbAssign.DataSource = admins;
                cmbAssign.DisplayMember = "Username";
                cmbAssign.ValueMember = "Id";
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error loading admins: {ex.Message}");
            }
        }

        private async void btnAssign_Click(object sender, EventArgs e)
        {
            if (cmbAssign.SelectedValue is int adminId)
            {
                var success = await _apiService.AssignTicketAsync(_ticket.Id, adminId);
                if (success) 
                {
                    MessageBox.Show("Assigned!");
                    LoadHistory(); 
                    // Ideally refresh ticket info too to update "Assigned To" label
                }
                else MessageBox.Show("Failed.");
            }
        }

        private async void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (cmbStatus.SelectedItem != null)
            {
                var status = cmbStatus.SelectedItem.ToString();
                var success = await _apiService.UpdateStatusAsync(_ticket.Id, status, _user.Id);
                if (success) 
                {
                    MessageBox.Show("Status Updated!");
                     LoadHistory();
                     // Ideally refresh ticket info too
                }
                else MessageBox.Show("Failed.");
            }
        }

        private async void btnAddComment_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtComment.Text)) return;
            var success = await _apiService.AddCommentAsync(_ticket.Id, txtComment.Text, _user.Id);
            if(success)
            {
                txtComment.Text = "";
                LoadHistory();
            }
            else MessageBox.Show("Failed to add comment.");
        }
    }
}
