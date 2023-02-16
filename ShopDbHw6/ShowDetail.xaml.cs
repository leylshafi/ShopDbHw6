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

namespace ShopDbHw6
{
    /// <summary>
    /// Interaction logic for ShowDetail.xaml
    /// </summary>
    public partial class ShowDetail : Window
    {
        SqlConnection? connection = null;
        DataTable? categories = null;
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        private decimal Rating { get; set; }
        public ShowDetail(SqlConnection? connection, DataTable? categories, int id, string productName, int quantity, decimal price, int categoryId)
        {
            InitializeComponent();
            DataContext = this;
            this.connection = connection;
            this.categories = categories;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
            CategoryId = categoryId;
            Id = id;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
