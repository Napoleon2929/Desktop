using Desktop.Models;
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
using System.Windows.Shapes;

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для ChooseWindow.xaml
    /// </summary>
    public partial class ChooseWindow : Window
    {
        private List<TokensToView> list;
        public bool isChosen = false;
        public TokensToView chosenToken = null;
        public ChooseWindow(string introduction, List<TokensToView> items)
        {
            try
            {
                InitializeComponent();
                Introduction.Content = introduction;
                list = items;
                foreach (var item in items)
                    ItemBox.Items.Add(item.ToString());
                ItemBox.SelectedIndex = 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            isChosen = true;
            if (ItemBox.SelectedIndex != -1)
                chosenToken = list[ItemBox.SelectedIndex];
            else
                return;
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
