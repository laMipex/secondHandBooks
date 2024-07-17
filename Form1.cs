using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.Common;
using System.Security.Policy;
using System.Diagnostics;
using iTextSharp.text.pdf;
using iTextSharp.text;
using IronBarCode;
using System.IO;
using iTextSharp.text.pdf.qrcode;


namespace secondHandBooks
{
    public partial class Form1 : Form
    {
        string connectionString = "Data Source=DESKTOP-2AAQ8OB;Initial Catalog=secondHandShop;Integrated Security=True";
        bool search = false;
        bool cart = false;

        public Form1()
        {
            InitializeComponent();
            LoadData();
            FillDataGridView();

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Brooklyn", 13);
            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dataGridView1.DefaultCellStyle.ForeColor = Color.White;
            dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Brooklyn", 11);

            dataGridView1.Columns["Title"].Width = 200;
            dataGridView1.Columns["Genre"].Width = 150;
            dataGridView1.Columns["Author"].Width = 150;
            dataGridView1.Columns["Price"].Width = 75;
            dataGridView1.Columns["Publisher"].Width = 150;
            dataGridView1.Columns["Seller"].Width = 132;

            dataGridView2.Columns.Add("Title", "Title");
            dataGridView2.Columns.Add("Genre", "Genre");
            dataGridView2.Columns.Add("Price", "Price");
            dataGridView2.Columns.Add("Seller", "Seller");

            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Brooklyn", 13);
            dataGridView2.EnableHeadersVisualStyles = false;

            dataGridView2.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dataGridView2.DefaultCellStyle.ForeColor = Color.White;
            dataGridView2.DefaultCellStyle.Font = new System.Drawing.Font("Brooklyn", 11);

            dataGridView2.Columns["Title"].Width = 200;
            dataGridView2.Columns["Genre"].Width = 150;
            dataGridView2.Columns["Price"].Width = 75;
            dataGridView2.Columns["Seller"].Width = 132;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            panelHome.Visible = true;
            panelSearch.Visible = false;
            panelInv.Visible = false;
            panelCart.Visible = false;
            panelProfile.Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            panelHome.Visible = false;
            panelSearch.Visible = true;
            panelInv.Visible = false;
            panelCart.Visible = false;
            panelProfile.Visible = false;
        }

        private void btnInv_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            panelHome.Visible = false;
            panelSearch.Visible = false;
            panelInv.Visible = true;
            panelCart.Visible = false;
            panelProfile.Visible = false;
        }

