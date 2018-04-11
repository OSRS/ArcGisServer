using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Osrs.Esri.ArcGisServer.Core.Catalog
{
    public sealed class Services
    {
        private readonly AgsServer server;
        public string CurrentVersion { get; private set; }

        private string[] folders;
        private Services[] serviceFolders;
        public IEnumerable<Services> Folders()
        {
            if (this.folders!=null && this.serviceFolders==null)
            {
                this.serviceFolders = new Services[this.folders.Length];
                for(int i=0;i<this.folders.Length;i++)
                {
                    this.serviceFolders[i] = Services.Create(server, "/" + this.folders[i]);
                }
            }
            return this.serviceFolders;
        }

        private Service[] items;
        public IEnumerable<Service> Items { get { return this.items; } }

        private Services(AgsServer server)
        {
            this.server = server;
        }

        internal static Services Create(AgsServer server, string url)
        {
            JToken token = AgsServer.Read(server.GetClient(), AgsRestClient.FormatUrl(server) + url, "");

            if (token != null && token is JObject)
            {
                JObject o = token as JObject;
                if (o != null)
                {
                    Services s = new Services(server);
                    try
                    {
                        s.CurrentVersion = (string)o["currentVersion"];
                        JArray a = o["folders"] as JArray;
                        if (a != null)
                        {
                            s.folders = new string[a.Count];
                            for (int i = 0; i < s.folders.Length; i++)
                            {
                                s.folders[i] = (string)a[i];
                            }
                        }
                        a = o["services"] as JArray;
                        if (a != null)
                        {
                            s.items = new Service[a.Count];
                            for (int i = 0; i < s.items.Length; i++)
                            {
                                s.items[i] = Service.Create(server, a[i]);
                            }
                        }
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
