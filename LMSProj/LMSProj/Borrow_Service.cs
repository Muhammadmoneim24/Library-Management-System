using LMSProj.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LMSProj
{
    public partial class Borrow_Service : Form
    {
        string connectionString = "Server=DESKTOP-MN6ULTF\\SSEXP;Database=LMSProj;Trusted_Connection=True;";
        BindingSource source;

        public event Action DataUpdated;

        public Borrow_Service(BorrowModel borrow) : this()
        {
            source.DataSource = borrow;
        }

        public Borrow_Service()
        {
            InitializeComponent();
            source = new BindingSource();
            BorrowModel borrow = new BorrowModel();
            source.DataSource = borrow;

            textBox1.DataBindings.Add("Text", source, "BorrowID");
            textBox2.DataBindings.Add("Text", source, "BookID");
            textBox3.DataBindings.Add("Text", source, "MemberID");
            textBox4.DataBindings.Add("Text", source, "BorrowDate");
            textBox5.DataBindings.Add("Text", source, "DueDate");
            textBox6.DataBindings.Add("Text", source, "ReturnDate");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            BorrowModel model = source.Current as BorrowModel;

            if (model != null)
            {
                if (model.BorrowID == 0)
                    AddBorrow(model);
                else
                    EditBorrow(model);

                DataUpdated?.Invoke();
            }
            else
            {
                MessageBox.Show("No data available to save.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddBorrow(BorrowModel model)
        {
            try
            {
                // Input validation
                if (model.BookID <= 0 || model.MemberID <= 0)
                {
                    MessageBox.Show("Invalid BookID or MemberID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Set default values
                model.BorrowDate = DateTime.Now.ToString("yyyy/MM/dd");
                model.DueDate = DateTime.Now.AddDays(15).ToString("yyyy/MM/dd");

                var Query = @"INSERT INTO Borrowings (BookID, MemberID, BorrowDate, DueDate, ReturnDate)
                              VALUES (@BookID, @MemberID, @BorrowDate, @DueDate, NULL);
                              SELECT SCOPE_IDENTITY();";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                {
                    command.Parameters.AddWithValue("@BookID", model.BookID);
                    command.Parameters.AddWithValue("@MemberID", model.MemberID);
                    command.Parameters.AddWithValue("@BorrowDate", model.BorrowDate);
                    command.Parameters.AddWithValue("@DueDate", model.DueDate);

                    conn.Open();
                    int id = Convert.ToInt32(command.ExecuteScalar());

                    UpdateBookAvailability(model.BookID);

                    MessageBox.Show("Book Borrowed Successfully", "Success", MessageBoxButtons.OK);
                    source.ResetBindings(false);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateBookAvailability(int bookID)
        {
            try
            {
                string query = "UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookID = @BookID AND AvailableCopies > 0";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@BookID", bookID);

                    conn.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("No available copies to borrow.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditBorrow(BorrowModel model)
        {
            try
            {
                // Input validation
                if (model.BorrowID <= 0 || model.BookID <= 0 || model.MemberID <= 0)
                {
                    MessageBox.Show("Invalid BorrowID, BookID, or MemberID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var Query = @"UPDATE Borrowings SET BookID = @BookID, MemberID = @MemberID, BorrowDate = @BorrowDate, DueDate = @DueDate, ReturnDate = @ReturnDate
                              WHERE BorrowID = @Id;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                {
                    command.Parameters.AddWithValue("@Id", model.BorrowID);
                    command.Parameters.AddWithValue("@BookID", model.BookID);
                    command.Parameters.AddWithValue("@MemberID", model.MemberID);
                    command.Parameters.AddWithValue("@BorrowDate", model.BorrowDate);
                    command.Parameters.AddWithValue("@DueDate", model.DueDate);
                    command.Parameters.AddWithValue("@ReturnDate", model.ReturnDate);

                    conn.Open();
                    int rows = command.ExecuteNonQuery();

                    MessageBox.Show($"The Borrowing Is Updated! {rows} row(s) affected");
                    source.ResetBindings(false);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
