using NancyWebApiCore.Interfaces;

namespace NancyWebApiCore.Helpers
{
    public class AppConfiguration : IAppConfiguration
    {
        public NytSettings NytSettings { get; set; }
    }
}