using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osrs.Esri.ArcGisServer.Core.Admin
{
    public sealed class Services
    {
        public string FolderName { get; private set; }
        public string Description { get; private set; }
        public bool WebEncrypted { get; private set; }
        public bool IsDefault { get; private set; }
        public string[] folders { get; private set; }
        public Services[] services { get; private set; }
    }
}
