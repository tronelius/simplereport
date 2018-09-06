using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace SimpleReport.App_Start
{
    public class ModuleFirstBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files.OrderByDescending(x => x.VirtualFile.Name == "module.js");
        }
    }
}