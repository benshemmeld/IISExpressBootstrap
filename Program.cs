using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace IISExpressBootstrap
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
                throw new Exception(@"2 arguments are required:
(1) Name of the site (matching site node in applicationhost.config)
(2) Relative path for the site (e.g. \Web");
            var siteName = args[0];

            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            //currentDir = @"C:\git\ChristianLifeAndMinistrySchedule"; //testing

            var physicalPath = Path.Combine(currentDir, args[1]);
            var applicationHostFilename = System.IO.Path.Combine(currentDir, "applicationhost.config");
            if (!File.Exists(applicationHostFilename))
                throw new Exception("applicationhost.config must exist in the current directory");

            var xml = XElement.Load(applicationHostFilename);

            var siteNode = xml.Descendants("site").FirstOrDefault(x => x.Attribute("name").Value == siteName);
            var virtualDirectoryNode = siteNode.Descendants("virtualDirectory").FirstOrDefault();
            virtualDirectoryNode.Attribute("physicalPath").SetValue(physicalPath);

            var bindingNode = siteNode.Descendants("binding").FirstOrDefault();
            var bindingInfo = bindingNode.Attribute("bindingInformation").Value;
            var match = Regex.Match(bindingInfo, "\\*\\:(\\d*)\\:(.*)");
            var port = match.Groups[1].ToString();
            var host = match.Groups[2].ToString();
                

            xml.Save(applicationHostFilename);

            var iisExpressPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "iis express\\iisexpress.exe");
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                FileName = iisExpressPath,
                Arguments = "/config:applicationhost.config",
            };
            var iisExpressProcess = Process.Start(startInfo);

            //Launch chrome
            Process.Start("chrome.exe", $"http://{host}:{port}");

            iisExpressProcess.WaitForExit();

        }
    }
}
