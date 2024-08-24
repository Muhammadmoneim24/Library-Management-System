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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LMSProj
{
    public partial class Borrowing_Manage : Form
    {
        string connectionString = "Server=DESKTOP-MN6ULTF\\SSEXP;Database=LMSProj;Trusted_Connection=True;";
        BindingSource source;

        public Borrowing_Manage()
        {
            InitializeComponent();
            source = new BindingSource();
            this.Shown += Borrowing_Manage_Shown;
        }

        private void Borrowing_Manage_Shown(object? sender, EventArgs e)
        {
            try
            {
                button1.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = LoadBooksBorrowings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading borrowings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Borrow_Service service = new Borrow_Service();
                service.DataUpdated += Service_DataUpdated;
                service.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the borrow service form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Service_DataUpdated()
        {
            try
            {
                button1.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                BorrowModel model = dataGridView1.CurrentRow.DataBoundItem as BorrowModel;
                if (model != null)
                {
                    Borrow_Service service = new Borrow_Service(model);
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

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Confirm Delete!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    BorrowModel member = dataGridView1.CurrentRow.DataBoundItem as BorrowModel;
                    if (member != null)
                    {
                        DeleteBorrow(member);
                        source.DataSource = new BorrowModel();
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

        private List<BorrowModel> LoadBooksBorrowings()
        {
            var Query = @"SELECT b1.BorrowID, b1.BookID, b1.MemberID, b1.BorrowDate, b1.DueDate, b1.ReturnDate, 
                         b2.AvailableCopies 
                          FROM Borrowings b1 
                          JOIN Books b2 ON b1.BookID = b2.BookID 
                          WHERE 1=1 ";

            DataTable table = new DataTable();
            List<BorrowModel> borrows = new List<BorrowModel>();

            try
            {
                string search = textBox1.Text.Trim();

                if (!string.IsNullOrEmpty(search))
                {
                    Query += " AND(MemberID = @search )";
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    command.Parameters.Add(new SqlParameter("@search", search));

                    dataAdapter.Fill(table);

                    foreach (DataRow row in table.Rows)
                    {
                        borrows.Add(new BorrowModel()
                        {
                            BorrowID = Convert.ToInt32(row["BorrowID"]),
                            BookID = Convert.ToInt32(row["BookID"]),
                            MemberID = Convert.ToInt32(row["MemberID"]),
                            BorrowDate = row["BorrowDate"].ToString(),
                            DueDate = row["DueDate"].ToString(),
                            ReturnDate = row["ReturnDate"].ToString(),
                            AvailableCopies = Convert.ToInt32(row["AvailableCopies"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while retrieving borrowings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return borrows;
        }

        private void DeleteBorrow(BorrowModel member)
        {
            try
            {
                var Query = @"DELETE FROM Borrowings WHERE BorrowID = @Id;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                {
                    command.Parameters.AddWithValue("@Id", member.BorrowID);

                    conn.Open();
                    int rows = command.ExecuteNonQuery();

                    MessageBox.Show($"Deleted! {rows} row(s) affected", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the borrowing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
