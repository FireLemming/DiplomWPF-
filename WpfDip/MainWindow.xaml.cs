using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.IO;
using Newtonsoft.Json;

namespace WpfDip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dictionary<string, List<string>> filt = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> filtBack = new Dictionary<string, List<string>>();
        Program prog = new Program();
        List<string> issuesFileOutputList = new List<string>();
        List<IssueWork> issueList = new List<IssueWork>();
        public static int countLimit;
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Метод для открытия окна выставления фильтров(нужен для сохранения страрых значений при снятии галочки)
        /// </summary>
        public bool OpenFilterWindow(bool check, string type)
        {
            if (check)//если чекбокс проставлен
            {
                if (filtBack.ContainsKey(type))//если ключ существует в резервном списке
                {
                    filt.Add(type, filtBack[type]);//довавление в список фильтров, заданных ранее
                    filtBack.Remove(type);//удавление ключа/очистка резервного списка
                }
                FilterWindow fw = new FilterWindow(filt, type);
                fw.ShowDialog();
            }
            else
            {
                if (filt.ContainsKey(type))//если существует ключ в основном словаре
                {
                    if (filtBack.ContainsKey(type))//если существует ключ в резервном словаре
                        filtBack[type].AddRange(filt[type]);//Добавление значений словаря во второй словарь для хранения фильтров
                    else
                        filtBack.Add(type, filt[type]);
                    filt.Remove(type);//удавление ключа/очистка основного списка
                }
            }
            if (filtBack.Count == 0 && filt.Count == 0)
                return false;//если фильтиры пустые - чекбокс отожмётся
            else return true;
        }

        private void btView_Click(object sender, RoutedEventArgs e)
        {
            issueList.Clear();
            dgAll.ItemsSource = null;
            dgAll.Items.Refresh();

            issuesFileOutputList.AddRange(prog.IssuesFilter(filt));
            foreach (var pathFile in issuesFileOutputList)
            {
                using (TextReader fs = File.OpenText(pathFile))
                {
                    issueList.AddRange(JsonConvert.DeserializeObject<List<IssueWork>>(fs.ReadToEnd()));
                }
            }
            dgAll.ItemsSource = issueList;
            

            MessageBox.Show("Выборка задач завершена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btExportCSV_Click(object sender, RoutedEventArgs e)
        {
            //if (dgAll.Items.Count > 0)
           // {
                ExportWindow ew = new ExportWindow(issuesFileOutputList, "csv");
                ew.ShowDialog();
           // }
            //else MessageBox.Show("Сначала выберите задачи", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btExportJSON_Click(object sender, RoutedEventArgs e)
        {
            if (dgAll.Items.Count > 0)
            {
                ExportWindow ew = new ExportWindow(issuesFileOutputList, "json");
                ew.ShowDialog();
            }
            else MessageBox.Show("Сначала выберите задачи", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
        }        

        private void btDel_Click(object sender, RoutedEventArgs e) 
        {
            CheckBox cb = new CheckBox();
            List<IssueWork> RemoveList = new List<IssueWork>();
            if (dgAll.ItemsSource == null)//проверка таблицы на пустоту
            {
                MessageBox.Show("Удаление невозмножно, таблица пуста", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            foreach (var s in dgAll.ItemsSource)
            {
                cb = dgAll.Columns[0].GetCellContent(s) as CheckBox;

                if(cb!=null)
                    if ((bool)cb.IsChecked)
                    {
                        RemoveList.Add(s as IssueWork);
                    }
            }
            issueList.RemoveAll(c => RemoveList.Contains(c));
            dgAll.ItemsSource = null;
            dgAll.Items.Refresh();
            dgAll.ItemsSource = issueList;
        }

        private void cbSummary_Click(object sender, RoutedEventArgs e)
        {
            string type = "summary";
            bool check = (bool)cbSummary.IsChecked;//эти строки для каждово cb свои, а дальше вызов метода
            if (!OpenFilterWindow(check, type))
                cbSummary.IsChecked = false;
        }

        private void cbKey_Click(object sender, RoutedEventArgs e)
        {
            string type = "key";
            bool check = (bool)cbKey.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbKey.IsChecked = false;
        }

        private void cbPriority_Click(object sender, RoutedEventArgs e)
        {
            string type = "priority";
            bool check = (bool)cbPriority.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbPriority.IsChecked = false;
        }

        private void cbStatus_Click(object sender, RoutedEventArgs e)
        {
            string type = "status";
            bool check = (bool)cbStatus.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbStatus.IsChecked = false;
        }

        private void cbType_Click(object sender, RoutedEventArgs e)
        {
            string type = "type";
            bool check = (bool)cbType.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbType.IsChecked = false;
        }

        private void cbCreated_Click(object sender, RoutedEventArgs e)
        {
            string type = "created";
            bool check = (bool)cbCreated.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbCreated.IsChecked = false;
        }

        private void cbEnvironment_Click(object sender, RoutedEventArgs e)
        {
            string type = "environment";
            bool check = (bool)cbEnvironment.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbEnvironment.IsChecked = false;
        }

        private void cbProject_Click(object sender, RoutedEventArgs e)
        {
            string type = "project";
            bool check = (bool)cbProject.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbProject.IsChecked = false;
        }

        private void cbAssigneeUser_Click(object sender, RoutedEventArgs e)
        {
            string type = "assignieeuser";
            bool check = (bool)cbAssigneeUser.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbAssigneeUser.IsChecked = false;
        }

        private void cbReporterUser_Click(object sender, RoutedEventArgs e)
        {
            string type = "reporteruser";
            bool check = (bool)cbReporterUser.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbReporterUser.IsChecked = false;
        }

        private void cbParamChangeCount_Click(object sender, RoutedEventArgs e)
        {
            string type = "paramchangecount";
            bool check = (bool)cbParamChangeCount.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbParamChangeCount.IsChecked = false;
        }

        private void cbParamChangeFilter_Click(object sender, RoutedEventArgs e)
        {
            string type = "paramchangefilter";
            bool check = (bool)cbParamChangeFilter.IsChecked;
            if (!OpenFilterWindow(check, type))
                cbParamChangeFilter.IsChecked = false;
        }
    }
}
