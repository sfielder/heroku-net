//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkersInMotion.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserDevice
    {
        public System.Guid UserDevicesGUID { get; set; }
        public Nullable<System.Guid> UserGUID { get; set; }
        public Nullable<System.Guid> LoginGUID { get; set; }
        public string DeviceID { get; set; }
        public string IPAddress { get; set; }
        public string DeviceInfo { get; set; }
        public Nullable<short> DeviceType { get; set; }
        public string PUSHID { get; set; }
        public string Phone { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<double> TimeZone { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<System.Guid> LastModifiedBy { get; set; }
    }
}
