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
        public ProductPage()
        {
            InitializeComponent();

            var currentShop = Mazina41Entities.GetContext().Product.ToList();
            ShopListView.ItemsSource = currentShop;
            ComboType.SelectedIndex = 0;
            UpdateService();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void UpdateService()
        {

            var currentShop = Mazina41Entities.GetContext().Product.ToList();
            var raw_products_count = currentShop.Count;

            if (TBoxSearch.Text.Length > 0)
                currentShop = currentShop.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            if (ComboType.SelectedIndex == 0)
            {
                currentShop = currentShop.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && (Convert.ToInt32(p.ProductDiscountAmount)) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentShop = currentShop.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) > 0 && (Convert.ToInt32(p.ProductDiscountAmount)) <= 9.99)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentShop = currentShop.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) > 9.99 && (Convert.ToInt32(p.ProductDiscountAmount)) <= 14.99)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentShop = currentShop.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) > 14.99 && (Convert.ToInt32(p.ProductDiscountAmount)) <= 100)).ToList();
            }

            currentShop = currentShop.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            SearchResultTB.Text = "кол-во " + Convert.ToString(currentShop.Count) + " из " + Convert.ToString(raw_products_count);
          
            ShopListView.ItemsSource = currentShop;
            ShopListView.ItemsSource = currentShop.ToList();

            if (RButtonDown.IsChecked.Value)
            {
                ShopListView.ItemsSource = currentShop.OrderByDescending(p => p.ProductCost).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
                ShopListView.ItemsSource = currentShop.OrderBy(p => p.ProductCost).ToList();
            }

        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateService();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateService();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
