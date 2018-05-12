using DasMulli.Win32.ServiceUtils;

using Simpbot.Core.Contracts;

namespace Simpbot.Cli.Service
{
    public class Win32SimpbotService : IWin32Service
    {
        private readonly ISimpbot _simpbotClient;
        private bool _stopRequestedByWindows;


        public Win32SimpbotService(ISimpbot simpbotClient, string serviceName)
        {
            _simpbotClient = simpbotClient;
            ServiceName = serviceName;
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            _simpbotClient.StopCallback = () =>
            {
                if(_stopRequestedByWindows)
                    serviceStoppedCallback.Invoke();
            };
            _simpbotClient.StartAsync().GetAwaiter();
        }
        public void Stop()
        {
            _stopRequestedByWindows = true;
            _simpbotClient.Dispose();
        }

        public string ServiceName { get; }
    }
}
