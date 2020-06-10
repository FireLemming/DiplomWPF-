using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfDip
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            
            InitializeComponent();
        }
        

        

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
                try
                {
                     Program prog = new Program(tbURL.Text, tbLogin.Text, tbAPI.Text);
                }
                catch
                {
                    MessageBox.Show("Ошибка входа. Проверьте правильность введенных данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                }
            this.Close();
            mainWindow.ShowDialog();
        }

        private void btLoginTest_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            Program prog = new Program("https://itqctest1.atlassian.net/", "alexey.kim@itqc.ru", "0LGTwkc7Nf9o946UImIW15A5");
            this.Close();
            mainWindow.ShowDialog();
        }

        private void tbURL_GotFocus(object sender, RoutedEventArgs e)//удалить значения по-умолчанию
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = string.Empty;
            textBox.GotFocus -= tbURL_GotFocus;
        }

        private void tbLogin_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = string.Empty;
            textBox.GotFocus -= tbURL_GotFocus;
        }

        private void tbAPI_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = string.Empty;
            textBox.GotFocus -= tbURL_GotFocus;
        }
    }
}
