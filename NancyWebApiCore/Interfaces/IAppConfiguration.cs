using NancyWebApiCore.Helpers;

namespace NancyWebApiCore.Interfaces
{
    public interface IAppConfiguration
    {
        NytSettings NytSettings { get; set; }
    }
}