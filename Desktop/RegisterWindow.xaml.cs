using Desktop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private string uri = @"http://localhost:49837/api/";
        public RegisterWindow()
        {
            InitializeComponent();
        }
        public RegisterWindow(string login)
        {
            InitializeComponent();
            Login_Textbox.Text = login;
        }
        private async void Sign_Click(object sender, RoutedEventArgs e)
        {
            Sign.IsEnabled = false;
            Continue.IsEnabled = false;
            TokensToView token = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                using (HttpResponseMessage responseMessage = await httpClient.GetAsync(uri + $"Login/?login={Login_Textbox.Text}&password={Password_Textbox.Password}"))
                using (Stream stream = await responseMessage.Content.ReadAsStreamAsync())
                using (StreamReader reader = new StreamReader(stream))
                {
                    token = JsonConvert.DeserializeObject<TokensToView>(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Sign.IsEnabled = true;
                Continue.IsEnabled = true;
                MessageBox.Show(ex.Message);
                return;
            }

            if (token == null)
            {
                Sign.IsEnabled = true;
                Continue.IsEnabled = true;
                MessageBox.Show("You entered invalid data");
                return;
            }
            var window = new MainWindow(token);
            window.Show();
            this.Close();
        }

        private async void Continue_Click(object sender, RoutedEventArgs e)
        {
            Sign.IsEnabled = false;
            Continue.IsEnabled = false;
            List<TokensToView> list = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                using (HttpResponseMessage responseMessage = await httpClient.GetAsync(uri + $"Login/?currentLogin={Login_Textbox.Text}&password={Password_Textbox.Password}"))
                using (Stream stream = await responseMessage.Content.ReadAsStreamAsync())
                using (StreamReader reader = new StreamReader(stream))
                {
                    list = JsonConvert.DeserializeObject<List<TokensToView>>(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Sign.IsEnabled = true;
                Continue.IsEnabled = true;
                MessageBox.Show(ex.Message);
                return;
            }
            if (list == null)
            {
                Sign.IsEnabled = true;
                Continue.IsEnabled = true;
                MessageBox.Show("You entered invalid data");
                return;
            }
            if (list.Count == 0)
            {
                Sign.IsEnabled = true;
                Continue.IsEnabled = true;
                MessageBox.Show("You have not got valid session\nLog in, please");
                return;
            }
            var choose = new ChooseWindow("Choose session", list);
            choose.Closed += ChooseWindow_Closed;
            choose.Show();

        }
        private void ChooseWindow_Closed(object sender, EventArgs e)
        {
            var dialog=(ChooseWindow)sender;
            if (dialog.isChosen && dialog.chosenToken != null)
            {
                var window = new MainWindow(dialog.chosenToken);
                window.Show();
                this.Close();
            }
            else
            {
                Sign.IsEnabled = true;
                Continue.IsEnabled = true;
            }
        }
    }
}
