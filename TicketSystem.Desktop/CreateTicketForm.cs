using System;
using System.Windows.Forms;
using TicketSystem.Desktop.Models;
using TicketSystem.Desktop.Services;

namespace TicketSystem.Desktop
{
    public partial class CreateTicketForm : Form
    {
        private readonly User _user;
        private readonly ApiService _apiService;
        private TextBox txtSubject;
        private TextBox txtDescription;
        private ComboBox cmbPriority;
        private Label lblSubject;
        private Label lblDesc;
        private Label lblPriority;
        private Button btnSubmit;

        public CreateTicketForm(User user)
        {
            _user = user;
            _apiService = new ApiService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            txtSubject = new TextBox();
            txtDescription = new TextBox();
            cmbPriority = new ComboBox();
            btnSubmit = new Button();
            lblSubject = new Label();
            lblDesc = new Label();
            lblPriority = new Label();
            SuspendLayout();
            // 
            // txtSubject
            // 
            txtSubject.Location = new Point(120, 17);
            txtSubject.Name = "txtSubject";
            txtSubject.Size = new Size(200, 27);
            txtSubject.TabIndex = 1;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(20, 85);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(300, 100);
            txtDescription.TabIndex = 3;
            // 
            // cmbPriority
            // 
            cmbPriority.Items.AddRange(new object[] { "Low", "Medium", "High" });
            cmbPriority.Location = new Point(120, 197);
            cmbPriority.Name = "cmbPriority";
            cmbPriority.Size = new Size(121, 28);
            cmbPriority.TabIndex = 5;
            // 
            // btnSubmit
            // 
            btnSubmit.Location = new Point(120, 240);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(85, 36);
            btnSubmit.TabIndex = 6;
            btnSubmit.Text = "Submit";
            btnSubmit.Click += btnSubmit_Click;
            // 
            // lblSubject
            // 
            lblSubject.Location = new Point(20, 20);
            lblSubject.Name = "lblSubject";
            lblSubject.Size = new Size(100, 23);
            lblSubject.TabIndex = 0;
            lblSubject.Text = "Subject:";
            // 
            // lblDesc
            // 
            lblDesc.Location = new Point(20, 60);
            lblDesc.Name = "lblDesc";
            lblDesc.Size = new Size(100, 23);
            lblDesc.TabIndex = 2;
            lblDesc.Text = "Description:";
            // 
            // lblPriority
            // 
            lblPriority.Location = new Point(20, 200);
            lblPriority.Name = "lblPriority";
            lblPriority.Size = new Size(100, 23);
            lblPriority.TabIndex = 4;
            lblPriority.Text = "Priority:";
            // 
            // CreateTicketForm
            // 
            ClientSize = new Size(350, 300);
            Controls.Add(lblSubject);
            Controls.Add(txtSubject);
            Controls.Add(lblDesc);
            Controls.Add(txtDescription);
            Controls.Add(lblPriority);
            Controls.Add(cmbPriority);
            Controls.Add(btnSubmit);
            Name = "CreateTicketForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Create Ticket";
            ResumeLayout(false);
            PerformLayout();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubject.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            var request = new 
            {
                Subject = txtSubject.Text,
                Description = txtDescription.Text,
                Priority = cmbPriority.SelectedItem.ToString(),
                UserId = _user.Id
            };

            try
            {
                var result = await _apiService.CreateTicketAsync(request);
                if (result != null)
                {
                    MessageBox.Show("Ticket Created!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create ticket.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
