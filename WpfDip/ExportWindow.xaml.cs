using Atlassian.Jira;
using Microsoft.Win32;
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
    /// Логика взаимодействия для ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        string type;
        List<string> issueList;
        
        public ExportWindow(List<string> IssueList, string Type)
        {
            InitializeComponent();
            issueList = IssueList;
            type = Type.Trim().ToLower();
            if(type == "csv")
                lbHeader.Content = "Экспорт в CSV-Файл";
            if (type == "json")
                lbHeader.Content = "Экспорт в JSON-Файл";
        }

        private void tbPath_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = string.Empty;
            textBox.GotFocus -= tbPath_GotFocus;
        }

        private void btFolder_Click(object sender, RoutedEventArgs e) //возможно придется удалять 
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.ShowDialog();
            tbPath.Text = sd.FileName;
        }

        private void btExport_Click(object sender, RoutedEventArgs e)
        {
            Program prog = new Program();
            if (type == "csv")
            {
                try
                {
                    prog.CSVWork(issueList, tbPath.Text + ".csv");
                }
                catch
                {
                    MessageBox.Show("Ошибка пути. Попробуйте снова", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                this.Close();
            }

            if(type == "json")
            {
                try
                {
                    prog.JsonWork(issueList, tbPath.Text + ".json");
                }
                catch
                {
                    MessageBox.Show("Ошибка пути. Попробуйте снова", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                this.Close();
            }
        }
    }
}
