//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Deneme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Yetki
    {
        public int YetkiID { get; set; }
        public int UserID { get; set; }
        public int CompanyId { get; set; }
        public Nullable<bool> isDefault { get; set; }
        public string Yetkiler { get; set; }
        public string CompanyName { get; set; }
    }
}
