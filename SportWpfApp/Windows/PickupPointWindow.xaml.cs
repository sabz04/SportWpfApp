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
    /// Логика взаимодействия для PickupPointWindow.xaml
    /// </summary>
    public partial class PickupPointWindow : Window
    {
        public PickupPointWindow()
        {
            InitializeComponent();
            
            LoadPickupPoints();
        }

        private void LoadPickupPoints()
        {
            using(var db = new SportDBEntities())
            {
                db.PickupPoint.ToList().ForEach(p => {
                    pickupPointsComboBox.Items.Add($"{p.Id}|{p.City} {p.Street} {p.HouseNumber}");
                });
                
            }
        }

        private void pickupPointsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(pickupPointsComboBox.SelectedItem != null )
            {
                string id = pickupPointsComboBox.SelectedItem.ToString().Split('|')[0];
                if(id != null && id != string.Empty) {
                    try
                    {
                        CurrentUser.PickupPointId = int.Parse(id);
                    }
                    catch(Exception ex) 
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                this.Close();
                MessageBox.Show($"Пункт выдачи выбран: {pickupPointsComboBox.SelectedItem.ToString()}");
                MainWindow main = new MainWindow();
                main.Show();
            }

            
        }
    }
}
