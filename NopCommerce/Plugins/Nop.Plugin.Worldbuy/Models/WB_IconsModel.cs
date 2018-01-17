using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Worldbuy.Models
{
    public static class WB_Icons
    {
        public static IList<WB_Icon> Icons { get; set; }
        public static void LoadIcon()
        {
            Icons = new List<WB_Icon>();
            var folderIcon = "~/Plugins/Worldbuy.Plugin/Content/img/worldbuy/icons";
            var pathFolder = HttpContext.Current.Server.MapPath(folderIcon);
            var dir = new DirectoryInfo(pathFolder);
            var files = dir.GetFiles();
            if (files != null)
            {
                foreach (var file in files)
                {
                    string filename = Path.GetFileName(file.FullName);
                    var icon = new WB_Icon
                    {
                        FullName = folderIcon + "/" + filename
                    };
                    Icons.Add(icon);
                }
            }
        }
    }
    public class WB_Icon
    {
        public string FileName
        {
            get
            {
                return Path.GetFileName(FullName);
            }
        }
        public string FileNameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FullName);
            }
        }
        public string FullName { get; set; }
    }
}
