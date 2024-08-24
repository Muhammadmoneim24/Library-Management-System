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
    public partial class Report : Form
    {
        string connectionString = "Server=DESKTOP-MN6ULTF\\SSEXP;Database=LMSProj;Trusted_Connection=True;";
        BindingSource source;

        public Report()
        {
            InitializeComponent();
            source = new BindingSource();

            this.Shown += Report_Shown;
        }

        private void Report_Shown(object? sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = LoadMostBorrowings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the most borrowed books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = LoadMostBorrowings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the most borrowed books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = LoadAllBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading all books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = LoadAllMembers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading all members: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = LoadOverdueBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading overdue books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<BorrowModel> LoadMostBorrowings()
        {
            var Query = @"SELECT TOP 10 b1.BorrowID, b1.BookID, b1.MemberID, b1.BorrowDate, b1.DueDate, b1.ReturnDate, 
                          b2.AvailableCopies
                          FROM Borrowings b1
                          JOIN Books b2 ON b1.BookID = b2.BookID
                          WHERE b2.AvailableCopies > 0
                          ORDER BY b2.AvailableCopies ASC";

            DataTable table = new DataTable();
            List<BorrowModel> borrows = new List<BorrowModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
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
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"A SQL error occurred: {sqlEx.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading most borrowed books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return borrows;
        }

        private List<BookModel> LoadAllBooks()
        {
            var Query = @"SELECT *
                          FROM Books";

            DataTable table = new DataTable();
            List<BookModel> books = new List<BookModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(table);

                    foreach (DataRow row in table.Rows)
                    {
                        books.Add(new BookModel()
                        {
                            BookID = Convert.ToInt32(row["BookID"]),
                            Title = row["Title"].ToString(),
                            Author = row["Author"].ToString(),
                            ISBN = Convert.ToInt32(row["ISBN"]),
                            TotalCopies = Convert.ToInt32(row["TotalCopies"]),
                            AvailableCopies = Convert.ToInt32(row["AvailableCopies"])
                        });
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"A SQL error occurred: {sqlEx.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading all books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return books;
        }

        private List<MemberModel> LoadAllMembers()
        {
            var Query = @"SELECT *
                          FROM Members";

            DataTable table = new DataTable();
            List<MemberModel> members = new List<MemberModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
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
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"A SQL error occurred: {sqlEx.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading all members: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return members;
        }

        private List<BorrowModel> LoadOverdueBooks()
        {
            var Query = @"SELECT b1.BorrowID, b1.BookID, b1.MemberID, b1.BorrowDate, b1.DueDate, b1.ReturnDate, 
                          b2.AvailableCopies
                          FROM Borrowings b1
                          JOIN Books b2 ON b1.BookID = b2.BookID
                          WHERE b1.ReturnDate IS NULL AND b1.DueDate < GETDATE()";

            DataTable table = new DataTable();
            List<BorrowModel> borrows = new List<BorrowModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
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
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"A SQL error occurred: {sqlEx.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading overdue books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return borrows;
        }
    }
}
