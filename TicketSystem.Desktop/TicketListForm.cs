using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TicketSystem.Desktop.Models;
using TicketSystem.Desktop.Services;

namespace TicketSystem.Desktop
{
    public partial class TicketListForm : Form
    {
        private readonly User _user;
        private readonly ApiService _apiService;
        private DataGridView dgvTickets;
        private Button btnRefresh;
        private Button btnCreate;
        private Button btnDetails;

        public TicketListForm(User user)
        {
            _user = user;
            _apiService = new ApiService();
            InitializeComponent();
            LoadTickets();
        }

        private void InitializeComponent()
        {
            this.dgvTickets = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnDetails = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).BeginInit();
            this.SuspendLayout();

            // btnCreate
            this.btnCreate.Location = new System.Drawing.Point(12, 12);
            this.btnCreate.Size = new System.Drawing.Size(100, 30);
            this.btnCreate.Text = "Create Ticket";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);

            // btnRefresh
            this.btnRefresh.Location = new System.Drawing.Point(120, 12);
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // btnDetails
            this.btnDetails.Location = new System.Drawing.Point(230, 12);
            this.btnDetails.Size = new System.Drawing.Size(100, 30);
            this.btnDetails.Text = "View Details";
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);

            // dgvTickets
            this.dgvTickets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTickets.Location = new System.Drawing.Point(12, 50);
            this.dgvTickets.Size = new System.Drawing.Size(760, 500);
            this.dgvTickets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTickets.MultiSelect = false;
            this.dgvTickets.ReadOnly = true;
            this.dgvTickets.AutoGenerateColumns = false;

            // Define Columns Manually to avoid messy auto-gen
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TicketNumber", HeaderText = "Ticket #" });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subject", HeaderText = "Subject", Width = 200 });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Priority", HeaderText = "Priority" });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status" });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CreatedAt", HeaderText = "Created Date", DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "AssignedToUsername", HeaderText = "Assigned To" });


            // TicketListForm
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.dgvTickets);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnDetails);
            this.Text = $"Tickets - {_user.Role}";
            this.StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).EndInit();
            this.ResumeLayout(false);
        }

        private async void LoadTickets()
        {
            try
            {
                int? userId = _user.Role == "Admin" ? (int?)null : _user.Id;
                var tickets = await _apiService.GetTicketsAsync(userId);
                dgvTickets.DataSource = tickets;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tickets: {ex.Message}");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadTickets();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            var createForm = new CreateTicketForm(_user);
            createForm.ShowDialog();
            LoadTickets();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (dgvTickets.SelectedRows.Count > 0)
            {
                var ticket = dgvTickets.SelectedRows[0].DataBoundItem as Ticket;
                if (ticket != null)
                {
                   var detailsForm = new TicketDetailsForm(ticket, _user);
                   detailsForm.ShowDialog();
                   LoadTickets();
                }
            }
            else
            {
                MessageBox.Show("Please select a ticket.");
            }
        }
    }
}
