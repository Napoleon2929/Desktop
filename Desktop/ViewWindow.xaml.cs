using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Desktop.Models;
using Newtonsoft.Json;

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для ViewWindow.xaml
    /// </summary>
    public partial class ViewWindow : Window
    {
        private EmployeesToView employees;
        private HttpClient httpClient;
        public ViewWindow(EmployeesToView toView)
        {
            InitializeComponent();
            employees = toView;
            SetTextBlocks();
        }
        private void SetTextBlocks()
        {
            Title = employees.Login;
            Login.Text = employees.Login;
            UserName.Text = $"{employees.Name} {employees.Surname}";
            Birthday.Text = employees.Birthday.ToString();
            HiringTime.Text = employees.Hiring_Time.ToString();
            Position.Text = employees.Position;
            Security.Text = employees.Security.ToString();
        }
        public ViewWindow(EmployeesToView toView, HttpClient http) 
            :this(toView)
        {
            httpClient = http;
        }

        private void Promotion_Click(object sender, RoutedEventArgs e)
        {
            var window = new PromotionWindow(employees, httpClient);
            window.Closed += Promotion_Closed;
            window.Show();
        }

        private void Promotion_Closed(object sender, EventArgs e)
        {
            var promotionWindow = (PromotionWindow)sender;
            employees = promotionWindow.employees;
            SetTextBlocks();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            var editWindow = (EditWindow)sender;
            employees = editWindow.employees;
            SetTextBlocks();
        }
        private async void Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await httpClient.PutAsync($"Login/?forChange={employees.Login}", new StringContent(JsonConvert.SerializeObject("qwerty"), Encoding.UTF8, "application/json"));
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        MessageBox.Show("You are unauthorized", "Error");
                        Close();
                        break;
                    case HttpStatusCode.MethodNotAllowed:
                        MessageBox.Show("You can not do it", "Access Error");
                        break;
                    case HttpStatusCode.Forbidden:
                        MessageBox.Show("Check your security level", "Access Error");
                        break;
                    case HttpStatusCode.NoContent:
                        break;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private  void Edit_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditWindow(httpClient, employees);
            window.Closed += Window_Closed;
            window.Show();
        }
        private async void Dismission_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"Login/?forDismissal={employees.Login}");
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        MessageBox.Show("You are unauthorized", "Error");
                        Close();
                        break;
                    case HttpStatusCode.MethodNotAllowed:
                        MessageBox.Show("You can not do it", "Access Error");
                        break;
                    case HttpStatusCode.Forbidden:
                        MessageBox.Show("Check your security level", "Access Error");
                        break;
                    case HttpStatusCode.NoContent:
                        MessageBox.Show("Employee have dismissed", "Success!");
                        Close();
                        break;
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
