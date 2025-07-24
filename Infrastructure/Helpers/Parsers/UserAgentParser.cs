using UAParser;

namespace Infrastructure.Helpers.Parser
{
    public static class UserAgentParser
    {
        private static readonly UAParser.Parser _parser = UAParser.Parser.GetDefault();


        public static (string Browser, string OS, string Device) Parse(string userAgent)
        {
            var client = _parser.Parse(userAgent);

            var browser = client.UA.Family;    // e.g. Chrome
            var os = client.OS.Family;         // e.g. Windows
            var device = client.Device.Family; // e.g. Other

            return (browser, os, device);
        }
    }
}
