using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace MessagingToolkit.Barcode.Service.Demo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (!Environment.UserInteractive)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				    new ScanService() 
			    };
                    ServiceBase.Run(ServicesToRun);
            }
            else
            {
                // Run as interactive exe in debug mode to allow easy
                // debugging.
                ScanService service = new ScanService();
                service.StartService();

                // Sleep the main thread indefinitely while the service code runs
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
