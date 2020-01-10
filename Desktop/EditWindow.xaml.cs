using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Desktop.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private HttpClient httpClient;
        public EmployeesToView employees;
        public EditWindow(HttpClient client, EmployeesToView toView)
        {
            httpClient = client;
            employees = toView;
            InitializeComponent();
            NameInput.Text = toView.Name;
            SurnameInput.Text = toView.Surname;
            BirthdayInput.Text = $"{toView.Birthday.Value.Date}";
            PositionInput.Text = toView.Position;
            SecurityInput.Text = toView.Security.ToString();
        }
        private async void OK_Click(object sender, RoutedEventArgs e)
        {
            if (NameInput.Text == "" || SurnameInput.Text == "" || BirthdayInput.Text == "" || PositionInput.Text == "" || SecurityInput.Text == "")
            {
                MessageBox.Show("Fill all fields");
                return;
            }
            else
            {
                employees.Name = NameInput.Text;
                employees.Surname = SurnameInput.Text;
                employees.Birthday = DateTime.Parse(BirthdayInput.Text);
                employees.Position = PositionInput.Text;
                employees.Security = int.Parse(SecurityInput.Text);
                var response = await httpClient.PutAsync($"Login/?forEdit={employees.Login}", new StringContent(JsonConvert.SerializeObject(employees), Encoding.UTF8, "application/json"));
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
                    case HttpStatusCode.InternalServerError:
                        MessageBox.Show("Server error!", "Error");
                        break;
                    case HttpStatusCode.NoContent:
                        Close();
                        break;
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SecurityInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
                e.Handled = true;
        }
        private void BirthdayInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text[0] != '.')
                e.Handled = true;
        }
    }
}
