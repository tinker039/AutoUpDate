using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FIleControl
{
    class DocumentHelp
    {
        XDocument xDoc;
        public DocumentHelp()
        {
            xDoc = XDocument.Load("Config.xml");
            XElement element = xDoc.Element("Config").Element("TargetDirectory");
            string[] ff = xDoc.Element("Config").Element("Files").Elements().Select(p => p.Value).ToArray();//.Select(p => p.Name).ToList();
        }
        private static DocumentHelp _instence = new DocumentHelp();
        public static DocumentHelp Instence { get => _instence; }
        public string[] DontMoveFiles { get => xDoc.Element("Config").Element("Files").Elements().Select(p => p.Value).ToArray(); }
        public string DataBasePath { get { return ""; } }
        public string SourceDirectory { get => xDoc.Element("Config").Element("SourceDirectory").Value; }
        public string TargetPath { get => xDoc.Element("Config").Element("TargetDirectory").Value; }
        public string OldDataBasePath { get => TargetPath + "DB File\\sqliteData2.db"; }
        public string NewDataBasePath { get => SourceDirectory + "DB File\\sqliteData2.db"; }
        public string[] DontMoveTabels { get => xDoc.Element("Config").Element("Tables").Elements().Select(p => p.Value).ToArray(); }
    }
}
