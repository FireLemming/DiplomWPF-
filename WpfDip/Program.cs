using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using Atlassian.Jira;

namespace WpfDip
{
    public class Program
    {

        public Program(string URL, string login, string APItoken)
        {
            jiraLog = JiraLogin(URL, login, APItoken);
        }
        public Program()
        {
        }
        static Atlassian.Jira.Jira jiraLog;
        /// <summary>
        /// Метод для подключения к Jira
        /// </summary>
        public Atlassian.Jira.Jira JiraLogin(string URL, string login, string APItoken)
        {
            Atlassian.Jira.Jira jiraLog = Atlassian.Jira.Jira.CreateRestClient(new Atlassian.Jira.Remote.JiraRestClient(URL, login, APItoken));
            var URLserv = jiraLog.ServerInfo.GetServerInfoAsync().Result.BaseUrl;
            return jiraLog;
        }
        /// <summary>
        /// 
        /// Метод для формирования списка задач на экспорт
        /// </summary>
        //public List<string> CreateIssuesList(Dictionary<string, List<string>> filt)//удалить
        //{
        //    var issuesFileList = IssuesFilter(filt);//вызов метода фильрации
        //    //string[] parMas;//объявление массива, куда будут записываться значения из Jira
        //    //List<IssueWork> issueList = new List<IssueWork>();//Создания списка, в который будут записываться объекты класса
        //    //IssueWork IssWork;
        //    //List<string> issuesFileOutputList = new List<string>();
        //    //foreach (var pathFile in issuesFileList)
        //    //{
        //    //    List<Atlassian.Jira.Issue> issues;
        //    //    using (TextReader fs = File.OpenText(pathFile))
        //    //    {
        //    //        issues = JsonConvert.DeserializeObject<List<Issue>>(fs.ReadToEnd());
        //    //    }

        //    //    foreach (var c in issues)
        //    //    {
        //    //        parMas = FillingOutputArray(c, filt);//Вызов метода проверки на null
        //    //        if (filt.ContainsKey("paramchangefilter"))
        //    //        {
        //    //            if (Convert.ToInt32(parMas[11]) >= MainWindow.countLimit)//Тест отбора по количеству переходов на этапе вывода
        //    //            {
        //    //                IssWork = new IssueWork(parMas[0], parMas[1], parMas[2], parMas[3], parMas[4], parMas[5], parMas[6], parMas[7], parMas[8], parMas[9], parMas[10], parMas[11]);
        //    //                issueList.Add(IssWork);
        //    //            }

        //    //        }
        //    //        else
        //    //        {
        //    //            IssWork = new IssueWork(parMas[0], parMas[1], parMas[2], parMas[3], parMas[4], parMas[5], parMas[6], parMas[7], parMas[8], parMas[9], parMas[10], parMas[11]);
        //    //            issueList.Add(IssWork);//Добавление объекта в список
        //    //        }
        //    //    }

        //    //    var pathFileFilter = Path.GetTempFileName();
        //    //    File.AppendAllText(pathFileFilter, JsonConvert.SerializeObject(issueList));//временный json 
        //    //    issuesFileOutputList.Add(pathFileFilter);//добавление в список
        //    //}
        //    return issuesFileList;
        //}


