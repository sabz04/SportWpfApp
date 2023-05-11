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
    /// Логика взаимодействия для TicketWindow.xaml
    /// </summary>
    public partial class TicketWindow : Window
    {
        private int _currentOrderId;
        public TicketWindow(int orderId)
        {
            InitializeComponent();
            this._currentOrderId = orderId;
            LoadData();
        }

        private void LoadData()
        {
            using(var db = new SportDBEntities())
            {
                int? totalDiscount = 0;
                int? totalPrice = 0;

                var order = db.Order.Include("OrderProduct").FirstOrDefault(x=>x.Id == _currentOrderId);
                if (order == null)
                    return;
                orderDateTextBlock.Text = order.OrderDate.ToString();
                orderDeliveryTextBlock.Text = order.DelieveryDate.ToString();
                orderCodeTextBlock.Text = order.OrderGetCode.ToString();
                
                var products = db.OrderProduct.Where(x=>x.OrderId == _currentOrderId).Select(x=>x.Product).ToList();

                products.ForEach(x =>
                {
                    productsListBox.Items.Add(x.Name);
                });
                products.ForEach(x => {
                    var orderProd = order.OrderProduct.FirstOrDefault(y => y.ProductId == x.Id);
                    for (int i = 0; i < orderProd.Count; i++)
                    {
                        totalDiscount += x.ActualDiscountAmount;
                    }

                });
                products.ForEach(x =>
                {
                    var orderProd = order.OrderProduct.FirstOrDefault(y => y.ProductId == x.Id);
                    for (int i = 0; i < orderProd.Count; i++)
                    {
                        totalPrice += x.Cost;
                    }
                });
                var discountPrice = ((totalPrice * totalDiscount) / 100);
                totalPriceTextBlock.Text = (totalPrice - discountPrice).ToString();
                totalDiscountTextBlock.Text = totalDiscount.ToString();

                var pickupPoint = db.PickupPoint.FirstOrDefault(x => x.Id == CurrentUser.PickupPointId);
                if(pickupPoint != null)
                {
                    pickupPointTextBlock.Text = $"{pickupPoint.Id}|{pickupPoint.City} {pickupPoint.Street} {pickupPoint.HouseNumber} ";
                }
            }
        }
    }
}
