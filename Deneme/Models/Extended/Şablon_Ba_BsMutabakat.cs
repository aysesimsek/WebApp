using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deneme.Models
{
    [MetadataType(typeof(Ba_BsMutabakatŞablonuMetadata))]
    public partial class Şablon_Ba_BsMutabakat
    {
        public Alphabet SelectColumn { get; set; }
        public enum Alphabet
        {
            A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, R, S, T, U, V, Y, Z

        }

    }
    public class Ba_BsMutabakatŞablonuMetadata
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


        [Display(Name = "TcKimlikNo", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string TcKimlikNo { get; set; }

        [Display(Name = "VergiNo", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string VergiNo { get; set; }

        [Display(Name = "KdvHariçTutar", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string KdvHariçTutar { get; set; }

        [Display(Name = "BelgeSayısı", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BelgeSayısı { get; set; }

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