        /// <summary>
        /// Метод для фильтрации задач
        /// </summary>
        public List<string> IssuesFilter(Dictionary<string, List<string>> filt)
        {

            var pathTempFile = Path.GetTempFileName();//создается временный файл, переменная хранит путь



            jiraLog.Issues.MaxIssuesPerRequest = int.MaxValue;
            //jiraLog.Issues.ValidateQuery = false;

            List<IssueWork> issuesList = new List<IssueWork>();

            List<string> issuesFileList = new List<string>();//список путей к файлам, хранящим данные
            List<string> issuesFileFilterList = new List<string>();
            List<Issue> issuesBegin = new List<Issue>();
            List<IssueWork> IssWork = new List<IssueWork>();
            issuesBegin.AddRange( jiraLog.Issues.GetIssuesFromJqlAsync("").Result.ToList()); ;//пишем первые 100 задач
            int count = 0;
            do {            //выгрузка всех задач в список
                var pathFile = Path.GetTempFileName();
                foreach (var c in issuesBegin)
                {
                    var parMas = FillingOutputArray(c, filt);//Вызов метода проверки на null
                    if (filt.ContainsKey("paramchangefilter"))
                    {
                        if (Convert.ToInt32(parMas[11]) >= MainWindow.countLimit)//Тест отбора по количеству переходов на этапе вывода
                            IssWork.Add(new IssueWork(parMas[0], parMas[1], parMas[2], parMas[3], parMas[4], parMas[5], parMas[6], parMas[7], parMas[8], parMas[9], parMas[10], parMas[11]));
                    }
                    else
                        IssWork.Add(new IssueWork(parMas[0], parMas[1], parMas[2], parMas[3], parMas[4], parMas[5], parMas[6], parMas[7], parMas[8], parMas[9], parMas[10], parMas[11]));
                }

                File.AppendAllText(pathFile, JsonConvert.SerializeObject(IssWork));//временный json 
                IssWork.Clear();
                issuesFileList.Add(pathFile);//добавление в список

                //issuesList.AddRange(issues);

                issuesBegin.Clear();
                count += 100;
                issuesBegin.AddRange(jiraLog.Issues.GetIssuesFromJqlAsync("", startAt: count).Result.ToList());
                string t = "";
            } while (issuesBegin.Count != 0);
            
            //
            //var issues = jiraLog.Issues.Queryable.Where(c =>
            //{
            //if (filt["summary"].Where(t => t == c.Summary.ToLower().Replace(" ", "")).Count() > 0)
            //{
            //    if (filt["key"].Where(t => t == c.Key.ToString().ToLower().Replace(" ", "")).Count() > 0)
            //        if (filt["priority"].Where(t => t == c.Priority.ToString().ToLower().Replace(" ", "")).Count() > 0)
            //            if (filt["status"].Where(t => t == c.Status.ToString().ToLower().Replace(" ", "")).Count() > 0)
            //                if (filt["type"].Where(t => t == c.Type.ToString().ToLower().Replace(" ", "")).Count() > 0)
            //                    if (filt["created"].Where(t => t == c.Created.ToString().ToLower().Replace(" ", "")).Count() > 0)
            //                        if (filt["environment"].Where(t => t == c.Environment.ToString().ToLower().Replace(" ", "")).Count() > 0)
            //                            if (filt["project"].Where(t => t == c.Project.ToLower().Replace(" ", "")).Count() > 0)
            //                                if (filt["assigneeuser"].Where(t => t == c.AssigneeUser.DisplayName.ToLower().Replace(" ", "")).Count() > 0)
            //                                    if (filt["reporteruser"].Where(t => t == c.ReporterUser.DisplayName.ToLower().Replace(" ", "")).Count() > 0)
            //                                        return true;
            //}
            //else return false;


            foreach (var pathFile in issuesFileList)
            {
                List<IssueWork> issues;
                using (TextReader fs = File.OpenText(pathFile))
                {
                    issues = JsonConvert.DeserializeObject<List<IssueWork>>(fs.ReadToEnd());
                }

                if (filt.ContainsKey("summary"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>//если условие выполняется - нужная задача записывается список
                        {
                            if (filt["summary"].Where(t => t == c.Summary.ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))//Возвращает все задачи, где проект в Jira равен пользовательскому и задача не записана в список
                            return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();
                }

                if (filt.ContainsKey("key"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>
                        {
                            if (filt["key"].Where(t => t == c.Key.ToString().ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))
                                return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();
                }

                if (filt.ContainsKey("priority"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>
                        {
                            if (filt["priority"].Where(t => t == c.Priority.ToString().ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))
                                return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();

                }

                if (filt.ContainsKey("status"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>
                        {
                            if (filt["status"].Where(t => t == c.Status.ToString().ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))
                                return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();
                }

                if (filt.ContainsKey("type"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>
                        {
                            if (filt["type"].Where(t => t == c.Type.ToString().ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))
                                return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();
                }

                if (filt.ContainsKey("created"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>
                        {
                            if (filt["created"].Where(t => t == c.Created.ToString().ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))
                                return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();
                }

                if (filt.ContainsKey("environment"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>
                        {
                            if (filt["environment"].Where(t => t == c.Environment.ToString().ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))
                                return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();
                }

                if (filt.ContainsKey("project"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>
                        {
                            if (filt["project"].Where(t => t == c.Project.ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))
                                return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();
                }

                if (filt.ContainsKey("assigneeuser"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>
                        {
                            if (filt["assigneeuser"].Where(t => t == c.AssigneeUser.ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))
                                return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();
                }

                if (filt.ContainsKey("reporteruser"))
                {
                    issuesList.AddRange(
                        issues.Where(c =>
                        {
                            if (filt["reporteruser"].Where(t => t == c.ReporterUser.ToLower().Replace(" ", "")).Count() > 0 && !issuesList.Contains(c))
                                return true;
                            else return false;
                        }).ToList());
                    issues.Clear();
                    issues.AddRange(issuesList);
                    issuesList.Clear();
                }
                var pathFileFilter = Path.GetTempFileName();
                File.AppendAllText(pathFileFilter, JsonConvert.SerializeObject(issues));//временный json 
                issuesFileFilterList.Add(pathFileFilter);//добавление в список
            }
            return issuesFileFilterList;
        }
        /// <summary>
        /// Метод для проверки на Null значений из Jira
        /// </summary>
        static string[] FillingOutputArray(Atlassian.Jira.Issue c, Dictionary<string, List<string>> filt)
        {
            string[] parMas = new string[12];//создание массива, в котором будут храниться значения параметров из Jira
            if (c.Summary != null)
            {
                parMas[0] = c.Summary;
            }
            else parMas[0] = "Аннотация не задана";

            if (c.Key != null)
            {
                parMas[1] = c.Key.ToString();
            }
            else parMas[1] = "Ключ не задан";

            if (c.Priority != null)
            {
                parMas[2] = c.Priority.ToString();
            }
            else parMas[2] = "Приоритет не задан";

            if (c.Status != null)
            {
                parMas[3] = c.Status.ToString();
            }
            else parMas[3] = "Статус не задан";

            if (c.Type != null)
            {
                parMas[4] = c.Type.ToString();
            }
            else parMas[4] = "Тип не задан";

            if (c.Created != null)
            {
                parMas[5] = c.Created.ToString();
            }
            else parMas[5] = "Время создания не задано";

            if (c.Environment != null)
            {
                parMas[6] = c.Environment;
            }
            else parMas[6] = "Окружение не задано";

            if (c.Project != null)
            {
                parMas[7] = c.Project;
            }
            else parMas[7] = "Принадлежность к проекту не задана";

            if (c.AssigneeUser != null)
                parMas[8] = c.AssigneeUser.DisplayName;
            else
                parMas[8] = "Исполнитель не задан";

            if (c.ReporterUser != null)
            {
                parMas[9] = c.ReporterUser.DisplayName;
            }
            else parMas[9] = "Создатель не задан";

            if (c.Description != null)
            {
                parMas[10] = c.Description;
            }
            else parMas[10] = "Описание не задано";

            parMas[11] = paramChangeCountWork(c, filt);

            return parMas;
        }
        /// <summary>
        /// Метод для подсчёта изменений статуса
        /// </summary>
        static string paramChangeCountWork (Atlassian.Jira.Issue c, Dictionary<string, List<string>> filt)
        {
            var changeLog = jiraLog.Issues.GetChangeLogsAsync(c.Key.ToString()).Result;
            int count = 0;
            var list = changeLog.ToList();
            list.Reverse();
            List<string> ListFromValue = new List<string>();//лист начальных значений Jira
            List<string> ListToValue = new List<string>();//лист конечных значений Jira
            List<string> ListParam = new List<string>();//лист со списком параметров из фильтра
            List<string> ListParamBuf = new List<string>();//лист со списком значений не разделенных параметров из фильтра
            List<string> ListParamValue = new List<string>();//лист со списком значений разделенных параметров из фильтра
            if (filt.ContainsKey("paramchangecount") || filt.ContainsKey("paramchangefilter"))
            {
                if(filt.ContainsKey("paramchangecount"))
                filt["paramchangecount"].ForEach(s =>
                {
                    ListParam.AddRange(s.Split(';'));//дробим список на значение(четные) и тип(нечетные)
                    for (int i = 0; i < ListParam.Count; i++)
                    {
                        if (i % 2 == 0)
                            ListParamBuf.Add(ListParam[i]);//Формируем список значений
                    }
                });

                if(filt.ContainsKey("paramchangefilter"))
                    filt["paramchangefilter"].ForEach(s =>
                    {
                        ListParam.AddRange(s.Split(';'));//дробим список на значение(четные) и тип(нечетные)
                        for (int i = 0; i < ListParam.Count; i++)
                        {
                            if (i % 2 == 0)
                                ListParamBuf.Add(ListParam[i]);//Формируем список значений
                        }
                    });

                ListParam.RemoveAll(c => ListParamBuf.Contains(c));//удаляем из списка параметров из фильтра все значения данных параметров
                ListParam = ListParam.Distinct().ToList();//удаляем все повторяющиеся значения, т.е. оставляем только одно значение - тип параметра
                foreach (var e in list)
                {
                    e.Items.ToList().ForEach(s =>
                    {
                        if (s.FieldName.Contains(ListParam[0]))
                        {
                            ListFromValue.Add(s.FromValue.ToLower().Replace(" ", "")); //Заполнение списка изначальными значениями
                            ListToValue.Add(s.ToValue.ToLower().Replace(" ", "")); //Заполнение списка конечными значениями
                        }
                    });
                }
                ListParamBuf.ForEach(t =>
                {

                    ListParamValue.AddRange(t.Split('-'));//записываем изначальное и конечное значения в список
                    for (int i = 0; i < ListFromValue.Count(); i++)
                    {
                        if (ListFromValue[i].Contains(ListParamValue[0].ToLower().Trim()) && ListToValue[i].Contains(ListParamValue[1].ToLower().Trim()))//проверка на то, что происходил переход статуса из изначального пользовательского значения в конечное
                            count++;
                    }
                    ListParamValue.Clear();
                });
                return count.ToString();
            }
            else
                return "Пользователь не задал значения";
        }
        /// <summary>
        /// Метод для экспорта CSV
        /// </summary>
        public void CSVWork(List<string> issueFileList, string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//прописать в инструментарий
            var issueList = new List<IssueWork>();
            foreach (var pathFile in issueFileList)
            {
                using (TextReader fs = File.OpenText(pathFile))
                {
                    issueList.AddRange(JsonConvert.DeserializeObject<List<IssueWork>>(fs.ReadToEnd()));
                }
            }
                StringBuilder csv = new StringBuilder();
            csv.AppendLine("Summary" + ";" +
                    "Key" + ";" +
                    "Priority" + ";" +
                    "Status" + ";" +
                    "Type" + ";" +
                    "Created" + ";" +
                    "Environment" + ";" +
                    "Project" + ";" +
                    "AssigneeUser" + ";" +
                    "ReporterUser" + ";" +
                    "Description" + ";" +
                    "paramChangeCount");
            foreach (var s in issueList)
            {
                csv.AppendLine(Regex.Replace(s.Summary.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.Key.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.Priority.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.Status.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.Type.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.Created.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.Environment.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.Project.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.AssigneeUser.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.ReporterUser.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.Description.Replace(";", ":"), @"\s+", " ") + ";" +
                    Regex.Replace(s.ParamChangeCount.Replace(";", ":"), @"\s+", " "));
            }

            File.WriteAllText(path, csv.ToString(), Encoding.GetEncoding(1251));//не работает кодировка

        }
        /// <summary>
        /// 
        /// Метод для экспорта в JSON
        /// </summary>
        public void JsonWork(List<string> issueFileList, string path)
        {
            var issueList = new List<IssueWork>();
            foreach (var pathFile in issueFileList)
            {
                using (TextReader fs = File.OpenText(pathFile))
                {
                    issueList.AddRange(JsonConvert.DeserializeObject<List<IssueWork>>(fs.ReadToEnd()));
                }
            }
            File.WriteAllText(path, JsonConvert.SerializeObject(issueList));
        }
    }
}
