using SportWpfApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для ProductOrderAddWindow.xaml
    /// </summary>
    public partial class ProductOrderAddWindow : Window
    {
        Product _selectedProd;
        public ProductOrderAddWindow(Product product)
        {
            InitializeComponent();
            _selectedProd = product;
            prodNameTextBlock.Text = _selectedProd.Name;
        }

        private void ProductOrderAddButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new SportDBEntities())
            {
                if (CurrentUser.UserCurrent != null)
            {
                if(CurrentUser.UserCurrent.Order.Count < 1)
                {
                        var newOrder = new Order
                        {
                            OrderDate = DateTime.Now,
                            StatusId = 1,
                            OrderGetCode = GenCode(),
                            UserId = CurrentUser.UserCurrent.Id,
                            PickupPointId = CurrentUser.PickupPointId,
                        };
                        db.Order.Add(newOrder);
                        OrderProduct orderProduct = new OrderProduct
                        {
                            Count = 1,
                            OrderId = newOrder.Id,
                            ProductId = _selectedProd.Id
                        };
                        
                        db.OrderProduct.Add(orderProduct);
                        db.SaveChanges();

                        CurrentUser.UserCurrent = db.User.FirstOrDefault(x=>x.Id== CurrentUser.UserCurrent.Id);
                        MessageBox.Show($"Товар {_selectedProd.Name} был добавлен к заказу");
                    }
                else
                {
                        var currentOrder = CurrentUser.UserCurrent.Order.FirstOrDefault();
                        var ord = db.Order.FirstOrDefault(x => x.Id == currentOrder.Id);
                        if (ord != null)
                        {
                            int? totalCount = 0;
                            if (ord.OrderProduct.Any(x => x.Product.Id == _selectedProd.Id))
                            {
                                foreach (var item in ord.OrderProduct)
                                {
                                    if (item.Product.Id == _selectedProd.Id)
                                    {
                                        item.Count++;
                                        totalCount = item.Count;
                                        break;
                                    }
                                }
                                MessageBox.Show($"Товар {_selectedProd.Name} уже был добавлен к заказу, количество: {totalCount}");

                            }
                            else
                            {
                                db.OrderProduct.Add(new OrderProduct
                                {
                                    OrderId = ord.Id,
                                    ProductId = _selectedProd.Id,
                                    Count = 1
                                });

                                MessageBox.Show($"Товар {_selectedProd.Name} добавлен к заказу");
                            }
                            db.SaveChanges();
                        }
                    }
            }
            else
            {
                if(CurrentUser.CurrentOrder == null)
                {
                    
                    var ord = new Order
                    {
                        OrderDate = DateTime.Now,
                        StatusId = 1,
                        OrderGetCode = GenCode(),

                        PickupPointId = CurrentUser.PickupPointId,
                    };
                        OrderProduct orderProduct = new OrderProduct
                        {
                            Count = 1,
                            OrderId = ord.Id,
                            ProductId = _selectedProd.Id
                        };

                        db.Order.Add(ord);
                        db.OrderProduct.Add(orderProduct);
                        db.SaveChanges();
                        
                        MessageBox.Show($"Заказ успешно создан! Товар {_selectedProd.Name} добавлен к новому заказу!");
                        db.SaveChanges();
                        CurrentUser.CurrentOrder = ord;

                    }
                else
                {
                    var ord = db.Order.FirstOrDefault(x=>x.Id == CurrentUser.CurrentOrder.Id);
                    if (ord != null)
                    {
                            int? totalCount = 0;
                            if(ord.OrderProduct.Any(x=>x.Product.Id == _selectedProd.Id))
                            {
                                foreach (var item in ord.OrderProduct)
                                {
                                    if (item.Product.Id == _selectedProd.Id)
                                    {
                                        item.Count++;
                                        totalCount = item.Count;
                                        break;
                                    }
                                }
                                MessageBox.Show($"Товар {_selectedProd.Name} уже был добавлен к заказу, количество: {totalCount}");
                                
                            }
                            else
                            {
                                db.OrderProduct.Add(new OrderProduct
                                {
                                    OrderId = ord.Id,
                                    ProductId = _selectedProd.Id,
                                    Count = 1
                                });
                                
                                MessageBox.Show($"Товар {_selectedProd.Name} добавлен к заказу");
                            }
                            db.SaveChanges();
                        }
                }
            }
            }
            this.Close();
        }
        private int GenCode()
        {
            Random rnd = new Random();
            return rnd.Next(0, 999);
        }
    }
}
