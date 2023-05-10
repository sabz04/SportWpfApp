using Microsoft.Win32;
using SportWpfApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
using System.Xml;

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

        public static ProductAddWindow Current { get; set; }


        public ProductAddWindow(Product currentProd = null)
        {
            InitializeComponent();
            Current = this;

            if(currentProd != null)
            {
                _currentProduct = currentProd;
                deleteButton.Visibility = Visibility.Visible;
                idTextBox.Text = currentProd.Id;
                descTextBox.Text = currentProd.Description;
                nameTextBox.Text = currentProd.Name;
                measurementUnitTextBox.Text = currentProd.MeasurementUnit;
                priceTextBox.Text = currentProd.Cost.ToString();
                maxDiscountAmount.Text = currentProd.MaxDiscountAmount.ToString();
                actualDiscountAmount.Text = currentProd.ActualDiscountAmount.ToString();
                countTextBox.Text = currentProd.StockQuanitity.ToString();
                
                using (MemoryStream memoryStream = new MemoryStream(currentProd.Image))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();

                    BitmapFrame bitmapFrame = BitmapFrame.Create(bitmapImage);
                    productImageImageView.Source = bitmapFrame;

                }
                
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
                var manufactsList = db.Manufacturer.ToList();
                manufactsList.ForEach(x => manufacturerComboBox.Items.Add(x.ManufacturerName));

                if (_currentProduct != null)
                {
                    categoryComboBox.SelectedItem = _currentProduct.ProductCategory.CategoryName;
                    manufacturerComboBox.SelectedItem = _currentProduct.Manufacturer.ManufacturerName;
                }
                else
                {
                    categoryComboBox.SelectedIndex = 0;
                    manufacturerComboBox.SelectedIndex = 0;
                }   
            }
        }

        public ProductAddWindow()
        {

            InitializeComponent();
            Current = this;
            idTextBox.IsEnabled= true;
            idTextBox.MaxLength= 6;
            LoadCbData();
        }
        private void productImageImageView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoadImage();
        }

        private void saveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedCategory == 0)
                {
                    return;
                }
                if (_selectedManufacturer == 0)
                {
                    return;
                }
                
                string desc = descTextBox.Text;
                string name = nameTextBox.Text;
                string measurementUnit = measurementUnitTextBox.Text;

                int actuadlDiscount = int.Parse(actualDiscountAmount.Text);
                int maxDiscount = int.Parse(maxDiscountAmount.Text);
                int cost = int.Parse(priceTextBox.Text);
                int stockCount = int.Parse(countTextBox.Text);
                string uniqueId = idTextBox.Text;
                if (uniqueId.Length < 1)
                {
                    return;
                }
                if (desc.Length < 10)
                {
                    MessageBox.Show("Описание слишком короткое!");
                    return;
                }
                if (name.Length < 1)
                {
                    MessageBox.Show("Название не должно быть пустым!");
                    return;
                }
                if (measurementUnit.Length < 1)
                {
                    MessageBox.Show("Единица измерения не указана!");
                    return;
                }
                if (cost < 1)
                {
                    MessageBox.Show("Стоимость слишком низкая!");
                    return;
                }
                if (actuadlDiscount > maxDiscount)
                {
                    MessageBox.Show("Размер актуальной скидки не должен превышать размер макисмальной!");
                    return;
                }
                if (maxDiscount > 50)
                {
                    MessageBox.Show("Не превышайте размер максимальной скидки больше 50%!");
                    return;
                }

                if (_currentProduct == null)
                {
                    if (_selectedImageBinary == null)
                    {
                        try
                        {
                            string currentPath = Directory.GetCurrentDirectory();

                            string imagePath = System.IO.Path.Combine(currentPath, "images/picture.png");
                            _selectedImageBinary = File.ReadAllBytes(imagePath);
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка получения стандартного изображения.");
                            return;
                        }
                    }
                    try
                    {
                        using (var db = new SportDBEntities())
                        {
                            if (db.Product.Any(x => x.Id.ToLower().Equals(uniqueId.ToLower())))
                            {
                                MessageBox.Show("Товар с таким идентификатором уже существует в системе.");
                                return;
                            }
                            var prod = new Product
                            {
                                Id = uniqueId,
                                ActualDiscountAmount = actuadlDiscount,
                                MaxDiscountAmount = maxDiscount,
                                Cost = cost,
                                Description = desc,
                                Name = name,
                                ProductCategoryId = _selectedCategory,
                                ManufacturerId = _selectedManufacturer,
                                MeasurementUnit = measurementUnit,
                                StockQuanitity = stockCount,
                                Image = _selectedImageBinary

                            };
                            db.Product.Add(prod);
                            db.SaveChanges();
                            MessageBox.Show("Сохранение успешно!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка добавления нового товара. \n{ex.Message}");
                    }
                }
                else
                {
                    if(_selectedImageBinary == null)
                    {
                        _selectedImageBinary =  _currentProduct.Image;
                    }
                    using(var db = new SportDBEntities())
                    {

                        var prod = db.Product.FirstOrDefault(x => x.Id.Equals(_currentProduct.Id));
                        if(prod== null)
                        {
                            MessageBox.Show("Ошибка получения текущего товара");
                            return;
                        }

                        
                        prod.ActualDiscountAmount = actuadlDiscount;
                        prod.MaxDiscountAmount = maxDiscount;
                        prod.Cost = cost;
                        prod.Description = desc;
                        prod.Name = name;
                        prod.ProductCategoryId = _selectedCategory;
                        prod.ManufacturerId = _selectedManufacturer;
                        prod.MeasurementUnit = measurementUnit;
                        prod.StockQuanitity = stockCount;
                        prod.Image = _selectedImageBinary;

                       
                        
                        db.SaveChanges();
                        MessageBox.Show("Сохранение успешно!");
                    }
                    ProductsPage.Current.LoadData();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка получения данных.");
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

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(_currentProduct != null)
            {
                using(var db = new SportDBEntities())
                {
                    var prod = db.Product.FirstOrDefault(x=>x.Id.Equals(_currentProduct.Id));
                    if(prod != null)
                    {
                        db.Product.Remove(prod);
                        db.SaveChanges();
                        MessageBox.Show("Удаление успешно!");
                        ProductsPage.Current.LoadData();
                        this.Close();
                    }    
                }
            }
        }
    }
}
