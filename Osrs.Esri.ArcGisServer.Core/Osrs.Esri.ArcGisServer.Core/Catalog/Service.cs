using Newtonsoft.Json.Linq;

namespace Osrs.Esri.ArcGisServer.Core.Catalog
{
    public sealed class Service
    {
        private readonly AgsServer server;
        public string Name { get; private set; }
        public ServiceType ServiceType { get; private set; }

        private Service(AgsServer server)
        {
            this.server = server;
        }

        internal static Service Create(AgsServer server, JToken token)
        {
            if (token!=null && token is JObject)
            {
                JObject o = token as JObject;
                if (o!=null)
                {
                    Service s = new Service(server);
                    try
                    {
                        s.Name = (string)o["name"];
                        s.ServiceType = AgsServer.DecodeServiceType((string)o["type"]);
                        return s;
                    }
                    catch
                    { }
                }
            }
            return null;
        }
    }
}
