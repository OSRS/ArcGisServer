using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace Osrs.Esri.ArcGisServer.Core.Admin
{
    public sealed class Machines
    {
        private readonly AgsServer server;

        private Machine[] machines;
        public IEnumerable<Machine> Items
        {
            get { return this.machines; }
        }

        private Machines(AgsServer server)
        {
            this.server = server;
        }

        public string Register(string machineName, string adminUrl)
        {
            if (!string.IsNullOrEmpty(machineName))
            {
                string sServiceURI = string.Format("http://{0}:{1}/{2}/admin/machines/register", server.Server, server.Port, server.Instance);
                WebClient wc = server.GetClient();
                wc.QueryString["machineName"] = machineName;
                if (!string.IsNullOrEmpty(adminUrl))
                    wc.QueryString["adminURL"] = adminUrl;
                string result = wc.UploadString(sServiceURI, "");
                this.Reload();
                return result;
            }

            return null;
        }

        public string Rename(string machineName, string newMachineName)
        {
            if (!string.IsNullOrEmpty(machineName) && !string.IsNullOrEmpty(newMachineName))
            {
                string sServiceURI = string.Format("http://{0}:{1}/{2}/admin/machines/rename", server.Server, server.Port, server.Instance);
                WebClient wc = server.GetClient();
                wc.QueryString["machineName"] = machineName;
                wc.QueryString["newMachineName"] = newMachineName;
                string result = wc.UploadString(sServiceURI, "");
                this.Reload();
                return result;
            }

            return null;
        }

        internal static Machines Create(AgsServer server)
        {
            Machines m = new Machines(server);
            if (m.Reload())
                return m;
            return null;
        }

        private bool Reload()
        {
            JToken token = AgsServer.Read(server.GetClient(), AgsAdminClient.FormatUrl(server, "/machines"), "");
            if (token != null && token is JObject)
            {
                try
                {
                    JObject o = token as JObject;
                    JArray a = (JArray)o["machines"];
                    this.machines = new Machine[a.Count];

                    for(int i=0;i<this.machines.Length;i++)
                    {
                        this.machines[i] = Machine.Create(this.server, a[i]);
                    }

                    return true;
                }
                catch
                { this.machines = null; } //don't care why
            }

            return false;
        }
    }
}
