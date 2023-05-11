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
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        String[] orderByPrice = { "По возрастанию", "По убыванию" };
        Product _selectedProd;
        String[] orderByDiscount = { "Все диапазоны", "0-9,99%", "10-14,99%", "15% и более" };

        bool isClickedLeft = false;
        public static ProductsPage Current { get; set; }

        FilterObject filterObject;
        public ProductsPage()
        {
            InitializeComponent();
            Current = this;
            filterObject = new FilterObject();
            priceComboBox.ItemsSource = orderByPrice;
            priceComboBox.SelectedIndex = 0;
            discountComboBox.ItemsSource = orderByDiscount;
            discountComboBox.SelectedIndex = 0;
        }

        public void LoadData()
        {
            using (var db = new SportDBEntities())
            {
                var products = db.Product.AsQueryable();
                var prods = db.Product.AsQueryable();
                if(filterObject.PriceOrderBy != null && filterObject.PriceOrderBy != String.Empty)
                {
                    if(filterObject.PriceOrderBy.Contains("По убыванию"))
                    {
                        products = products.OrderByDescending(x=>x.Cost);
                    }
                    else
                    {
                        products = products.OrderBy(x => x.Cost);
                    }
                }
                if (filterObject.Search != null && filterObject.Search != String.Empty)
                {
                    products = products
                        .Where(
                        x=>x.Description.ToLower().Contains(filterObject.Search.ToLower()) 
                        || x.Name.ToLower().Contains(filterObject.Search.ToLower()));
                }
                if (filterObject.Discount != null && filterObject.Discount != String.Empty)
                {
                    if (filterObject.Discount.Contains("Все диапазоны")) 
                    {
                        products = products.Where(x=> x.ActualDiscountAmount > 0);
                    }
                    if (filterObject.Discount.Contains("0-9,99%"))
                    {
                        products = products.Where(x => x.ActualDiscountAmount<10 && x.ActualDiscountAmount >0);
                    }
                    if(filterObject.Discount.Contains("10-14,99%"))
                    {
                        products = products.Where(x => x.ActualDiscountAmount < 15 && x.ActualDiscountAmount > 10);
                    }
                    if (filterObject.Discount.Contains("15% и более"))
                    {
                        products = products.Where(x => x.ActualDiscountAmount > 15);
                    }
                }

                var items = products
                    .Include(x=>x.Manufacturer)
                    .Include(x=>x.ProductCategory)
                    .ToList();
                ProductsListView.ItemsSource = null;
                ProductsListView.Items.Clear();

                CurrentItemCountTextBlock.Text = $"{items.Count} из {prods.Count()}";

                ProductsListView.ItemsSource = items;

            }
            
        }

        private void priceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(priceComboBox.SelectedItem != null)
                filterObject.PriceOrderBy = priceComboBox.SelectedItem.ToString();
            LoadData();
        }

        private void discountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (discountComboBox.SelectedItem != null)
                filterObject.Discount = discountComboBox.SelectedItem.ToString();
            LoadData();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchTextBox.Text != String.Empty)
            {
                filterObject.Search= searchTextBox.Text;
                LoadData();
            }
        }

        private void ProductsListView_SelectionChanged(object sender, EventArgs e)
        {
            if (ProductsListView.SelectedItem == null)
                return;
            _selectedProd = ProductsListView.SelectedItem as Product;
            
            if (isClickedLeft)
            {
                if (ProductAddWindow.Current != null)
                    ProductAddWindow.Current.Close();
                if (CurrentUser.UserCurrent != null && CurrentUser.UserCurrent.Role.RoleName.ToLower().Contains("админ"))
                {
                    var productAddWindow = new ProductAddWindow(_selectedProd);
                    productAddWindow.Show();
                }
            }
            if (!isClickedLeft)
            {
                ProductOrderAddWindow productOrderAddWindow = new ProductOrderAddWindow(_selectedProd);
                productOrderAddWindow.Show();


            }
            ProductsListView.SelectedItem = null;


        }

        private void ProductsListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                isClickedLeft= false;


                
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                isClickedLeft = true;
                
            }
            
        }
    }

    class FilterObject
    {
        public String PriceOrderBy { get; set; }
        public String Discount { get; set; }
        public String Search { get; set; }
    }
}
