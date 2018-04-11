using Newtonsoft.Json.Linq;
using Osrs.Esri.ArcGisServer.Core.Catalog;
using System.Net;

namespace Osrs.Esri.ArcGisServer.Core
{
    public sealed class AgsServer
    {
        private readonly string server;
        public string Server { get { return this.server; } }

        private readonly string instance;
        public string Instance { get { return this.instance; } }

        private readonly ushort port;
        public ushort Port { get { return this.port; } }

        private string user;
        public string User
        {
            get { return this.user; }
            set
            {
                this.user = value;
                this.Token = null;
            }
        }

        private string password;
        public string Password
        {
            get { return this.user; }
            set
            {
                this.password = value;
                this.Token = null;
            }
        }

        public string Token { get; private set; }

        public AgsRestClient Client
        {
            get
            {
                return new AgsRestClient(this);
            }
        }

        public AgsServer(string server) : this(server, 6080, "arcgis", null, null)
        { }

        public AgsServer(string server, ushort port) : this(server, port, "arcgis", null, null)
        { }

        public AgsServer(string server, ushort port, string instance) : this(server, port, instance, null, null)
        { }

        public AgsServer(string server, string user, string pwd) : this(server, 6080, "arcgis", user, pwd)
        { }

        public AgsServer(string server, ushort port, string user, string pwd) : this(server, port, "arcgis", user, pwd)
        { }

        public AgsServer(string server, ushort port, string instance, string user, string pwd)
        {
            this.server = server;
            this.instance = instance;
            this.port = port;
            this.User = user;
            this.Password = pwd;
        }

        private void GetToken()
        {
            if (!(string.IsNullOrEmpty(this.User) && string.IsNullOrEmpty(this.Password)))
            {
                WebClient wc = GetClient(false, false);
                try
                {
                    string sTokenURI = string.Format("http://{0}:{1}/{2}/admin/generateToken", this.Server, this.Port, this.Instance);
                    this.Token = wc.UploadString(sTokenURI, "POST", "username=" + this.User + " &password=" + this.Password + " &client=requestip");
                }
                catch
                {
                    this.Token = null;
                }
            }
        }

        internal static string ConcatFolderService(string folder, string service)
        {
            if (!string.IsNullOrEmpty(folder))
            {
                if (folder != "/") //else we just return service anyway
                    return folder + "/" + service;
            }
            return service;
        }

        internal static ServiceType DecodeServiceType(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.ToLowerInvariant();
                switch(value)
                {
                    case "mapserver":
                        return ServiceType.Map;
                    case "imageserver":
                        return ServiceType.Image;
                    case "featureserver":
                        return ServiceType.Feature;
                    case "geocodeserver":
                        return ServiceType.Geocode;
                    case "geodataserver":
                        return ServiceType.GeoData;
                    case "geometryserver":
                        return ServiceType.Geometry;
                    case "globeserver":
                        return ServiceType.Globe;
                    case "gpserver":
                        return ServiceType.GP;
                    case "mobileserver":
                        return ServiceType.Mobile;
                    case "networkserver":
                        return ServiceType.Network;
                    case "sceneserver":
                        return ServiceType.Scene;
                    case "streamserver":
                        return ServiceType.Stream;
                }
            }
            return ServiceType.Unknown;
        }

        internal static string EncodeServiceType(ServiceType typ)
        {
            switch(typ)
            {
                case ServiceType.Feature:
                    return "FeatureServer";
                case ServiceType.Geocode:
                    return "GeocodeServer";
                case ServiceType.GeoData:
                    return "GeoDataServer";
                case ServiceType.Geometry:
                    return "GeometryServer";
                case ServiceType.Globe:
                    return "GlobeServer";
                case ServiceType.GP:
                    return "GPServer";
                case ServiceType.Image:
                    return "ImageServer";
                case ServiceType.Map:
                    return "MapServer";
                case ServiceType.Mobile:
                    return "MobileServer";
                case ServiceType.Network:
                    return "NetworkServer";
                case ServiceType.Scene:
                    return "SceneServer";
                case ServiceType.Stream:
                    return "StreamServer";
            }
            return string.Empty;
        }

        internal WebClient GetClient()
        {
            return this.GetClient(true, true);
        }

        internal WebClient GetClient(bool json, bool token)
        {
            WebClient wc = new WebClient();
            //common headers, every time
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.Headers[HttpRequestHeader.Accept] = "text/plain";
            if (json)
                wc.QueryString["f"] = "json"; //we're doing json, so it's kinda always there too
            if (token)
            {
                if (string.IsNullOrEmpty(this.Token))
                    GetToken();
                wc.QueryString["token"] = this.Token;
            }
            return wc;
        }

        internal static JToken Read(WebClient wc, string sServiceURI, string arg)
        {
            try
            {
                string sResults = wc.UploadString(sServiceURI, arg);
                return JRaw.Parse(sResults);
            }
            catch
            { }
            return null;
        }
    }
}
