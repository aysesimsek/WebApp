using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deneme.Models
{
    [MetadataType(typeof(BaBsMutabakatBilgileriMetaData))]
    public partial class BaBsMutabakatBilgileri
    {
        public Months SelectMonths { get; set; }
        public enum Months
        {
            Ocak, Şubat, Mart, Nisan, Mayıs, Hazirazn, Temmuz, Ağustos, Eylül, Ekim, Kasım, Aralık

        }
        public Tip SelectType { get; set; }
        public enum Tip
        {
            Alış, Satış

        }

    }

    public class BaBsMutabakatBilgileriMetaData
    {
        
        [Display(Name = "Ay", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Ay { get; set; }

        [Display(Name = "Yıl", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Yıl { get; set; }

        [Display(Name = "FaturaTipi", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string FaturaTipi { get; set; }


        public Months SelectMonths { get; set; }
        public enum Months
        {
            Ocak, Şubat, Mart, Nisan, Mayıs, Hazirazn, Temmuz, Ağustos, Eylül, Ekim, Kasım, Aralık

        }
        public Tip SelectType { get; set; }
        public enum Tip
        {
            Alış, Satış

        }

    }
}