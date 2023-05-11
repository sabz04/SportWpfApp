using SportWpfApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        private int _currentOrderId = 0;
        public OrderPage()
        {
            InitializeComponent();
            LoadOrderProducts();
            LoadPickupPoints();
            orderProductsListView.SelectionChanged += OrderProductsListView_SelectionChanged;
        }

        private void OrderProductsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (orderProductsListView.SelectedItem == null)
                return;
            var selectedProduct = orderProductsListView.SelectedItem as Product;
            using (var db = new SportDBEntities())
            {
                var prod = db.Product.FirstOrDefault(x => x.Id == selectedProduct.Id);
                if (prod == null)
                    return;
                var order = new Order();
                if(CurrentUser.CurrentOrder != null)
                    order = db.Order.Include(x => x.OrderProduct).FirstOrDefault(x=>x.Id == CurrentUser.CurrentOrder.Id);
                if(CurrentUser.UserCurrent != null)
                {
                    var currentOrder = CurrentUser.UserCurrent.Order.FirstOrDefault();
                    order = db.Order.Include(x => x.OrderProduct).FirstOrDefault(x => x.Id == currentOrder.Id);
                }
                   

                var result = MessageBox.Show("Удалить товар из заказа?", "Подтвердите удаление", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var orderProd = order.OrderProduct.FirstOrDefault(x=>x.ProductId ==  prod.Id);
                    db.OrderProduct.Remove(orderProd);
                        
                }
                else if (result == MessageBoxResult.No)
                {
                    
                }
                db.SaveChanges();
                LoadOrderProducts();
            }
            
        }

        private void LoadOrderProducts()
        {
            orderProductsListView.ItemsSource = null;
            orderProductsListView.Items.Clear();
            using (var db = new SportDBEntities())
            {
                int? totalDiscount = 0;
                int? totalPrice = 0;
                var orderProducts = new List<OrderProduct>();
                var products = new List<Product>();
                var order = new Order();
                if (CurrentUser.UserCurrent == null)
                {
                    order = db.Order.FirstOrDefault(x => x.Id == CurrentUser.CurrentOrder.Id);
                    orderProducts = db.OrderProduct.Include(x => x.Product).Where(x => x.OrderId == CurrentUser.CurrentOrder.Id).ToList();
                    products = orderProducts.Select(x => x.Product).ToList();
                }
                else
                {
                    order = db.Order.FirstOrDefault(x => x.UserId == CurrentUser.UserCurrent.Id);
                    orderProducts = db.OrderProduct.Include(x => x.Product).Where(x => x.OrderId == order.Id).ToList();
                    products = orderProducts.Select(x => x.Product).ToList();
                }
                _currentOrderId = order.Id;
                orderProductsListView.ItemsSource = products;
                orderCodeTextBlock.Text = "Код для получения: " + order.OrderGetCode.ToString();
                products.ForEach(x => {
                    var orderProd = orderProducts.FirstOrDefault(y => y.ProductId == x.Id);
                    for (int i = 0; i < orderProd.Count; i++)
                    {
                        totalDiscount += x.ActualDiscountAmount;
                    }

                });
                products.ForEach(x => {
                    var orderProd = orderProducts.FirstOrDefault(y => y.ProductId == x.Id);
                    for(int i = 0; i < orderProd.Count; i++)
                    {
                        totalPrice += x.Cost;
                    }
                } 
               
                );
                int? discountPrice = totalPrice * totalDiscount / 100;
                totalPriceTextBlock.Text = "Конечная цена(с учетом скидки): "+ (totalPrice - discountPrice);
                totalDiscountTextBlock.Text = "Конечная скидка: " + totalDiscount.ToString();
                if(products.Any(x=>x.StockQuanitity > 3))
                {
                    order.DelieveryDate= DateTime.Now.AddDays(3);
                }
                else
                {
                    order.DelieveryDate = DateTime.Now.AddDays(6);
                }
                db.SaveChanges();
                deliveryDateTextBlock.Text = "Приблизительная дата доставки: \n" + order.DelieveryDate.ToString();
            }
        }

        private void LoadPickupPoints()
        {
            using (var db = new SportDBEntities())
            {
                db.PickupPoint.ToList().ForEach(p => {
                    pickupPointsComboBox.Items.Add($"{p.Id}|{p.City} {p.Street} {p.HouseNumber}");
                    if(p.Id == CurrentUser.PickupPointId)
                    {
                        pickupPointsComboBox.SelectedItem = $"{p.Id}|{p.City} {p.Street} {p.HouseNumber}";
                    }
                });
                
            }
        }

        private void pickupPointsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pickupPointsComboBox.SelectedItem != null)
            {
                string id = pickupPointsComboBox.SelectedItem.ToString().Split('|')[0];
                if (id != null && id != string.Empty)
                {
                    try
                    {
                        CurrentUser.PickupPointId = int.Parse(id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
               
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MainFrame.Navigate(new ProductsPage());
        }

        private void getTicketPDFButton_Click(object sender, RoutedEventArgs e)
        {
           
            TicketWindow ticketWindow = new TicketWindow(_currentOrderId);
            ticketWindow.Show();
        }
    }
}
