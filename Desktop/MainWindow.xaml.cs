using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Desktop.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool validateToken = true;
        private HttpClient httpClient;
        EmployeesToView myProfile = null;
        Search search;
        public MainWindow(TokensToView tokens)
        {
            try
            {
                httpClient = new HttpClient()
                {
                    DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", tokens.Token) },
                    BaseAddress = new Uri(@"http://localhost:49837/api/")
                };
                InitializeComponent();
                GetProfile();
                Endtime.Content = tokens.End_time;
                search = new Search();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void GetProfile()
        {
            var response = await httpClient.GetAsync("Profile");
            if (!GetError(response.StatusCode, true))
                return;
            myProfile = await Response<EmployeesToView>.GetResponse(response);
            Login.Text = myProfile.Login;
            UserName.Text = $"{myProfile.Name} {myProfile.Surname}";
            Birthday.Text = myProfile.Birthday.ToString();
            HiringTime.Text = myProfile.Hiring_Time.ToString();
            Position.Text = myProfile.Position;
            Security.Text = myProfile.Security.ToString();
            //Profile.Content = myProfile;
            GetLogin($"{myProfile.Name} {myProfile.Surname}");
            response.Dispose();
        }
        private void GetLogin(string toShow)
        {
            LoginName.Content = toShow;
        }
        private bool GetError(HttpStatusCode httpStatus, bool isRequiredLogIn)
        {
            switch (httpStatus)
            {
                case HttpStatusCode.Unauthorized:
                    MessageBox.Show("Your session have ended\nPlease Log In again","Error");
                    if (isRequiredLogIn)
                    {
                        validateToken = false;
                        var window = new RegisterWindow();
                        window.Show();
                        Close();
                        return false;
                    }
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    MessageBox.Show("Your security level less than required level", "Error");
                    return false;
                case HttpStatusCode.Forbidden:
                    MessageBox.Show("Server do not accept this method", "Error");
                    return false;
                case HttpStatusCode.InternalServerError:
                    return false;
            }
            return true;
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new RegisterWindow();
                window.Show();
                Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,ex.GetType().Name);
            }
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            validateToken = false;
            new RegisterWindow(myProfile.Login).Show();
            Close();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            BuildSearchingItems();
        }
        private async void BuildSearchingItems()
        {
            HttpResponseMessage responseMessage;
            string searchText = SearchLogin.Text;
            try
            {
                responseMessage = await httpClient.GetAsync($"Profile/?search={searchText}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if (!GetError(responseMessage.StatusCode, true))
                return;
            search = new Search(await Response<IEnumerable<EmployeesToView>>.GetResponse(responseMessage));
            SearchField.Children.Clear();
            for (var i = 0; i < search.FoundItems.Count; i++)
            {
                SearchField.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var border = new Border()
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                };

                StackPanel panel = new StackPanel();
                panel.Children.Add(new TextBlock()
                {
                    Text = $"{search.FoundItems[i].Status}\n{search.FoundItems[i].Name} {search.FoundItems[i].Surname}\n{search.FoundItems[i].Position}"
                });
                var button = new Button()
                {
                    Content = "Profile",
                    Width = 60,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                button.Click += Profile_Click;
                button.Name = $"b{i}";
                panel.Children.Add(button);

                var label = new Label()
                {
                    Content = panel
                };
                border.Child = label;
                SearchField.Children.Add(border);
                Grid.SetRow(border, i);
            }
        }
        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var window = new ViewWindow(search.FoundItems[int.Parse(button.Name.Substring(1))], httpClient);
            window.Show();
            window.Closed += Profile_Closed;
        }

        private void Profile_Closed(object sender, EventArgs e)
        {
            BuildSearchingItems();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserControl.SelectedIndex == 0)
                GetProfile();
            if (UserControl.SelectedIndex != 1)
            {
                SearchLogin.Text = "";
                SearchField.Children.Clear();
            }
        }
        private void LoginName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UserControl.SelectedIndex = 0;
        }
        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                if (validateToken)
                    await httpClient.DeleteAsync("Logout");
                httpClient.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            new ChangePassword(httpClient).Show();
        }

        private void SecurityInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
                e.Handled = true;
        }
        private void BirthdayInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text[0]!='.')
                e.Handled = true;
        }

        private async void Hire_Click(object sender, RoutedEventArgs e)
        {
            if (LoginInput.Text == "" || NameInput.Text == "" || SurnameInput.Text == "" || BirthdayInput.Text == "" || PositionInput.Text == "" || SecurityInput.Text == "")
            {
                MessageBox.Show("Fill all fields");
                return;
            }
            EmployeesToView beginner = new EmployeesToView();
            beginner.Login = LoginInput.Text;
            beginner.Name = NameInput.Text;
            beginner.Surname = SurnameInput.Text;
            beginner.Birthday = DateTime.Parse(BirthdayInput.Text);
            beginner.Position = PositionInput.Text;
            beginner.Security = int.Parse(SecurityInput.Text);
            try
            {
                var response = await httpClient.PostAsync("Login", new StringContent(JsonConvert.SerializeObject(beginner), Encoding.UTF8, "application/json"));
                if (!GetError(response.StatusCode, true))
                    return;
                else
                    MessageBox.Show($"Login - {beginner.Login}\nPassword - qwerty", "For beginner");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
