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
    /// Логика взаимодействия для CaptchaWindow.xaml
    /// </summary>
    public partial class CaptchaWindow : Window
    {
        public CaptchaWindow()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        private void SendCaptchaButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaTextBox.Text.Equals(TrueCaptchaTextBox.Text))
            {
                this.Close();
                LoginWindow.Instance.Show();
                
                
            }
            else
            {
                MessageBox.Show("Неверная каптча!");
                GenerateCaptcha();
            }
        }

        private void GenCaptchaButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
        }
        public void GenerateCaptcha()
        {
            CaptchaTextBox.IsReadOnly = true;
            String allowchar = " ";
            String pwd = "";
            allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z";
            allowchar += "1,2,3,4,5,6,7,8,9,0";

            char[] a = { ',' };
            String[] ar = allowchar.Split(a);
            string temp = " ";
            Random r = new Random();
            for (int i = 0; i < 4; i++)
            {
                temp = ar[(r.Next(0, ar.Length))];
                pwd += temp;
            }
            CaptchaTextBox.Text = pwd;
        }

    }
}
