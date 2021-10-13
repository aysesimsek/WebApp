using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deneme.Models
{
    public class EvidenceFile
    {
        public EvidenceFile()
        {
            File = new List<HttpPostedFileBase>();
        }

        [Display(Name = "File", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public List<HttpPostedFileBase> File { get; set; }

        public int MutabakatDetayId { get; set; }

        public string DosyaYolu  { get; set; }
    }
}