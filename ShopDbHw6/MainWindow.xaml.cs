using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ShopDbHw6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection? connection = null;
        SqlDataAdapter? adapter = null;
        DataViewManager? dataView = null;
        DataSet? dataSet = null;
        int catId = 0;
        public MainWindow()
        {
            InitializeComponent();
            Configuration();
        }
        private void Configuration()
        {
            var conStr = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build()
                        .GetConnectionString("ShopDb");

            connection = new SqlConnection(conStr);
            adapter = new SqlDataAdapter("SELECT * FROM Product; SELECT * FROM Category; SELECT * FROM Rating", connection);
            dataSet = new DataSet();
            dataView = new DataViewManager(dataSet);

            adapter.TableMappings.Add("Table", "Product");
            adapter.TableMappings.Add("Table1", "Category");
            adapter.TableMappings.Add("Table2", "Rating");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (connection is not null && dataSet is not null)
            {
                adapter?.Fill(dataSet);
                ProductList.ItemsSource = dataSet.Tables["Product"]?.AsDataView();

                categoriescb.DataContext = dataSet.Tables["Category"];
                categoriescb.DisplayMemberPath = dataSet.Tables["Category"]?.Columns["Name"]?.ColumnName;
            }
        }

        private void categoriescb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoriescb.SelectedItem is DataRowView rowView)
            {
                var row = rowView.Row;

                var table = dataSet?.Tables["Product"];
                if (table != null && dataView != null)
                {
                    var view = dataView.CreateDataView(table);

                    view.RowFilter = $"CategoryId = {row["Id"]}";

                    catId = (int)row["Id"];
                    ProductList.ItemsSource = view;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTxt.Text))
            {
                ProductList.ItemsSource = dataSet?.Tables["Product"]?.AsDataView();
                return;
            }

            var view = dataView?.CreateDataView(dataSet?.Tables?["Product"]);

            if (catId == 0)
                view.RowFilter = $"Name LIKE '%{SearchTxt.Text}%'";
            else view.RowFilter = $"Name LIKE '%{SearchTxt.Text}%' AND CategoryId={catId}";

            ProductList.ItemsSource = view;
        }

        private void ProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var row = (ProductList.Items[ProductList.SelectedIndex] as DataRowView)?.Row;
            var id = Convert.ToInt32(row?["Id"]);
            var name = row?["Name"].ToString();
            var price = Convert.ToDecimal(row?["Price"]);
            var quantity = Convert.ToInt32(row?["Quantity"]);
            var categoryId = Convert.ToInt32(row?["CategoryId"]);

            ShowDetail show = new(connection, dataSet?.Tables["Category"], id, name, quantity, price, categoryId);

            show.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddWindow add = new(connection, dataSet?.Tables["Category"]);
            add.ShowDialog();
            dataSet?.Clear();
            if (dataSet is not null)
            {
                adapter?.Fill(dataSet);
                ProductList.ItemsSource = dataSet?.Tables["Product"]?.AsDataView();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Closed)
            {
                connection?.Open();

            }
            if (ProductList.SelectedIndex >= 0)
            {

                var row = (ProductList.Items[ProductList.SelectedIndex] as DataRowView)?.Row;
                var id = Convert.ToInt32(row?["Id"]);
                var command = connection?.CreateCommand();
                command.CommandText = "DELETE Product WHERE Id=@id";
                command.Parameters.Add("id", SqlDbType.NVarChar);
                command.Parameters["id"].Value = id;
                command.ExecuteNonQuery();
                dataSet?.Clear();
                if (dataSet is not null)
                {
                    adapter?.Fill(dataSet);
                    ProductList.ItemsSource = dataSet?.Tables["Product"]?.AsDataView();
                }
                connection.Close();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Closed)
            {
                connection?.Open();

            }
            if (ProductList.SelectedIndex>=0)
            {
                var row = (ProductList.Items[ProductList.SelectedIndex] as DataRowView)?.Row;
                var id = Convert.ToInt32(row?["Id"]);
                var name = row?["Name"].ToString();
                var price = Convert.ToDecimal(row?["Price"]);
                var quantity = Convert.ToInt32(row?["Quantity"]);
                var categoryId = Convert.ToInt32(row?["CategoryId"]);
                EditWindow edit = new(connection, dataSet?.Tables["Category"], name, quantity, price, categoryId,id);
                edit.ShowDialog();
                dataSet?.Clear();
                if (dataSet is not null)
                {
                    adapter?.Fill(dataSet);
                    ProductList.ItemsSource = dataSet?.Tables["Product"]?.AsDataView();
                }
                connection.Close();
            }
        }
    }
}

