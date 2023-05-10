using SportWpfApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SportWpfApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public int _counter = 0, s = 0;
        public static LoginWindow Instance { get; private set; }
        private System.Timers.Timer timer;
        public LoginWindow()
        {
            InitializeComponent();
            CurrentUser.UserCurrent = null;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += timer_Tick;
            timer.AutoReset = true;
            Instance = this;
            loginTextBox.Text = "loginDEpxl2018";
            passwordTextBox.Text = "P6h4Jq";
            //LoadImage();
        }

        private void LoadImage()
        {
            var path = @"C:\Users\Сабиров\Desktop\демо экзамен\Задание\09_1.5-2022_2\Вариант 2\Вариант 2\Сессия 1\Товар_import";
            var items = new List<String>();
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var images = Directory.GetFiles(@"C:\Users\Сабиров\Desktop\демо экзамен\Задание\09_1.5-2022_2\Вариант 2\Вариант 2\Сессия 1\Товар_import");
            using (var db = new SportDBEntities())
            {
                foreach (var item in db.Product)
                {
                    try
                    {
                        item.Image = File.ReadAllBytes(images.FirstOrDefault(p => p.Contains(item.Id)));
                    }
                    catch
                    {
                        item.Image = File.ReadAllBytes(images.FirstOrDefault(p=> p.Contains("picture")));
                    }
                }
                db.SaveChanges();
            }
        }


    

    private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            using(var db = new SportDBEntities())
            {
                var user = db.User
                    .Include("Role")
                    .Include("Order")
                    .FirstOrDefault(x => x.Login == loginTextBox.Text && x.Password == passwordTextBox.Text);
                if (user != null)
                {
                    CurrentUser.UserCurrent = user;
                    MessageBox.Show("Авторизация успешна");
                    this.Hide();
                    PickupPointWindow pickupPointWindow = new PickupPointWindow();
                    pickupPointWindow.Show();

                }
                else
                {
                    MessageBox.Show("Ошибка авторизации");
                    if (_counter > 1)
                    {
                        this.Focus();
                        AuthButton.IsEnabled = false;
                        GuestAuthButton.IsEnabled = false;
                        
                        timer.Start();

                        return;
                    }
                    if (_counter > 0)
                    {
                        CaptchaWindow captchaWin = new CaptchaWindow();
                        captchaWin.Show();
                        this.Hide();
                    }
                    _counter++;
                }
            }  
        }

        private void GuestAuthButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentUser.UserCurrent = null;
            PickupPointWindow pickupPointWindow = new PickupPointWindow();
            pickupPointWindow.Show();
            this.Hide();
        }

        void timer_Tick(object sender, ElapsedEventArgs e)
        {
            s += 1;
            if (s > 10)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    AuthButton.IsEnabled = true;
                    GuestAuthButton.IsEnabled = true;
                    _counter = 0;
                    timer.Stop();
                }));
            }
        }

    }
}
