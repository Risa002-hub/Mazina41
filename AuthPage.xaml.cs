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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private DispatcherTimer _timer;
        private int _remainingSeconds = 0;

        public AuthPage()
        {
            InitializeComponent();
            InitializeTimer();
        }
        private void InitializeTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += OnTimerTick;
        }
        private void OnTimerTick (object sender, RoutedEventArgs e)
        {
            _remainingSeconds--;

            if(_remainingSeconds>0)
            {
                LoginBtn.Content = $"Ожидайте ({_remainingSeconds})";
                return;
            }
            _timer.Stop();
            ResetControls();
        }
        private void ResetControls()
        {
            LoginBtn.Content = " войти";
            LoginBtn.IsEnabled = LoginTB.IsEnabled = PassTB.IsEnabled = true;    
        }

        private void StartCooldown()
        {
            _remainingSeconds = 10;
            LoginBtn.IsEnabled = LoginTB.IsEnabled = PassTB.IsEnabled = false;
            LoginBtn.Content= $"Ожидайте ({_remainingSeconds})";
            _timer.Start();
        }
        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage(null));
            MessageBox.Show("Вы вошли как гость");
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PassTB.Text;
            if(login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            User user = Mazina41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                LoginTB.Text = "";
                PassTB.Text = "";
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
                LoginBtn.IsEnabled = false;
                // проработать время (10) секунд таймер
                LoginBtn.IsEnabled = true;
            }

        }
    }
}
