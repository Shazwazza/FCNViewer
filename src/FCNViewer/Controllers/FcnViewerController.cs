using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;
using FCNViewer.Models;

namespace FCNViewer.Controllers
{
    [UseFcnReportFormatter]
    public class FcnViewerController : ApiController
    {        
        public FcnModel Get()
        {
            var fileChangesMonitor = typeof(HttpRuntime)
                .GetProperty("FileChangesMonitor", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .GetValue(null, null);

            var fcnVal = fileChangesMonitor.GetType()
                .GetField("_FCNMode", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(fileChangesMonitor);

            var dirs = (Hashtable)fileChangesMonitor.GetType()
                .GetField("_dirs", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(fileChangesMonitor);

            var aliases = (Hashtable)fileChangesMonitor.GetType()
                .GetField("_aliases", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(fileChangesMonitor);

            var dirMonAppPathInternal = fileChangesMonitor.GetType()
                .GetField("_dirMonAppPathInternal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(fileChangesMonitor);
            var dirMonAppPathInternalName = dirMonAppPathInternal == null ? null : dirMonAppPathInternal == null ? null : dirMonAppPathInternal.GetType()
                .GetField("Directory", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(dirMonAppPathInternal);
            var dirMonAppPathInternalIsDirMonAppPathInternal = dirMonAppPathInternal == null ? null : dirMonAppPathInternal.GetType()
                .GetField("_isDirMonAppPathInternal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(dirMonAppPathInternal);
            var dirMonAppPathInternalDirMonCompletion = dirMonAppPathInternal == null ? null : dirMonAppPathInternal.GetType()
                .GetField("_dirMonCompletion", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(dirMonAppPathInternal);

            var dirMonSubdirs = fileChangesMonitor.GetType()
                .GetField("_dirMonSubdirs", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(fileChangesMonitor);
            var dirMonSubdirsName = dirMonSubdirs == null ? null : dirMonSubdirs.GetType()
                .GetField("Directory", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(dirMonSubdirs);
            var dirMonSubdirsIsDirMonAppPathInternal = dirMonSubdirs == null ? null : dirMonSubdirs.GetType()
                .GetField("_isDirMonAppPathInternal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(dirMonSubdirs);
            var dirMonSubdirsDirMonCompletion = dirMonSubdirs == null ? null : dirMonSubdirs.GetType()
                .GetField("_dirMonCompletion", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                .GetValue(dirMonSubdirs);


            var t = typeof(HttpRuntime).Assembly.GetType("System.Web.DirMonCompletion");

            var dirMonCount = (int)t.InvokeMember("_activeDirMonCompletions",
                                                   BindingFlags.NonPublic
                                                   | BindingFlags.Static
                                                   | BindingFlags.GetField,
                                                   null,
                                                   null,
                                                   null);

            var result = new FcnModel
            {
                ActiveWatchers = dirMonCount,
                FcnMode = (int)fcnVal
            };

            foreach (DictionaryEntry d in dirs)
            {
                var fileMons = (Hashtable) d.Value.GetType()
                    .GetField("_fileMons", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase)
                    .GetValue(d.Value);

                result.DirectoriesMonitored[d.Key.ToString()] = fileMons.Count;
            }

            foreach (DictionaryEntry d in aliases)
            {
                var key = d.Key.ToString();
                var virtualPath = key.StartsWith(HttpRuntime.AppDomainAppPath) ? key.Substring(HttpRuntime.AppDomainAppPath.Length - 1) : key;
                result.AliasesMonitored.Add(virtualPath);
            }

            //Fills in the 'Single' dir monitor details
            result.DirectoryMonitorProperties.Add(new DirectoryMonitorProperties
            {                
                DirectoryMonitorName = dirMonAppPathInternalName?.ToString() ?? string.Empty,                
                FileChangesMonitorType = dirMonAppPathInternal?.ToString() ?? string.Empty,
                IsDirMonAppPathInternal = (bool?) dirMonAppPathInternalIsDirMonAppPathInternal ?? false,
                DirectoryMonitorCompletion = dirMonAppPathInternalDirMonCompletion?.ToString() ?? string.Empty,
            });

            //Fills in the default dir monitor details
            result.DirectoryMonitorProperties.Add(new DirectoryMonitorProperties
            {                
                DirectoryMonitorName = dirMonSubdirsName?.ToString() ?? string.Empty,                
                FileChangesMonitorType = dirMonSubdirs?.ToString() ?? string.Empty,
                IsDirMonAppPathInternal = (bool?)dirMonSubdirsIsDirMonAppPathInternal ?? false,
                DirectoryMonitorCompletion = dirMonSubdirsDirMonCompletion?.ToString() ?? string.Empty,
            });

            return result;
        }
    }
}
