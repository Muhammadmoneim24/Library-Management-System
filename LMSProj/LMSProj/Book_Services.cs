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
    public partial class Book_Services : Form
    {
        string connectionString = "Server=DESKTOP-MN6ULTF\\SSEXP;Database=LMSProj;Trusted_Connection=True;";
        BindingSource source;

        public event Action DataUpdated;

        public Book_Services(BookModel book) : this()
        {
            source.DataSource = book;
        }

        public Book_Services()
        {
            InitializeComponent();
            source = new BindingSource();
            BookModel book = new BookModel();
            source.DataSource = book;

            textBox1.DataBindings.Add("Text", source, "Title");
            textBox2.DataBindings.Add("Text", source, "Author");
            textBox3.DataBindings.Add("Text", source, "ISBN");
            textBox4.DataBindings.Add("Text", source, "PublicationYear");
            textBox5.DataBindings.Add("Text", source, "CategoryID");
            textBox6.DataBindings.Add("Text", source, "TotalCopies");
            textBox7.DataBindings.Add("Text", source, "AvailableCopies");
            textBox8.DataBindings.Add("Text", source, "BookID");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                BookModel bookModel = source.Current as BookModel;
                try
                {
                    if (bookModel.BookID == 0)
                        AddBook(bookModel);
                    else
                        EditBook(bookModel);

                    DataUpdated?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddBook(BookModel book)
        {
            try
            {
                var Query = @"INSERT INTO Books (Title, ISBN, Author, PublicationYear, CategoryID, TotalCopies, AvailableCopies)
                              VALUES (@Title, @ISBN, @Author, @PublicationYear, @CategoryID, @TotalCopies, @AvailableCopies);
                              SELECT SCOPE_IDENTITY();";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                {
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@ISBN", book.ISBN);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
                    command.Parameters.AddWithValue("@CategoryID", book.CategoryID);
                    command.Parameters.AddWithValue("@TotalCopies", book.TotalCopies);
                    command.Parameters.AddWithValue("@AvailableCopies", book.AvailableCopies);

                    conn.Open();
                    int id = Convert.ToInt32(command.ExecuteScalar());

                    MessageBox.Show("Book Added Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void EditBook(BookModel book)
        {
            try
            {
                var Query = @"UPDATE Books SET Title = @Title, ISBN = @ISBN, Author = @Author, 
                              PublicationYear = @PublicationYear, CategoryID = @CategoryID, 
                              TotalCopies = @TotalCopies, AvailableCopies = @AvailableCopies
                              WHERE BookID = @Id;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(Query, conn))
                {
                    command.Parameters.AddWithValue("@Id", book.BookID);
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@ISBN", book.ISBN);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
                    command.Parameters.AddWithValue("@CategoryID", book.CategoryID);
                    command.Parameters.AddWithValue("@TotalCopies", book.TotalCopies);
                    command.Parameters.AddWithValue("@AvailableCopies", book.AvailableCopies);

                    conn.Open();
                    int rows = command.ExecuteNonQuery();

                    MessageBox.Show($"{rows} row(s) updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private bool ValidateInput()
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(textBox1.Text))
                errors.AppendLine("Title is required.");
            if (string.IsNullOrEmpty(textBox2.Text))
                errors.AppendLine("Author is required.");
            if (string.IsNullOrEmpty(textBox3.Text) )
                errors.AppendLine("ISBN must Entered.");
            if (!int.TryParse(textBox4.Text, out _))
                errors.AppendLine("Publication Year must be a valid number.");
            if (string.IsNullOrEmpty(textBox5.Text))
                errors.AppendLine("Category ID is required.");
            if (!int.TryParse(textBox6.Text, out int totalCopies) || totalCopies < 0)
                errors.AppendLine("Total Copies must be a non-negative number.");
            if (!int.TryParse(textBox7.Text, out int availableCopies) || availableCopies < 0 || availableCopies > totalCopies)
                errors.AppendLine("Available Copies must be between 0 and Total Copies.");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}
