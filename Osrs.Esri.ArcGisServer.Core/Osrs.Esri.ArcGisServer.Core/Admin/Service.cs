using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Esri.ArcGisServer.Core.Admin
{
    public sealed class Service
    {
        private readonly AgsServer server;
        private readonly Services services;
        public string Name { get; private set; }
        public ServiceType ServiceType { get; private set; }
        public string Description { get; private set; }
        public string Capabilities { get; private set; }
        public string ClusterName { get; private set; }
        public int MinInstancesPerNode { get; private set; }
        public int MaxInstancesPerNode { get; private set; }
        public int MaxWaitTime { get; private set; }
        public int MaxIdleTime { get; private set; }
        public int MaxUsageTime { get; private set; }
        public int RecycleInterval { get; private set; }
        public LoadBalancingMethod LoadBalancing { get; private set; }
        public IsolationLevel IsolationLevel { get; private set; }
        public Dictionary<string, string> Properties { get; private set; }
        public List<ServiceExtension> Extensions { get; private set; }

        public string Start()
        {
            try
            {
                return this.server.GetClient().UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/services/{0}/start", AgsServer.ConcatFolderService(this.services.FolderName,this.Name))), "");
            }
            catch
            { }
            return null;
        }

        public string Stop()
        {
            try
            {
                return this.server.GetClient().UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/services/{0}/stop", AgsServer.ConcatFolderService(this.services.FolderName, this.Name))), "");
            }
            catch
            { }
            return null;
        }

        public string Edit()
        {
            return this.Edit(false);
        }

        public string Edit(bool runAsync)
        {
            try
            {
                WebClient wc = this.server.GetClient();
                JObject o = new JObject();
                o["description"] = this.Description;
                o["capabilities"] = this.Capabilities;
                o["clusterName"] = this.ClusterName;
                o["minInstancePerNode"] = this.MinInstancesPerNode;
                o["maxInstancesPerNode"] = this.MaxInstancesPerNode;
                o["maxWaitTime"] = this.MaxWaitTime;
                o["maxIdleTime"] = this.MaxIdleTime;
                o["maxUsageTime"] = this.MaxUsageTime;
                o["recycleInterval"] = this.RecycleInterval;
                o["loadBalancing"] = AgsAdminClient.Format(this.LoadBalancing);
                o["isolationLevel"] = AgsAdminClient.Format(this.IsolationLevel);
                o["properties"] = AgsAdminClient.Format(this.Properties);
                o["extensions"] = AgsAdminClient.Format(this.Extensions);

                return wc.UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/services/{0}/edit", AgsServer.ConcatFolderService(this.services.FolderName, this.Name))), "service="+o.ToString(Newtonsoft.Json.Formatting.None));
            }
            catch
            { }
            return null;
        }

        public string Delete()
        {
            try
            {
                return this.server.GetClient().UploadString(AgsAdminClient.FormatUrl(this.server, string.Format("/services/{0}/delete", AgsServer.ConcatFolderService(this.services.FolderName, this.Name))), "");
            }
            catch
            { }
            return null;
        }

        public void ItemInformation()
        { }

        public void Statistics()
        { }

        public void Types()
        { }

        public void Permissions()
        { }

        //public void Extensions()
        //{ }
    }

    public sealed class ServiceExtension
    {
        public string TypeName { get; private set; }
        public bool Enabled { get; private set; }
        public string Capabilities { get; private set; }
        public Dictionary<string, string> Properties { get; private set; }

        internal JObject Format()
        {
            JObject o = new JObject();
            o.Add("typeName", this.TypeName);
            o.Add("enabled", this.Enabled);
            o.Add("capabilities", this.Capabilities);
            o.Add("properties", AgsAdminClient.Format(this.Properties));
            return o;
        }
    }
}
