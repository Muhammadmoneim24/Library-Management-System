using LMSProj.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LMSProj
{
    public partial class Book_Manage : Form
    {
        string connectionString = "Server=DESKTOP-MN6ULTF\\SSEXP;Database=LMSProj;Trusted_Connection=True;";
        BindingSource source;

        public Book_Manage()
        {
            InitializeComponent();
            source = new BindingSource();
            this.Shown += Book_Manage_Shown;
        }

        private void Book_Manage_Shown(object? sender, EventArgs e)
        {
            button1.PerformClick();
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = LoadBooks();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Book_Services book = new Book_Services();
                book.DataUpdated += Book_DataUpdated;
                book.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening book service form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                BookModel model = dataGridView1.CurrentRow.DataBoundItem as BookModel;
                if (model == null)
                {
                    MessageBox.Show("No book selected. Please select a book to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Book_Services book = new Book_Services(model);
                book.DataUpdated += Book_DataUpdated;
                book.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening book service form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Confirm Delete!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    BookModel book = dataGridView1.CurrentRow.DataBoundItem as BookModel;
                    if (book == null)
                    {
                        MessageBox.Show("No book selected. Please select a book to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    DeleteBook(book);
                    source.DataSource = new BookModel();
                    button1.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Book_DataUpdated()
        {
            button1.PerformClick();
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = FilterByCategory();
        }

        
        private List<BookModel> LoadBooks()
        {

            var Query = "SELECT * FROM Books WHERE 1=1 ";
            DataTable table = new DataTable();
            List<BookModel> books = new List<BookModel>();

            // Input validation
            string searchText = BookSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                Query += " AND (Title Like @searchText OR Author Like @searchText OR ISBN Like @searchText)";
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        command.Parameters.Add(new SqlParameter("@searchText", $"%{searchText}%"));
                    }

                    dataAdapter.Fill(table);

                    foreach (DataRow row in table.Rows)
                    {
                        books.Add(new BookModel()
                        {
                            BookID = Convert.ToInt32(row["BookID"]),
                            Author = row["Author"].ToString(),
                            Title = row["Title"].ToString(),
                            ISBN = Convert.ToInt32(row["ISBN"]),
                            PublicationYear = row["PublicationYear"].ToString(),
                            CategoryID = Convert.ToInt32(row["CategoryID"]),
                            TotalCopies = Convert.ToInt32(row["TotalCopies"]),
                            AvailableCopies = Convert.ToInt32(row["AvailableCopies"]),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return books;
        }
        private DataTable FilterByCategory()
        {
            string query = @"SELECT c.CategoryName, b.Title AS BookTitle, SUM(b.AvailableCopies) AS AvailableCopies
                             FROM Books b
                             JOIN Categories c ON b.CategoryID = c.CategoryID";

            var categfilter = CategFilter.Text.Trim();
            if (!string.IsNullOrEmpty(categfilter))
            {
                query += " WHERE c.CategoryName LIKE @categfilter";
            }

            query += " GROUP BY c.CategoryName, b.Title";

            DataTable table = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    if (!string.IsNullOrEmpty(categfilter))
                    {
                        command.Parameters.Add(new SqlParameter("@categfilter", $"%{categfilter}%"));
                    }

                    dataAdapter.Fill(table);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering books by category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return table;
        }

        private void DeleteBook(BookModel book)
        {
            var Query = @"DELETE FROM Books WHERE BookID = @Id; ";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                {
                    command.Parameters.AddWithValue("@Id", book.BookID);

                    conn.Open();
                    int rows = command.ExecuteNonQuery();

                    MessageBox.Show($"Deleted {rows} record(s).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
