using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCNViewer.Models
{
    public class FcnModel
    {
        public FcnModel()
        {
            DirectoriesMonitored = new Dictionary<string, int>();
            AliasesMonitored = new List<string>();
            DirectoryMonitorProperties = new List<DirectoryMonitorProperties>();
        }
        public int FcnMode { get; set; }
        public int ActiveWatchers { get; set; }
        public Dictionary<string, int> DirectoriesMonitored { get; set; }
        public List<string> AliasesMonitored { get; set; }
        public List<DirectoryMonitorProperties> DirectoryMonitorProperties { get; set; }
    }
}