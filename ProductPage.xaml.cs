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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mazina41
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
  

    public partial class ProductPage : Page
    {

         List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
         int newOrderID = 0;
        private User currentUser;
        public ProductPage(User user)
        {
            InitializeComponent();
            currentUser = user;

            if (user == null)
            {
                FIOTB.Text = "Гость";
                RoleTB.Text = "Гость";
            }
            else
            {
                FIOTB.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
                switch (user.UserRole)
                {
                    case 1:
                        RoleTB.Text = "Клиент"; break;
                    case 2:
                        RoleTB.Text = "Менджер"; break;
                    case 3:
                        RoleTB.Text = "Администратор"; break;
                }
            }
            var currentProducts = Mazina41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProducts;

            ComboType.SelectedIndex = 0;

            UpdateService();

            var Orders = Mazina41Entities.GetContext().Order.ToList();
            newOrderID = Orders.Count + 1;
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }


        private void TBSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateService();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateService();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateService();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateService();
        }
        private void UpdateService()
        {

            var currentProducts = Mazina41Entities.GetContext().Product.ToList();
            var raw_products_count = currentProducts.Count;

            if (TBoxSearch.Text.Length > 0)
                currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            if (ComboType.SelectedIndex == 0)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && (Convert.ToInt32(p.ProductDiscountAmount)) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) > 0 && (Convert.ToInt32(p.ProductDiscountAmount)) <= 9.99)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) > 9.99 && (Convert.ToInt32(p.ProductDiscountAmount)) <= 14.99)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) > 15 && (Convert.ToInt32(p.ProductDiscountAmount)) <= 100)).ToList();
            }

            currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            SearchTBl.Text = "кол-во " + Convert.ToString(currentProducts.Count) + " из " + Convert.ToString(raw_products_count);
            
            ProductListView.ItemsSource = currentProducts;
            ProductListView.ItemsSource = currentProducts.ToList();

            if (RButtonDown.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentProducts.OrderBy(p => p.ProductCost).ToList();
            }



        }

        

       

     
        private void Oreeder_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts = selectedProducts.Distinct().ToList();

            var orderProductsCopy = new List<OrderProduct>(selectedOrderProducts);
            var productsCopy = new List<Product>(selectedProducts);

            OrderWindow orderWindow = new OrderWindow(orderProductsCopy, productsCopy, FIOTB.Text, currentUser);
            orderWindow.ShowDialog();
            selectedProducts.Clear();
            selectedOrderProducts.Clear();
            ViewOrderBtn.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedIndex >= 0)
            {
                var prod = ProductListView.SelectedItem as Product;
                selectedProducts.Add(prod);

                var newOrderProd = new OrderProduct();
                newOrderProd.OrderID = newOrderID;

                newOrderProd.ProductArticleNumber = prod.ProductArticleNumber;
                newOrderProd.OrderProductCount = 1;

                var selOP = selectedOrderProducts.Where(p => Equals(p.ProductArticleNumber, prod.ProductArticleNumber));
                if (selOP.Count() == 0)
                {
                    selectedOrderProducts.Add(newOrderProd);
                }
                else
                {
                    foreach (OrderProduct p in selectedOrderProducts)
                    {
                        if (p.ProductArticleNumber == prod.ProductArticleNumber)
                            p.OrderProductCount++;
                    }
                }

                ViewOrderBtn.Visibility = Visibility.Visible;
                ProductListView.SelectedIndex = -1;
            }
        }
    }
}
