using System;
using System.Collections.Generic;
using System.Text;

namespace WpfDip
{
    public class IssueWork
    {
        public IssueWork(string summary, string key, string priority, string status, string type, string created, string environment, string project, string assigneeUser, string reporterUser, string description, string paramChangeCount)
        {
            Summary = summary;
            Key = key;
            Priority = priority;
            Status = status;
            Type = type;
            Created = created;
            Environment = environment;
            Project = project;
            AssigneeUser = assigneeUser;
            ReporterUser = reporterUser;
            Description = description;
            ParamChangeCount = paramChangeCount;
        }
        public string Summary { get; set; }
        public string Key { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Created { get; set; }
        public string Environment { get; set; }
        public string Project { get; set; }
        public string ReporterUser { get; set; }
        public string AssigneeUser { get; set; }
        public string Description { get; set; }
        public string ParamChangeCount { get; set; }
    }
}
