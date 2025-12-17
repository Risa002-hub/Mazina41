using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Controls;

namespace Mazina41
{
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        private OrderProduct currentOrderProduct = new OrderProduct();
        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string FIO, User user)
        {
            InitializeComponent();
            currentOrder.OrderClientID = user?.UserID;
            var currentPickups = Mazina41Entities.GetContext().PickUpPoint.ToList();
            PickupCombo.ItemsSource = currentPickups;

            ClientFIO.Text = FIO;
            OrderNumber.Text = selectedOrderProducts.First().OrderID.ToString();

            ProductListView.ItemsSource = selectedProducts;
            foreach (Product p in selectedProducts)
            {
                p.ProductQuantityInStock = 1;
                foreach (OrderProduct q in selectedOrderProducts)
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                        p.ProductQuantityInStock = q.OrderProductCount;
                }
            }

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            StartDate.Text = DateTime.Now.ToString();
            SetDeliveryDate();
        }


        private void SetDeliveryDate()
        {
            if (selectedOrderProducts == null || !selectedOrderProducts.Any())
            {
                DeliveryDate.Text = DateTime.Now.AddDays(6).ToString("dd.MM.yyyy");
                return;
            }


            int totalProductCount = selectedOrderProducts.Sum(op => op.OrderProductCount);

            int deliveryDays;
            if (totalProductCount < 3)
            {
                deliveryDays = 6;
            }
            else
            {
                deliveryDays = 3;
            }

            DateTime deliveryDate = DateTime.Now.AddDays(deliveryDays);
            StartDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
            DeliveryDate.Text = deliveryDate.ToString("dd.MM.yyyy");
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Product;
            if (product != null)
            {
                product.ProductQuantityInStock++;

                var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == product.ProductArticleNumber);
                if (selectedOP != null)
                {
                    selectedOP.OrderProductCount++;
                }

                ProductListView.Items.Refresh();
                SetDeliveryDate();
            }
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Product;
            if (product != null && product.ProductQuantityInStock > 0)
            {
                product.ProductQuantityInStock--;

                var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == product.ProductArticleNumber);
                if (selectedOP != null)
                {
                    selectedOP.OrderProductCount--;

                    if (selectedOP.OrderProductCount <= 0)
                    {
                        selectedOrderProducts.Remove(selectedOP);
                        selectedProducts.Remove(product);

                    }
                }
               
                ProductListView.Items.Refresh();
                SetDeliveryDate();
            }
        }

        private void Save2_Click(object sender, RoutedEventArgs e)
        {
            if (PickupCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи!");
                return;
            }

            if (selectedOrderProducts == null || !selectedOrderProducts.Any())
            {
                MessageBox.Show("Добавьте товары в заказ! КОРЗИНА ПУСТА");
                return;
            }
            
            try
            {

                currentOrder.OrderDate = DateTime.Now;
                currentOrder.OrderDeliveryDate = DateTime.Parse(DeliveryDate.Text);
                currentOrder.OrderPickupPoint = (PickupCombo.SelectedItem as PickUpPoint).PickUpPointID;
                currentOrder.OrderStatus = "Новый";


                var lastOrder = Mazina41Entities.GetContext().Order
                    .OrderByDescending(o => o.OrderCode)
                    .FirstOrDefault();
                currentOrder.OrderCode = lastOrder != null ? lastOrder.OrderCode + 1 : 1;


                currentOrder.OrderProduct = selectedOrderProducts;


                Mazina41Entities.GetContext().Order.Add(currentOrder);
                Mazina41Entities.GetContext().SaveChanges();

                MessageBox.Show("Заказ успешно сохранен!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}");
            }
        }
    }
}
