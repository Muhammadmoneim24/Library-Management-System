using LMSProj.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LMSProj
{
    public partial class Member_Service : Form
    {

        string connectionString = "Server=DESKTOP-MN6ULTF\\SSEXP;Database=LMSProj;Trusted_Connection=True;";
        BindingSource source;

        public event Action DataUpdated;

        public Member_Service(MemberModel member) : this()
        {
            source.DataSource = member;

        }
        public Member_Service()
        {
            InitializeComponent();


            source = new BindingSource();
            MemberModel book = new MemberModel();
            source.DataSource = book;

            textBox1.DataBindings.Add("Text", source, "MemberID");
            textBox2.DataBindings.Add("Text", source, "FirstName");
            textBox3.DataBindings.Add("Text", source, "LastName");
            textBox4.DataBindings.Add("Text", source, "Email");
            textBox5.DataBindings.Add("Text", source, "PhoneNumber");
            textBox6.DataBindings.Add("Text", source, "JoinDate");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MemberModel model = source.Current as MemberModel;
                if (model == null)
                {
                    MessageBox.Show("No member selected or member data is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (model.MemberID == 0)
                    AddMember(model);
                else
                    EditMember(model);

                DataUpdated?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddMember(MemberModel member)
        {
            try
            {
                // Validate member data
                if (string.IsNullOrWhiteSpace(member.FirstName) || string.IsNullOrWhiteSpace(member.LastName) ||
                    string.IsNullOrWhiteSpace(member.Email) || string.IsNullOrWhiteSpace(member.PhoneNumber))
                {
                    MessageBox.Show("All fields must be filled out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var Query = @"INSERT INTO Members (FirstName, LastName, Email, PhoneNumber, JoinDate)
                      VALUES (@FName, @LName, @Email, @PhoneNum, @JoinDate);
                      SELECT SCOPE_IDENTITY();";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                {
                    command.Parameters.AddWithValue("@FName", member.FirstName);
                    command.Parameters.AddWithValue("@LName", member.LastName);
                    command.Parameters.AddWithValue("@Email", member.Email);
                    command.Parameters.AddWithValue("@PhoneNum", member.PhoneNumber);
                    command.Parameters.AddWithValue("@JoinDate", member.JoinDate);

                    conn.Open();
                    int id = Convert.ToInt32(command.ExecuteScalar());
                    member.MemberID = id;

                    MessageBox.Show("Member Is Added.", "Added!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    source.ResetBindings(false);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditMember(MemberModel member)
        {
            try
            {
                // Validate member data
                if (member.MemberID <= 0 || string.IsNullOrWhiteSpace(member.FirstName) || string.IsNullOrWhiteSpace(member.LastName) ||
                    string.IsNullOrWhiteSpace(member.Email) || string.IsNullOrWhiteSpace(member.PhoneNumber))
                {
                    MessageBox.Show("All fields must be filled out, and MemberID must be valid.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var Query = @"UPDATE Members SET FirstName = @FName, LastName = @LName, Email = @Email, PhoneNumber = @PhoneNum, JoinDate = @JoinDate
                      WHERE MemberID = @Id;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                {
                    command.Parameters.AddWithValue("@Id", member.MemberID);
                    command.Parameters.AddWithValue("@FName", member.FirstName);
                    command.Parameters.AddWithValue("@LName", member.LastName);
                    command.Parameters.AddWithValue("@Email", member.Email);
                    command.Parameters.AddWithValue("@PhoneNum", member.PhoneNumber);
                    command.Parameters.AddWithValue("@JoinDate", member.JoinDate);

                    conn.Open();
                    int rows = command.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show($"The Member Is Updated! {rows} row(s) affected", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No rows were updated. Please check the MemberID.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    source.ResetBindings(false);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
