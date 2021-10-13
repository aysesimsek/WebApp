using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deneme.Models
{
    [MetadataType(typeof(CariMutabakatŞablonuMetadata))]
    public partial class Şablon_CariMutabakat
    {

        public Alphabet SelectColumn { get; set; }
        public enum Alphabet
        {
            A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, R, S, T, U, V, Y, Z

        }

    }
    public class CariMutabakatŞablonuMetadata
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

        [Display(Name = "VergiDairesi", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string VergiDairesi { get; set; }

        [Display(Name = "VergiNo", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string VergiNo { get; set; }

        [Display(Name = "KdvHariçTutar", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string KdvHariçTutar { get; set; }

        [Display(Name = "KdvTutarı", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string KdvTutarı { get; set; }

        [Display(Name = "BelgeTarihi", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BelgeTarihi { get; set; }

        [Display(Name = "BelgeNumarası", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string BelgeNumarası { get; set; }

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