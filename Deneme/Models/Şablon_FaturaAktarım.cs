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
    
    public partial class Şablon_FaturaAktarım
    {
        public int ŞablonId { get; set; }
        public int KullanıcıId { get; set; }
        public int CompanyId { get; set; }
        public string ŞablonAdı { get; set; }
        public Nullable<int> BaşlangıçSatırı { get; set; }
        public string CariKodu { get; set; }
        public string CariAdı { get; set; }
        public string DövizTipi { get; set; }
        public string VergiDairesi { get; set; }
        public string TcKimlikNo { get; set; }
        public string VergiNo { get; set; }
        public string KdvHariçTutar { get; set; }
        public string BelgeTarihi { get; set; }
        public string BelgeNumarası { get; set; }
        public string OluşturmaTarihi { get; set; }
    }
}
