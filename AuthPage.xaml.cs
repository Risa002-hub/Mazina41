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
using System.Windows.Threading;

namespace Mazina41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private DispatcherTimer _timer;
        private int _remainingSeconds = 0;
        private string currentCaptcha = "";
        private int failedAttempts = 0;


        private void GenerateCaptcha()
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            currentCaptcha = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            captchaOneWord.Text = currentCaptcha[0].ToString();
            captchaTwoWord.Text = currentCaptcha[1].ToString();
            captchaThreeWord.Text = currentCaptcha[2].ToString();
            captchaFourWord.Text = currentCaptcha[3].ToString();

            CaptchaPanel.Visibility = Visibility.Visible;
            CaptchaTB.Visibility = Visibility.Visible;
            CaptchaTB.Text = "";
        }
        public AuthPage()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                _remainingSeconds--;
                if (_remainingSeconds <= 0)
                {
                    _timer.Stop();
                    LoginBtn.Content = "Войти";
                    LoginBtn.IsEnabled = true;
                    LoginTB.IsEnabled = true;
                    PassTB.IsEnabled = true;
                }
                else
                {
                    LoginBtn.Content = $"Ждите ({_remainingSeconds})";
                }
            };
        }
        private void InitializeTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += OnTimerTick;
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            _remainingSeconds--;

            if (_remainingSeconds > 0)
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

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PassTB.Text;
            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            if (failedAttempts > 0)
            { 
                if (CaptchaTB.Text != currentCaptcha)
                {
                    MessageBox.Show("Вы не прошли проверку каптчи");
                    LoginBtn.IsEnabled= false;
                    _remainingSeconds = 10;
                    LoginBtn.Content = $"Ждите ({_remainingSeconds})";
                    _timer.Start();
                    LoginBtn.IsEnabled = true;

                    GenerateCaptcha();
                    return;
                }
            }


            User user = Mazina41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                LoginTB.Text = "";
                PassTB.Text = "";
                failedAttempts = 0;
                CaptchaPanel.Visibility = Visibility.Collapsed;
                CaptchaTB.Visibility = Visibility.Collapsed;
            }
            else
            {
                failedAttempts++;
                MessageBox.Show("Введены неверные данные");
                if (failedAttempts == 1)
                {
                    GenerateCaptcha();
                }
                else if (failedAttempts > 1) 
                {
                    LoginBtn.IsEnabled = false;
                    _remainingSeconds = 10;
                    LoginBtn.Content = $"Ждите ({_remainingSeconds})";
                    _timer.Start();
                    LoginTB.IsEnabled = true;

                }
                /*/ проработать время (10) секунд таймер
                _remainingSeconds = 10;
                LoginBtn.IsEnabled = false;
                LoginTB.IsEnabled= false;
                PassTB.IsEnabled= false;
                LoginBtn.Content = $"Ждите ({_remainingSeconds})";
                _timer.Start();
                LoginTB.IsEnabled= true;
                LoginBtn.IsEnabled= true;
                */
            }
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage(null));
            MessageBox.Show("Вы вошли как гость");
        }

       
    }
}