using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace Osrs.Esri.ArcGisServer.Core.Admin
{
    public sealed class Machine
    {
        private readonly AgsServer server;
        public string Name { get; private set; }
        public string AdminUrl { get; private set; }
        public string Synchronize { get; private set; }

        private Machine(AgsServer server)
        {
            this.server = server;
        }

        public string Unregister()
        {
            try
            {
                return this.server.GetClient().UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/machines/{0}/unregister", this.Name)), "");
            }
            catch
            { }
            return null;
        }

        public string Start()
        {
            try
            {
                return this.server.GetClient().UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/machines/{0}/start", this.Name)), "");
            }
            catch
            { }
            return null;
        }

        public string Stop()
        {
            try
            {
                return this.server.GetClient().UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/machines/{0}/stop", this.Name)), "");
            }
            catch
            { }
            return null;
        }

        public MachineInfo Info()
        {
            try
            {
                return MachineInfo.Create(this.server, this.server.GetClient().UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/machines/{0}", this.Name)), ""));
            }
            catch
            { }
            return null;
        }

        public MachineStatus Status()
        {
            try
            {
                return MachineStatus.Create(this.server.GetClient().UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/machines/{0}/status", this.Name)), ""));
            }
            catch
            { }
            return null;
        }

        internal static Machine Create(AgsServer server, JToken token)
        {
            if (token!=null && token is JObject)
            {
                JObject o = (JObject)token;

                Machine m = new Machine(server);
                token = o["machineName"];
                if (token != null)
                    m.Name = (string)token;
                token = o["adminURL"];
                if (token != null)
                    m.AdminUrl = (string)token;
                token = o["synchronize"];
                if (token != null)
                    m.Synchronize = (string)token;
            }
            return null;
        }
    }

    public sealed class MachineStatus
    {
        public string ConfiguredState { get; internal set; }
        public string RealTimeState { get; internal set; }

        private MachineStatus(string configuredState, string realTimeState)
        {
            this.ConfiguredState = configuredState;
            this.RealTimeState = realTimeState;
        }

        internal static MachineStatus Create(JToken token)
        {
            if (token != null && token is JObject)
            {
                JObject o = (JObject)token;
                MachineStatus info = new MachineStatus((string)o["configuredState"], (string)o["realTimeState"]);
                return info;
            }
            return null;
        }
    }

    public sealed class MachineInfo
    {
        private readonly AgsServer server;
        public string Name { get; private set; }
        public string Platform { get; private set; }
        public string AdminUrl { get; set; }
        public Dictionary<string, uint> Ports { get; private set; }

        private MachineInfo(AgsServer server)
        {
            this.server = server;
        }

        public string Edit()
        {
            try
            {
                WebClient wc = this.server.GetClient();
                if (this.Ports != null)
                {
                    foreach (KeyValuePair<string, uint> cur in this.Ports)
                    {
                        wc.QueryString.Add(cur.Key, cur.Value.ToString());
                    }
                }
                wc.QueryString["adminURL"] = this.AdminUrl;
                return wc.UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/machines/{0}/edit", this.Name)), "");
            }
            catch
            { }
            return null;
        }


        internal static MachineInfo Create(AgsServer server, JToken token)
        {
            if (token!=null && token is JObject)
            {
                JObject o = (JObject)token;
                MachineInfo info = new MachineInfo(server);
                info.Name = (string)o["machineName"];
                info.Platform = (string)o["platform"];
                o = (JObject)o["ports"];
                info.Ports = new Dictionary<string, uint>();
                foreach (KeyValuePair<string, JToken> cur in o)
                {
                    info.Ports[cur.Key] = (ushort)cur.Value;
                }
                return info;
            }
            return null;
        }
    }
}
