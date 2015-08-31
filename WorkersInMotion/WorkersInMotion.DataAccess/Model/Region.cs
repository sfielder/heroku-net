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
    
    public partial class Region
    {
        public Region()
        {
            this.Markets = new HashSet<Market>();
            this.Territories = new HashSet<Territory>();
        }
    
        public System.Guid RegionGUID { get; set; }
        public string Name { get; set; }
        public Nullable<System.Guid> OrganizationGUID { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public string Description { get; set; }
        public string REGIONID { get; set; }
    
        public virtual ICollection<Market> Markets { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Region Regions1 { get; set; }
        public virtual Region Region1 { get; set; }
        public virtual ICollection<Territory> Territories { get; set; }
    }
}
