namespace LMSProj
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
        }

        private void Form1_Shown(object? sender, EventArgs e)
        {
            pictureBox1_Click(sender, e);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Book_Manage book = new Book_Manage();
            book.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Member_Manage member = new Member_Manage();
            member.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Borrowing_Manage borrowing = new Borrowing_Manage();
            borrowing.Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            report.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string imagePath = @"C:\Users\lap shop\Downloads\lms.jpg";
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Load(imagePath);
        }
    }
}
