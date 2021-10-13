using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deneme.Models
{
    [MetadataType(typeof(CariBilgileriŞablonuMetadata))]
    public partial class Şablon_CariBilgileri
    {
        public Alphabet SelectColumn { get; set; }
        public enum Alphabet
        {
            A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, R, S, T, U, V, Y, Z

        }
    }
    public class CariBilgileriŞablonuMetadata
    {

        [Display(Name = "ŞablonAdı", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string ŞablonAdı { get; set; }

        [Display(Name = "BaşlangıçSatırı", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public int BaşlangıçSatırı { get; set; }

        [Display(Name = "CariKodu", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string CariKodu { get; set; }

        [Display(Name = "CariAdı", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string CariAdı { get; set; }

        [Display(Name = "CurrentType", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string CariTipi { get; set; }

        [Display(Name = "DövizTipi", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string DövizTipi { get; set; }

        [Display(Name = "VergiDairesi", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string VergiDairesi { get; set; }

        [Display(Name = "TcKimlikNo", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string TcKimlikNo { get; set; }

        [Display(Name = "VergiNo", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string VergiNo { get; set; }

        [Display(Name = "Email1", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Email1 { get; set; }

        [Display(Name = "Email2", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Email2 { get; set; }

        [Display(Name = "Cariİl", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Cariİl { get; set; }

        [Display(Name = "Telefon", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Telefon { get; set; }

        [Display(Name = "Adres", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Adres { get; set; }

        [Display(Name = "OluşturmaTarihi", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public System.DateTime OluşturmaTarihi { get; set; }

        public Alphabet SelectColumn { get; set; }
        public enum Alphabet
        {
            A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, R, S, T, U, V, Y, Z

        }
    }
}