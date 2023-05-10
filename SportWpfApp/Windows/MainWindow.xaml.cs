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
       
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new ProductsPage());

          

            if(CurrentUser.UserCurrent != null)
            {
                if (CurrentUser.UserCurrent.Role.RoleName.ToLower().Contains("админ"))
                { 
                    ProductAddButton.Visibility =  Visibility.Visible;
                }

                    userCredentialsTextBox.Text = CurrentUser.GetCreds();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LoginWindow.Instance.Show();
        }

        private void ProductAddButton_Click(object sender, RoutedEventArgs e)
        {
            
                ProductAddWindow productAddWindow = new ProductAddWindow(); 
                productAddWindow.Show();
            
        }
    }
}
