using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using DasMulli.Win32.ServiceUtils;

using Simpbot.Core.Contracts;

namespace Simpbot.Cli.Service
{
    public static class SimpbotServiceBuilder
    {
        public static void RegisterService(string registerFlag, string runFlag)
        {
            var remainingArgs = Environment.GetCommandLineArgs()
                .Where(arg => arg != registerFlag)
                .Select(arg =>
                {
                    arg = Regex.Replace(arg, @"(\\*)" + "\"", @"$1$1\" + "\"");
                    arg = "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
                    return arg;
                })
                .Append(runFlag);

            var host = Process.GetCurrentProcess().MainModule.FileName;

            if (!host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase))
            {
                // For self-contained apps, skip the dll path
                remainingArgs = remainingArgs.Skip(1);
            }

            var fullServiceCommand = host + " " + string.Join(" ", remainingArgs);

            // Do not use LocalSystem in production.. but this is good for demos as LocalSystem will have access to some random git-clone path
            // Note that when the service is already registered and running, it will be reconfigured but not restarted
            var serviceDefinition = new ServiceDefinitionBuilder("Simpbot.Cli.Service")
                .WithDisplayName("Simpbot.Cli.Service")
                .WithDescription("Simple bot built with Discrod.NET")
                .WithBinaryPath(fullServiceCommand)
                .WithCredentials(Win32ServiceCredentials.LocalSystem)
                .WithAutoStart(true)
                .Build();

            new Win32ServiceManager().CreateOrUpdateService(serviceDefinition, true);

            Console.WriteLine($@"Successfully registered and started service Simpbot.Cli");
        }


        public static void RunAsService(ISimpbot simpbot)
        {
            var simpbotService = new Win32SimpbotService(simpbot, "Simpbot.Cli.Service");
            var serviceHost = new Win32ServiceHost(simpbotService);
            serviceHost.Run();
        }
    }
}