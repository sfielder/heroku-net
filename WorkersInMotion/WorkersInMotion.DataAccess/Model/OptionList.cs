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
    
    public partial class OptionList
    {
        public System.Guid OptionListGUID { get; set; }
        public Nullable<System.Guid> OrganizationGUID { get; set; }
        public Nullable<System.Guid> ListGUID { get; set; }
        public Nullable<short> ListType { get; set; }
        public string ListValue { get; set; }
        public string ListURL { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<System.Guid> LastModifiedBy { get; set; }
    }
}
