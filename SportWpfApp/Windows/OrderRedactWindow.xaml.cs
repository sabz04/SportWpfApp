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
    /// Логика взаимодействия для OrderRedactWindow.xaml
    /// </summary>
    public partial class OrderRedactWindow : Window
    {
        Order _currentOrder;
        private int _selectedStatus = 0;
        private int _selectedPickup = 0;

        public static OrderRedactWindow Current { get; set; }
        public OrderRedactWindow(Order order)
        {
            InitializeComponent();
            Current = this;

            _currentOrder = order;

            LoadPickupPoints();
            LoadStatus();

            if (_currentOrder != null)
            {
                orderIdTextBlock.Text = $"Заказ № {_currentOrder.Id}";
                if (_currentOrder.UserId != null)
                    orderUserTextBox.Text = $"{_currentOrder.User.Surname} {_currentOrder.User.Name} {_currentOrder.User.Patronymic}";
                orderDateTextBox.Text = _currentOrder.OrderDate.ToString();
                if (_currentOrder.DelieveryDate != null)
                    orderDeliveryTextBox.Text = _currentOrder.DelieveryDate.ToString();
                orderCodeTextBlock.Text = _currentOrder.OrderGetCode.ToString();
                orderStatusComboBox.SelectedIndex = (int)_currentOrder.StatusId - 1;
                pickupPointsComboBox.SelectedIndex = (int)_currentOrder.PickupPointId - 1;
            }
        }
        private void LoadPickupPoints()
        {
            using (var db = new SportDBEntities())
            {
                db.PickupPoint.ToList().ForEach(p => {
                    pickupPointsComboBox.Items.Add($"{p.Id}|{p.City} {p.Street} {p.HouseNumber}");
                });

            }
        }
        private void LoadStatus()
        {
            using (var db = new SportDBEntities())
            {
                db.OrderStatus.ToList().ForEach(p => {
                    orderStatusComboBox.Items.Add($"{p.StatusName}");
                });

            }
        }

        private void redactOrderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new SportDBEntities())
            {
                if (_selectedStatus == 0)
                {
                    return;
                }
                if (_selectedPickup == 0)
                {
                    return;
                }
                if (orderCodeTextBlock.Text == "")
                {
                    MessageBox.Show("Пожалуйста введите код для получения!");
                    return;
                }
                if (orderDeliveryTextBox.Text == "")
                {
                    MessageBox.Show("Пожалуйста введите дату получения товара!");
                    return;
                }

                if (_currentOrder != null)
                {
                    try
                    {
                        _currentOrder = db.Order.Find(_currentOrder.Id);

                        #region DeliveryDate
                        string date = orderDeliveryTextBox.Text;
                        string[] dates = date.Split('.');
                        int month = int.Parse(dates[1]);
                        int days = int.Parse(dates[0]);
                        string yearString = dates[2].Substring(0, 4);
                        int year = Int32.Parse(yearString);
                        DateTime dateTime = new DateTime(year, month, days);
                        #endregion

                        _currentOrder.DelieveryDate = dateTime;
                        _currentOrder.OrderGetCode = Int32.Parse(orderCodeTextBlock.Text);
                        _currentOrder.PickupPointId = _selectedPickup;
                        _currentOrder.StatusId = _selectedStatus;

                        db.SaveChanges();
                        MessageBox.Show($"Данные заказа № {_currentOrder.Id} были успешно изменены!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка добавления нового товара. \n{ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Текущий пользователь равен null.");
                    return;
                }
            }
        }

        private void orderStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (orderStatusComboBox.SelectedItem == null)
                return;
            var selectStatus = orderStatusComboBox.SelectedItem.ToString();
            using (var db = new SportDBEntities())
            {
                var status = db.OrderStatus.FirstOrDefault(x => x.StatusName.ToLower().Contains(selectStatus.ToLower()));
                if (status == null)
                    return;
                _selectedStatus = status.Id;
            }
        }

        private void pickupPointsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pickupPointsComboBox.SelectedItem == null)
                return;
            int id = Int32.Parse(pickupPointsComboBox.SelectedItem.ToString().Split('|')[0]);
            using (var db = new SportDBEntities())
            {
                var pickup = db.PickupPoint.FirstOrDefault(x => x.Id.Equals(id));
                if (pickup == null)
                    return;
                _selectedPickup = pickup.Id;
            }
        }
    }
}
