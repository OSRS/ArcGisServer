namespace Osrs.Esri.ArcGisServer.Core.Catalog
{
    public sealed class AgsRestClient
    {
        internal const string urlBase = "rest/services";
        private readonly AgsServer server;

        internal AgsRestClient(AgsServer server)
        {
            this.server = server;
        }

        public Services Services()
        {
            return Catalog.Services.Create(server, "/");
        }

        public void ServiceFootprints()
        {
        }

        internal static string FormatUrl(AgsServer server)
        {
            return string.Format("http://{0}:{1}/{2}/{3}", server.Server, server.Port, server.Instance, urlBase);
        }

        internal static string FormatUrl(AgsServer server, string url)
        {
            return string.Format("http://{0}:{1}/{2}/{3}{4}", server.Server, server.Port, server.Instance, urlBase, url);
        }
    }
}
