using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Common;

namespace ShopDbHw6
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        SqlConnection? connection = null;
        DataTable? categories = null;

        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        private int categoryId { get; set; }
        private int productId { get; set; }
        public EditWindow(SqlConnection? connection, DataTable? categories, string? productName, int quantity, decimal price, int categoryId, int productId)
        {
            InitializeComponent();
            DataContext = this;
            this.connection = connection;
            this.categories = categories;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
            this.categoryId = categoryId;
            this.productId = productId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            catid.DataContext = categories;
            catid.DisplayMemberPath = categories?.Columns["Name"]?.ColumnName;

            catid.SelectedIndex = categoryId - 1;
        }

        private void catid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (catid.SelectedItem is DataRowView rowView)
            {
                var row = rowView.Row;
                categoryId = Convert.ToInt32(row["Id"]);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }

            if (string.IsNullOrWhiteSpace(ProductName))
                builder.Append($"{nameof(ProductName)} Cannot Be Empty\n");

            if (Price <= 0)
                builder.Append($"{nameof(Price)} Cannot be below or equal to 0\n");

            if (Quantity < 0)
                builder.Append($"{nameof(Quantity)} Cannot be below 0\n");

            if (categoryId == -1)
                builder.Append($"{nameof(categoryId)} Must be choosen\n");

            if (builder.Length > 0)
            {
                MessageBox.Show(builder.ToString());
                return;
            }
            try
            {
                
                connection?.Open();

                var command = connection?.CreateCommand();
                var tran = connection?.BeginTransaction();

                command.Transaction = tran;

                command.CommandText = "UPDATE Product SET [Name] = @name, CategoryID = @categoryId, Quantity = @quantity, Price = @price WHERE Id = @productId";
                

                command.Parameters.Add("name", SqlDbType.NVarChar);
                command.Parameters["name"].Value = name.Text;

                command.Parameters.Add("categoryId", SqlDbType.Int);
                command.Parameters["categoryId"].Value = categoryId;

                command.Parameters.Add("quantity", SqlDbType.Int);
                command.Parameters["quantity"].Value = Quantity;

                command.Parameters.Add("price", SqlDbType.Decimal);
                command.Parameters["price"].Value = price.Text;

                command.Parameters.Add("productId", SqlDbType.Int);
                command.Parameters["productId"].Value = productId;

                command.ExecuteNonQuery();

                tran?.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection?.Close();
            }
            DialogResult = true;

        }
    }
}
