using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class UploadJobAttachmentRequest
    {
        public string FileName { get; set; }
        public string FileContent { get; set; } // Base64Encoded file Content String
        internal string OrganizationName { get; set; }
        internal Guid JobGUID { get; set; }
        internal string FileType { get; set; }
    }
}