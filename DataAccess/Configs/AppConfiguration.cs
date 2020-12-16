namespace DataAccess.Configs
{
    public sealed class AppConfiguration : IAppConfiguration
    {
        public NytSettings NytSettings { get; set; }
    }
}