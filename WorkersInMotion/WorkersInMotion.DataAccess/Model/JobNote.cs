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
    
    public partial class JobNote
    {
        public System.Guid JobNoteGUID { get; set; }
        public Nullable<System.Guid> JobGUID { get; set; }
        public Nullable<bool> Deletable { get; set; }
        public Nullable<short> NoteType { get; set; }
        public string NoteText { get; set; }
        public string FileName { get; set; }
        public string FileURL { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
    
        public virtual Job Job { get; set; }
    }
}
