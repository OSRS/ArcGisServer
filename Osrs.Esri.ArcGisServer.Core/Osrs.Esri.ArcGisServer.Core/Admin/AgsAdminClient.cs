using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace Osrs.Esri.ArcGisServer.Core.Admin
{
    public enum LoadBalancingMethod
    {
        Unknown,
        RoundRobin,
        FailOver
    }

    public enum IsolationLevel
    {
        Unknown,
        Low,
        High
    }

    public sealed class AgsAdminClient
    {
        internal const string urlBase = "admin";
        private readonly AgsServer server;

        internal AgsAdminClient(AgsServer server)
        {
            this.server = server;
        }

        public bool ClearRestCache()
        {
            return ClearRestCache(null);
        }

        public bool ClearRestCache(string folder)
        {
            WebClient wc = this.server.GetClient();
            if (!string.IsNullOrEmpty(folder))
                wc.QueryString["folderName"] = folder;
            JToken result = AgsServer.Read(wc, FormatUrl(this.server, "/system/handlers/rest/cache/clear"), "");
            if (result!=null && result is JObject)
            {
                JObject o = result as JObject;
                if (o != null)
                {
                    try
                    {
                        return (bool)o["success"]; //well, that's wonky
                    }
                    catch
                    { }
                }
            }
            return false;
        }

        public bool ClearRestCache(string folder, string serviceName, ServiceType typ)
        {
            WebClient wc = this.server.GetClient();
            if (!string.IsNullOrEmpty(folder))
                wc.QueryString["folderName"] = folder;
            if (!string.IsNullOrEmpty(serviceName) && typ!= ServiceType.Unknown)
            {
                wc.QueryString["serviceName"] = serviceName;
                wc.QueryString["type"] = AgsServer.EncodeServiceType(typ);
            }
            JToken result = AgsServer.Read(wc, FormatUrl(this.server, "/system/handlers/rest/cache/clear"), "");
            if (result != null && result is JObject)
            {
                JObject o = result as JObject;
                if (o != null)
                {
                    try
                    {
                        return (bool)o["success"]; //well, that's wonky
                    }
                    catch
                    { }
                }
            }
            return false;
        }

        internal static string FormatUrl(AgsServer server)
        {
            return string.Format("http://{0}:{1}/{2}/{3}", server.Server, server.Port, server.Instance, urlBase);
        }

        internal static string FormatUrl(AgsServer server, string url)
        {
            return string.Format("http://{0}:{1}/{2}/{3}{4}", server.Server, server.Port, server.Instance, urlBase, url);
        }

        internal static string Format(LoadBalancingMethod method)
        {
            if (method == LoadBalancingMethod.FailOver)
                return "FAIL_OVER";
            else if (method == LoadBalancingMethod.RoundRobin)
                return "ROUND_ROBIN";
            return "ROUND_ROBIN";
        }

        internal static string Format(IsolationLevel level)
        {
            if (level == IsolationLevel.High)
                return "HIGH";
            else if (level == IsolationLevel.Low)
                return "LOW";
            return "HIGH";
        }

        internal static JObject Format(Dictionary<string, string> kvps)
        {
            JObject o = new JObject();
            if (kvps != null)
            {
                foreach (KeyValuePair<string, string> cur in kvps)
                {
                    o.Add(cur.Key, cur.Value);
                }
            }
            return o;
        }

        internal static JArray Format(IEnumerable<ServiceExtension> items)
        {
            JArray a = new JArray();
            if (items!=null)
            {
                foreach(ServiceExtension ext in items)
                {
                    a.Add(ext.Format());
                }
            }
            return a;
        }
    }
}