        private void btnCart_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            panelHome.Visible = false;
            panelSearch.Visible = false;
            panelInv.Visible = false;
            panelCart.Visible = true;
            panelProfile.Visible = false;
        }

        private void btnPayTab_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
            panelHome.Visible = false;
            panelSearch.Visible = false;
            panelInv.Visible = false;
            panelCart.Visible = false;
            panelProfile.Visible = true;
        }

        private void btnSBoxClear_Click(object sender, EventArgs e)
        {
            txtBoxSearchInv.Clear();
            rbAFirstName.Checked = false;
            rbALastName.Checked = false;
            rbPubCountry.Checked = false;
            rbBookTitle.Checked = false;
            rbGenre.Checked = false;
            rbPublisher.Checked = false;
            rbPages.Checked = false;
            rbSeller.Checked = false;
            rbSellerCountry.Checked = false;
        }

        private void txtBoxSearchInv_TextChanged(object sender, EventArgs e)
        {
            btnSBoxClear.Visible = true;
            if (txtBoxSearchInv.Text == "")
            {
                btnSBoxClear.Visible = false;
            }
        }

        private void txtBoxSearchInv_Enter(object sender, EventArgs e)
        {
            if (txtBoxSearchInv.Text == "Search...")
            {
                txtBoxSearchInv.Text = "";
                btnSBoxClear.Visible = false;
            }
        }

        private void txtBoxSearchInv_Leave(object sender, EventArgs e)
        {
            if (txtBoxSearchInv.Text == "")
            {
                txtBoxSearchInv.Text = "Search...";
                btnSBoxClear.Visible = false;
            }
        }

        string queryMostReviews = @"
            SELECT TOP 1 s.s_username AS mostRUser, s.sales AS mostRSales, r.grade AS mostRGrade
            FROM rating r
            JOIN seller s ON r.s_username = s.s_username
            ORDER BY r.reviews DESC;";

        string queryBestRating = @"
            SELECT TOP 1 s.s_username AS mostRaUser, s.sales AS mostRaSales, r.grade AS mostRaGrade
            FROM rating r
            JOIN seller s ON r.s_username = s.s_username
            ORDER BY r.grade DESC;";

        string queryMostSales = @"
            SELECT TOP 1 s.s_username AS mostSUser, s.sales AS mostSSales, r.grade AS mostSGrade
            FROM seller s
            JOIN rating r ON s.s_username = r.s_username
            ORDER BY s.sales DESC;";

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand cmdMostReviews = new SqlCommand(queryMostReviews, connection))
                    {
                        SqlDataReader reader = cmdMostReviews.ExecuteReader();
                        if (reader.Read())
                        {
                            mostRUser.Text = reader["mostRUser"].ToString();
                            mostRSales.Text = reader["mostRSales"].ToString();
                            mostRGrade.Text = reader["mostRGrade"].ToString();
                        }
                        reader.Close();
                    }

                    using (SqlCommand cmdBestRating = new SqlCommand(queryBestRating, connection))
                    {
                        SqlDataReader reader = cmdBestRating.ExecuteReader();
                        if (reader.Read())
                        {
                            mostRaUser.Text = reader["mostRaUser"].ToString();
                            mostRaSales.Text = reader["mostRaSales"].ToString();
                            mostRaGrade.Text = reader["mostRaGrade"].ToString();
                        }
                        reader.Close();
                    }

                    using (SqlCommand cmdMostSales = new SqlCommand(queryMostSales, connection))
                    {
                        SqlDataReader reader = cmdMostSales.ExecuteReader();
                        if (reader.Read())
                        {
                            mostSUser.Text = reader["mostSUser"].ToString();
                            mostSSales.Text = reader["mostSSales"].ToString();
                            mostSGrade.Text = reader["mostSGrade"].ToString();
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void FillDataGridView()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT t.title_name AS Title, t.type AS Genre, " +
                               "a.au_fname + ' ' + a.au_lname AS Author, " +
                               "p.pub_name AS Publisher, t.prices AS Price, " +
                               "s.s_username AS Seller " +
                               "FROM titles t " +
                               "INNER JOIN title_authors ta ON t.title_id = ta.title_id " +
                               "INNER JOIN authors a ON ta.au_id = a.au_id " +
                               "INNER JOIN publishers p ON ta.pub_id = p.pub_id " +
                               "INNER JOIN seller s ON ta.s_username = s.s_username";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                if (dataGridView1 != null && dataGridView1.Columns != null)
                {
                    if (dataGridView1.Columns.Contains("title_id"))
                    {
                        dataGridView1.Columns["title_id"].Visible = false;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("The dataGridView1 or its Columns collection is null.");
                }


                dataGridView1.Columns["Title"].HeaderText = "Title";
                dataGridView1.Columns["Genre"].HeaderText = "Genre";
                dataGridView1.Columns["Author"].HeaderText = "Author";
                dataGridView1.Columns["Publisher"].HeaderText = "Publisher";
                dataGridView1.Columns["Price"].HeaderText = "Price";
                dataGridView1.Columns["Seller"].HeaderText = "Seller";
            }

        }

        private void btnShowR_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            panelHome.Visible = false;
            panelSearch.Visible = false;
            panelInv.Visible = true;
            panelCart.Visible = false;
            panelProfile.Visible = false;

            string query = @"SELECT t.title_name AS Title, t.type AS Genre, 
               CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, t.prices AS Price, s.s_username AS Seller 
                    FROM rating r
                    JOIN seller s ON r.s_username = s.s_username
                    JOIN title_authors ta ON ta.s_username = s.s_username
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    WHERE s.s_username = (SELECT TOP 1 r.s_username FROM rating r GROUP BY r.s_username ORDER BY COUNT(r.reviews) DESC)";

            ShowData(query);

            search = true;
            btnClearData.Visible = true;
        }




        private void btnShowRa_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            panelHome.Visible = false;
            panelSearch.Visible = false;
            panelInv.Visible = true;
            panelCart.Visible = false;
            panelProfile.Visible = false;

            string query = @"SELECT t.title_name AS Title, t.type AS Genre, 
               CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, t.prices AS Price, s.s_username AS Seller 
                    FROM rating r
                    JOIN seller s ON r.s_username = s.s_username
                    JOIN title_authors ta ON ta.s_username = s.s_username
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    WHERE s.s_username = (SELECT TOP 1 r.s_username FROM rating r GROUP BY r.s_username ORDER BY AVG(r.grade) DESC)";

            ShowData(query);

            search = true;
            btnClearData.Visible = true;
        }



        private void btnShowS_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            panelHome.Visible = false;
            panelSearch.Visible = false;
            panelInv.Visible = true;
            panelCart.Visible = false;
            panelProfile.Visible = false;

            string query = @"SELECT t.title_name AS Title, t.type AS Genre, 
               CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, t.prices AS Price, s.s_username AS Seller 
                    FROM seller s
                    JOIN title_authors ta ON ta.s_username = s.s_username
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    WHERE s.s_username = (SELECT TOP 1 s.s_username FROM seller s GROUP BY s.s_username ORDER BY SUM(s.sales) DESC)";

            ShowData(query);

            search = true;
            btnClearData.Visible = true;
        }



        private void ShowData(string query)
        {
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connectionString);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;

                if (dataTable.Columns.Contains("Title"))
                    dataGridView1.Columns["Title"].HeaderText = "Title";

                if (dataTable.Columns.Contains("Genre"))
                    dataGridView1.Columns["Genre"].HeaderText = "Genre";

                if (dataTable.Columns.Contains("Author"))
                    dataGridView1.Columns["Author"].HeaderText = "Author";

                if (dataTable.Columns.Contains("Publisher"))
                    dataGridView1.Columns["Publisher"].HeaderText = "Publisher";

                if (dataTable.Columns.Contains("Price"))
                    dataGridView1.Columns["Price"].HeaderText = "Price";

                if (dataTable.Columns.Contains("Seller"))
                    dataGridView1.Columns["Seller"].HeaderText = "Seller";

                if (dataTable.Columns.Contains("title_id"))
                    dataGridView1.Columns["title_id"].Visible = false;

                if (dataTable.Columns.Contains("pub_id"))
                    dataGridView1.Columns["pub_id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2 && search)
            {
                btnClearData.Visible = true;
            }
            else
            {
                btnClearData.Visible = false;
            }
        }

        private void btnClearData_Click(object sender, EventArgs e)
        {
            btnClearData.Visible = false;
            search = false;
            FillDataGridView();
        }

        private void btnSBoxSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtBoxSearchInv.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a search term!");
                return;
            }

            string query = string.Empty;

            if (rbAFirstName.Checked)
            {
                query = @"SELECT t.title_name AS Title, t.type AS Genre, 
                   CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, 
                   t.prices AS Price, s.s_username AS Seller 
                    FROM title_authors ta
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    JOIN seller s ON ta.s_username = s.s_username
                    WHERE a.au_fname LIKE @searchTerm";
            }
            else if (rbALastName.Checked)
            {
                query = @"SELECT t.title_name AS Title, t.type AS Genre, 
                   CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, 
                   t.prices AS Price, s.s_username AS Seller 
                    FROM title_authors ta
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    JOIN seller s ON ta.s_username = s.s_username
                    WHERE a.au_lname LIKE @searchTerm";
            }
            else if (rbPubCountry.Checked)
            {
                query = @"SELECT t.title_name AS Title, t.type AS Genre, 
                   CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, 
                   t.prices AS Price, s.s_username AS Seller 
                    FROM title_authors ta
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    JOIN seller s ON ta.s_username = s.s_username
                    WHERE p.country LIKE @searchTerm";
            }
            else if (rbBookTitle.Checked)
            {
                query = @"SELECT t.title_name AS Title, t.type AS Genre, 
                   CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, 
                   t.prices AS Price, s.s_username AS Seller 
                    FROM title_authors ta
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    JOIN seller s ON ta.s_username = s.s_username
                    WHERE t.title_name LIKE @searchTerm";
            }
            else if (rbGenre.Checked)
            {
                query = @"SELECT t.title_name AS Title, t.type AS Genre, 
                   CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, 
                   t.prices AS Price, s.s_username AS Seller 
                    FROM title_authors ta
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    JOIN seller s ON ta.s_username = s.s_username
                    WHERE t.type LIKE @searchTerm";
            }
            else if (rbPublisher.Checked)
            {
                query = @"SELECT t.title_name AS Title, t.type AS Genre, 
                   CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, 
                   t.prices AS Price, s.s_username AS Seller 
                    FROM title_authors ta
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    JOIN seller s ON ta.s_username = s.s_username
                    WHERE p.pub_name LIKE @searchTerm";
            }
            else if (rbPages.Checked)
            {
                if (!int.TryParse(searchTerm, out int pages))
                {
                    MessageBox.Show("Please enter a valid number for pages.");
                    return;
                }

                query = @"SELECT t.title_name AS Title, t.type AS Genre, 
                       CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, 
                       t.prices AS Price, s.s_username AS Seller 
                        FROM title_authors ta
                        JOIN titles t ON t.title_id = ta.title_id
                        JOIN authors a ON a.au_id = ta.au_id
                        JOIN publishers p ON p.pub_id = ta.pub_id
                        JOIN seller s ON ta.s_username = s.s_username
                        WHERE t.pages = @searchTerm";
            }
            else if (rbSeller.Checked)
            {
                query = @"SELECT t.title_name AS Title, t.type AS Genre, 
                   CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, 
                   t.prices AS Price, s.s_username AS Seller 
                    FROM title_authors ta
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    JOIN seller s ON ta.s_username = s.s_username
                    WHERE s.s_username LIKE @searchTerm";
            }
            else if (rbSellerCountry.Checked)
            {
                query = @"SELECT t.title_name AS Title, t.type AS Genre, 
                   CONCAT(a.au_fname, ' ', a.au_lname) AS Author, p.pub_name AS Publisher, 
                   t.prices AS Price, s.s_username AS Seller 
                    FROM title_authors ta
                    JOIN titles t ON t.title_id = ta.title_id
                    JOIN authors a ON a.au_id = ta.au_id
                    JOIN publishers p ON p.pub_id = ta.pub_id
                    JOIN seller s ON ta.s_username = s.s_username
                    WHERE s.country LIKE @searchTerm";
            }
            else
            {
                MessageBox.Show("Please select what are you searching for!");
                return;
            }

            if (rbPages.Checked)
            {
                ShowData(query, searchTerm, true);
            }
            else
            {
                ShowData(query, searchTerm);
            }

            tabControl1.SelectedIndex = 2;
            search = true;
            btnClearData.Visible = true;

            panelHome.Visible = false;
            panelSearch.Visible = false;
            panelInv.Visible = true;
            panelCart.Visible = false;
            panelProfile.Visible = false;

            txtBoxSearchInv.Clear();
        }

        private void ShowData(string query, string searchTerm, bool isInteger = false)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    if (isInteger)
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchTerm", int.Parse(searchTerm));
                    }
                    else
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                    }

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No results found.");
                        return;
                    }

                    dataGridView1.DataSource = dataTable;

                    if (dataTable.Columns.Contains("Title"))
                        dataGridView1.Columns["Title"].HeaderText = "Title";

                    if (dataTable.Columns.Contains("Genre"))
                        dataGridView1.Columns["Genre"].HeaderText = "Genre";

                    if (dataTable.Columns.Contains("Author"))
                        dataGridView1.Columns["Author"].HeaderText = "Author";

                    if (dataTable.Columns.Contains("Publisher"))
                        dataGridView1.Columns["Publisher"].HeaderText = "Publisher";

                    if (dataTable.Columns.Contains("Price"))
                        dataGridView1.Columns["Price"].HeaderText = "Price";

                    if (dataTable.Columns.Contains("Seller"))
                        dataGridView1.Columns["Seller"].HeaderText = "Seller";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                string title = dataGridView1.Rows[e.RowIndex].Cells["Title"].Value.ToString();
                string genre = dataGridView1.Rows[e.RowIndex].Cells["Genre"].Value.ToString();
                decimal price = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells["Price"].Value.ToString());
                string seller = dataGridView1.Rows[e.RowIndex].Cells["Seller"].Value.ToString();

                dataGridView2.Rows.Add(title, genre, price, seller);

                if (dataGridView2.Rows.Count == 0)
                {
                    lblEmptyCart.Visible = true;
                    btnBuy.Visible = false;
                    dataGridView2.Visible = false;
                }
                else
                {
                    lblEmptyCart.Visible = false;
                    btnBuy.Visible = true;
                    dataGridView2.Visible = true;
                }

                decimal totalAmmount = 0;
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    totalAmmount += decimal.Parse(row.Cells["Price"].Value.ToString());
                }
                txtBoxAmmount.Text = totalAmmount.ToString();
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView2.Rows.Count)
            { 
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                dataGridView2.Rows.Remove(row);
            }

            if (dataGridView2.Rows.Count == 0)
            {
                lblEmptyCart.Visible = true;
                btnBuy.Visible = false;
                dataGridView2.Visible = false;
            }
            else
            {
                lblEmptyCart.Visible = false;
                btnBuy.Visible = true;
                dataGridView2.Visible = true;
            }
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            txtBoxDate.Text = DateTime.Now.ToString();
            txtBoxCurrency.Text = "Euro " + "|" + " €";
            txtBoxPurchaseFor.Text = "Book/Books";
            txtBoxRecipient.Text = "SecondHandBooks";
            txtBoxRecipientAcc.Text = "170-30024679000-11";
            tabControl1.SelectedIndex = 4;

            panelHome.Visible = false;
            panelSearch.Visible = false;
            panelInv.Visible = false;
            panelCart.Visible = false;
            panelProfile.Visible = true;

            btnPayTab.Enabled = true;
        }

        private void btnClearCheck_Click(object sender, EventArgs e)
        {
            txtBoxDate.Clear();
            txtBoxRecipient.Clear();
            txtBoxRecipientAcc.Clear();
            txtBoxFullName.Clear();
            txtBoxAmmount.Clear();

            btnPayTab.Enabled = false;
            tabControl1.SelectedIndex = 3;

            panelHome.Visible = false;
            panelSearch.Visible = false;
            panelInv.Visible = false;
            panelCart.Visible = true;
            panelProfile.Visible = false;
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            string filePath = "Receipt.pdf";

            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            iTextSharp.text.Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            Paragraph title = new Paragraph("Payment Receipt\n", boldFont);
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            Paragraph data = new Paragraph
            {
                $"Full Name: {txtBoxFullName.Text}\n",
                $"Purchase For: {txtBoxPurchaseFor.Text}\n",
                $"Recipient: {txtBoxRecipient.Text}\n",
                $"Amount: {txtBoxAmmount.Text} {txtBoxCurrency.Text}\n",
                $"Recipient Account: {txtBoxRecipientAcc.Text}\n",
                $"Date: {txtBoxDate.Text}\n"
            };
            data.Alignment = Element.ALIGN_CENTER;
            doc.Add(data);

            BarcodeQRCode barcode = new BarcodeQRCode($"Full Name: {txtBoxFullName.Text}, Purchase For: {txtBoxPurchaseFor.Text}, " +
                $"Recipient: {txtBoxRecipient.Text}, Amount: {txtBoxAmmount.Text} {txtBoxCurrency.Text}, Recipient Account: {txtBoxRecipientAcc.Text}, " +
                $"Date: {txtBoxDate.Text}", 200, 200, new Dictionary<EncodeHintType, object>());
            iTextSharp.text.Image qrCodeImage = barcode.GetImage();
            qrCodeImage.Alignment = Element.ALIGN_CENTER;
            doc.Add(qrCodeImage);

            doc.Close();
            writer.Close();

            MessageBox.Show("Receipt generated successfully! Please save it and give it to\n" +
                "your nearest bank or post office! Or you can pay using your own bank account!");

            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }

}

