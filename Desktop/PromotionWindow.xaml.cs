using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Desktop.Models;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для PromotionWindow.xaml
    /// </summary>
    public partial class PromotionWindow : Window
    {
        private HttpClient httpClient;
        public EmployeesToView employees;
        public PromotionWindow(EmployeesToView toView, HttpClient http)
        {
            httpClient = http;
            employees = toView;
            InitializeComponent();
            Position.Text = employees.Position;
            Security.Text = employees.Security.ToString();
        }

        private void Security_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
                e.Handled = true;
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if(Position.Text == "" || Security.Text == "")
            {
                MessageBox.Show("Fill all fields", "Error");
                return;
            }
            var json = JsonConvert.SerializeObject(new { Position = Position.Text, Security = Security.Text });
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"Login/?forPromotion={employees.Login}", data);
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    MessageBox.Show("You are unauthorized", "Error");
                    Close();
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    MessageBox.Show("You can not do it", "Access Error");
                    Close();
                    break;
                case HttpStatusCode.Forbidden:
                    MessageBox.Show("Check your security level", "Access Error");
                    break;
                case HttpStatusCode.NoContent:
                    employees.Position = Position.Text;
                    employees.Security = int.Parse(Security.Text);
                    Close();
                    break;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
