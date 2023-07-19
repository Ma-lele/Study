using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace XHS.Windows.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ServiceRunner>(s =>
                {
                    s.ConstructUsing(name => new ServiceRunner());
                    s.WhenStarted((tc, hc) => tc.Start(hc));
                    s.WhenStopped((tc, hc) => tc.Stop(hc));
                    s.WhenContinued((tc, hc) => tc.Continue(hc));
                    s.WhenPaused((tc, hc) => tc.Pause(hc));
                });

                x.RunAsLocalSystem();
                x.StartAutomatically();

                x.SetDescription("新合盛程序启动服务");
                x.SetDisplayName("XHS.Windows.Server");
                x.SetServiceName("XHS.Windows.Server");

                x.EnableShutdown();
            });
        }
    }
}
