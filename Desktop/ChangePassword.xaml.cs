using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private HttpClient httpClient;
        public ChangePassword(HttpClient client)
        {
            httpClient = client;
            InitializeComponent();
        }
        private async void Change_Click(object sender, RoutedEventArgs e)
        {
            if(NewPassword.Password != ConfirmPassword.Password)
            {
                MessageBox.Show("You have not confirm your password", "Error");
                return;
            }
            try
            {
                var json = JsonConvert.SerializeObject(new { OldPassword = OldPassword.Password, NewPassword = NewPassword.Password });
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync("Login", data);
                if(response.StatusCode == HttpStatusCode.Forbidden)
                    MessageBox.Show("You have writen uncorrect your old password", "Error");
                if(response.StatusCode == HttpStatusCode.NoContent)
                {
                    MessageBox.Show("Success!");
                    Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
