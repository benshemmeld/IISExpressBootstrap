using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISExpressBootstrap
{
    public class Hosts
    {
        private string _hostsFilePath;

        public Hosts()
        {
            LocalHostNames = new List<string>();

            var systemDir = Environment.SystemDirectory;
            _hostsFilePath = System.IO.Path.Combine(systemDir, "drivers\\etc\\hosts");

            var lines = System.IO.File.ReadAllLines(_hostsFilePath).Where(x => !x.StartsWith("#"));

            foreach (var line in lines)
            {
                var parts = line.Split('\t');
                if (parts[0] == "127.0.0.1")
                {
                    LocalHostNames.Add(parts[1].ToLower());
                }
            }

        }

        public List<string> LocalHostNames { get; set; }

        public void AddLocalHostName(string hostname)
        {
            if (LocalHostNames.Contains(hostname.ToLower()))
            {
                return;
            }

            System.IO.File.AppendAllLines(_hostsFilePath, new List<string> {$"127.0.0.1\t{hostname.ToLower()}"});
        }

    }
}
