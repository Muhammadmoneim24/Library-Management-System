using LMSProj.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LMSProj
{
    public partial class Member_Manage : Form
    {
        string connectionString = "Server=DESKTOP-MN6ULTF\\SSEXP;Database=LMSProj;Trusted_Connection=True;";
        BindingSource source;

        public Member_Manage()
        {
            InitializeComponent();
            source = new BindingSource();

            this.Shown += Member_Manage_Shown;
        }

        private void Member_Manage_Shown(object? sender, EventArgs e)
        {
            button1.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = LoadMembers();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Member_Service service = new Member_Service();
                service.DataUpdated += Service_DataUpdated;
                service.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the borrow service form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                MemberModel model = dataGridView1.CurrentRow.DataBoundItem as MemberModel;
                if (model != null)
                {
                    Member_Service service = new Member_Service(model);
                    service.DataUpdated += Service_DataUpdated;
                    service.Show();
                }
                else
                {
                    MessageBox.Show("No borrowing record selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the borrow service form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Service_DataUpdated()
        {
            button1.PerformClick();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Confirm Delete!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    MemberModel member = dataGridView1.CurrentRow.DataBoundItem as MemberModel;
                    if (member != null)
                    {
                        DeleteMember(member);
                        source.DataSource = new MemberModel();
                        button1.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show("No borrowing record selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the borrowing record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private List<MemberModel> LoadMembers()
        {
            try
            {
                var Query = "SELECT * FROM Members WHERE 1=1 ";

                DataTable table = new DataTable();
                List<MemberModel> members = new List<MemberModel>();

                string search = $"%{textBox1.Text.Trim()}%";

                if (!string.IsNullOrEmpty(search))
                {
                    Query += " AND (FirstName LIKE @search OR MemberID LIKE @search)";
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    command.Parameters.Add(new SqlParameter("@search", search));
                    dataAdapter.Fill(table);

                    foreach (DataRow row in table.Rows)
                    {
                        members.Add(new MemberModel()
                        {
                            MemberID = Convert.ToInt32(row["MemberID"]),
                            FirstName = row["FirstName"].ToString(),
                            LastName = row["LastName"].ToString(),
                            Email = row["Email"].ToString(),
                            PhoneNumber = row["PhoneNumber"].ToString(),
                            JoinDate = row["JoinDate"].ToString()
                        });
                    }
                    return members;
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<MemberModel>(); // Return empty list on error
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<MemberModel>(); // Return empty list on error
            }
        }

        private void DeleteMember(MemberModel member)
        {
            try
            {
                if (member == null)
                {
                    MessageBox.Show("No member selected for deletion.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var Query = @"DELETE FROM Members WHERE MemberID = @Id;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                {
                    command.Parameters.AddWithValue("@Id", member.MemberID);
                    conn.Open();
                    int rows = command.ExecuteNonQuery();
                    MessageBox.Show($"Deleted {rows} member(s).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
