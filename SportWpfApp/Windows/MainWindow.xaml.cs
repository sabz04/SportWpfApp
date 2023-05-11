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
using System.Windows.Shapes;

namespace SportWpfApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; }
       
        public MainWindow()
        {
            InitializeComponent();
            Instance= this;
            MainFrame.Navigate(new ProductsPage());
            orderButton.Click += OrderButton_Click;

            CheckUserOrders();
            if (CurrentUser.UserCurrent != null)
            {
                if (CurrentUser.UserCurrent.Role.RoleName.ToLower().Contains("админ"))
                { 
                    ProductAddButton.Visibility =  Visibility.Visible;
                }
                userCredentialsTextBox.Text = CurrentUser.GetCreds();
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderPage());    
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LoginWindow.Instance.Show();
        }

        private void ProductAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductAddWindow.Current != null)
                ProductAddWindow.Current.Close();
            ProductAddWindow productAddWindow = new ProductAddWindow(); 
            productAddWindow.Show();
            
        }
        public void CheckUserOrders()
        {
            using (var db = new SportDBEntities()) {
                if(CurrentUser.UserCurrent != null)
                {
                    var user = db.User.Include("Order").FirstOrDefault(x => x.Id == CurrentUser.UserCurrent.Id);
                    if(user.Order.FirstOrDefault().OrderProduct.Count> 0)
                    {
                        orderButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        orderButton.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if(CurrentUser.CurrentOrder != null)
                    {
                        var orderProds = db.OrderProduct.Where(x => x.OrderId == CurrentUser.CurrentOrder.Id);
                        if(orderProds.Count() > 0)
                        {
                            orderButton.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }
    }
}
