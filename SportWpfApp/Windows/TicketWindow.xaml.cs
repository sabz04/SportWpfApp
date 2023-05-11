using Microsoft.Office.Interop.Word;
using SportWpfApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Windows.Markup;

namespace SportWpfApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для TicketWindow.xaml
    /// </summary>
    public partial class TicketWindow : System.Windows.Window
    {
        private int _currentOrderId;
        private Order currentOrder = new Order();
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
                currentOrder = order;
                if (order == null)
                    return;
                orderDateTextBlock.Text = order.OrderDate.ToString();
                orderDeliveryTextBlock.Text = order.DelieveryDate.ToString();
                orderCodeTextBlock.Text = order.OrderGetCode.ToString();
                
                var products = db.OrderProduct.Where(x => x.OrderId == _currentOrderId).Select(x=>x.Product).ToList();

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

        private void getPDFButton_Click(object sender, RoutedEventArgs e)
        {
            var wordApp = new Word.Application();
            wordApp.Visible = false;
            Word.Document doc = wordApp.Documents.Add();
            Word.Paragraph paragraph = doc.Paragraphs.Add();

            Word.Range range = paragraph.Range;
            range.Text = $"Заказ №{currentOrder.Id}";
            paragraph.set_Style("Заголовок 1");
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            range.Bold = 1;
            range.InsertParagraphAfter();

            Word.Range range2 = paragraph.Range;
            range2.Text = $"Дата заказа: {orderDateTextBlock.Text}";
            paragraph.set_Style("Обычный");
            range2.InsertParagraphAfter();

            Word.Range range3 = paragraph.Range;
            range3.Text = $"Дата доставки: {orderDeliveryTextBlock.Text}";
            paragraph.set_Style("Обычный");
            range3.InsertParagraphAfter();

            Word.Range range4 = paragraph.Range;
            range4.Text = $"Состав заказа:";
            paragraph.set_Style("Обычный");
            range4.InsertParagraphAfter();

            var tableParagraph = doc.Paragraphs.Add();
            var tableRange = tableParagraph.Range;
            var studentsTable = doc.Tables.Add(tableRange, productsListBox.Items.Count, 2);
            studentsTable.Borders.InsideLineStyle = studentsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            studentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            Word.Range cellRange;
            cellRange = studentsTable.Cell(1, 1).Range;
            cellRange.Text = "Номер";
            cellRange = studentsTable.Cell(1, 2).Range;
            cellRange.Text = "Название товара";
            studentsTable.Rows[1].Range.Bold = 1;
            studentsTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            int row = 1;
            var stepSize = 1;
            foreach (var prod in productsListBox.Items)
            {
                cellRange = studentsTable.Cell(row + stepSize, 1).Range;
                cellRange.Text = row.ToString();
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                cellRange = studentsTable.Cell(row + stepSize, 2).Range;
                cellRange.Text = prod.ToString();
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                row++;
            }

            Word.Range range5 = paragraph.Range;
            range5.Text = $"Сумма заказа: {totalPriceTextBlock.Text} руб.";
            paragraph.set_Style("Обычный");
            range5.InsertParagraphAfter();

            Word.Range range6 = paragraph.Range;
            range6.Text = $"Скидка заказа: {totalDiscountTextBlock.Text}%";
            paragraph.set_Style("Обычный");
            range6.InsertParagraphAfter();

            Word.Range range7 = paragraph.Range;
            range7.Text = $"Пункт получения: {pickupPointTextBlock.Text}";
            paragraph.set_Style("Обычный");
            range7.InsertParagraphAfter();

            Word.Range range8 = paragraph.Range;
            range8.Text = $"Код для получения: {currentOrder.OrderGetCode.ToString()}";
            paragraph.set_Style("Обычный");
            range8.InsertParagraphAfter();

            String path = $@"E:\\talon{currentOrder.Id}.pdf";
            doc.SaveAs2(path, WdSaveFormat.wdFormatPDF);

            System.Diagnostics.Process.Start(path);
        }
    }
}
