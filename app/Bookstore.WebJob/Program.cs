using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Bookstore.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        static void Main()
        {
            var configuration = new JobHostConfiguration();
            configuration.NameResolver = new AppSettingsNameResolver();

            JobHost host = new JobHost(configuration);
            host.RunAndBlock();
        }
    }
}
