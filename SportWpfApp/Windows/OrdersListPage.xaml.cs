using SportWpfApp.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SportWpfApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrdersListPage.xaml
    /// </summary>
    public partial class OrdersListPage : Page
    {
        Order _selectedOrder;

        public OrdersListPage()
        {
            InitializeComponent();

            using (var db = new SportDBEntities())
            {
                var orders = db.Order
                    .Include("OrderStatus")
                    .Include("User")
                    .ToList();

                OrdersDataGrid.ItemsSource = orders;
            }
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null)
                return;
            _selectedOrder = OrdersDataGrid.SelectedItem as Order;

            if (OrderRedactWindow.Current != null)
                OrderRedactWindow.Current.Close();
            OrderRedactWindow orderRedactWindow = new OrderRedactWindow(_selectedOrder);
            orderRedactWindow.Show();
        }
    }
}
