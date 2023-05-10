using Microsoft.Win32;
using SportWpfApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ProductAddWindow.xaml
    /// </summary>
    public partial class ProductAddWindow : Window
    {

        private Product _currentProduct;
        private int _selectedCategory=0;
        private byte[] _selectedImageBinary;
        private int _selectedManufacturer = 0;

        public ProductAddWindow(Product currentProd = null)
        {
            InitializeComponent();
            if(currentProd != null)
            {
                
                _currentProduct = currentProd;
            }
            else
            {
                idTextBox.IsEnabled = true;
            }
            LoadCbData();
        }

        private void LoadCbData()
        {
            categoryComboBox.Items.Clear();
            using (var db = new SportDBEntities())
            {
                var catsList = db.ProductCategory.ToList();
                catsList.ForEach(x => categoryComboBox.Items.Add(x.CategoryName));
                categoryComboBox.SelectedIndex= 0;

                var manufactsList = db.Manufacturer.ToList();
                manufactsList.ForEach(x => manufacturerComboBox.Items.Add(x.ManufacturerName));
                manufacturerComboBox.SelectedIndex= 0;
            }
        }

        public ProductAddWindow()
        {
            InitializeComponent();
            idTextBox.IsEnabled= true;
            LoadCbData();
        }
        private void productImageImageView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoadImage();
        }

        private void saveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if(_selectedCategory == 0)
            {
                return;
            }
            if (_selectedImageBinary == null || _selectedManufacturer == 0)
                return;
            if(_currentProduct == null)
            {

                string uniqueId = idTextBox.Text;
                if(uniqueId.Length < 1)
                {
                    return;
                }
                try
                {
                    using (var db = new SportDBEntities())
                    {
                        if(db.Product.Any(x=>x.Id.ToLower().Equals(uniqueId.ToLower())))
                        {
                            MessageBox.Show("Товар с таким идентификатором уже существует в системе.");
                            return;
                        }
                        var prod = new Product
                        {
                            Id = uniqueId,
                            ActualDiscountAmount = int.Parse(actualDiscountAmount.Text),
                            MaxDiscountAmount = int.Parse(maxDiscountAmount.Text),
                            Cost = int.Parse(priceTextBox.Text),
                            Description = descTextBox.Text,
                            Name = nameTextBox.Text,
                            ProductCategoryId = _selectedCategory,
                            ManufacturerId = _selectedManufacturer,
                            MeasurementUnit = measurementUnitTextBox.Text,
                            StockQuanitity = int.Parse(countTextBox.Text),
                            Image = _selectedImageBinary

                        };
                        db.Product.Add(prod);
                        db.SaveChanges();
                        MessageBox.Show("Сохранение успешно!");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления нового товара. \n{ex.Message}");
                }
            }
            
        }
        private void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения| *.jpg; *.jpeg";
            openFileDialog.Title = "Выберите изображение";
            if(openFileDialog.ShowDialog() == true)
            {
                try
                {
                    productImageImageView.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    _selectedImageBinary = File.ReadAllBytes(openFileDialog.FileName);
                }
                catch
                {
                    MessageBox.Show("Ошибка выбора изображения.");
                }
            }
            else
            {
                MessageBox.Show("Ошибка выбора изображения.");
            }
        }
        private void categoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryComboBox.SelectedItem == null)
                return;
            var category = categoryComboBox.SelectedItem.ToString();
            using(var db = new SportDBEntities())
            {
                var cat = db.ProductCategory.FirstOrDefault(x => x.CategoryName.ToLower().Contains(category.ToLower()));
                if (cat == null)
                    return;
                _selectedCategory = cat.Id;
            }

        }

        private void manufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (manufacturerComboBox.SelectedItem == null)
                return;
            var manufacturer = manufacturerComboBox.SelectedItem.ToString();
            using (var db = new SportDBEntities())
            {
                var manufacture = db.Manufacturer.FirstOrDefault(x => x.ManufacturerName.ToLower().Contains(manufacturer.ToLower()));
                if (manufacture == null)
                    return;
                _selectedManufacturer = manufacture.Id;
            }
        }
    }
}
