using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deneme.Models
{
    [MetadataType(typeof(YetkiMetadata))]
    public partial class Yetki
    {
        public Boolean BaOluştur { get; set; }
        public Boolean BaDüzenle { get; set; }
        public Boolean BaMailGönder { get; set; }
        public Boolean BaSil { get; set; }
        public Boolean BaSmsGönder { get; set; }

        public Boolean BsOluştur { get; set; }
        public Boolean BsDüzenle { get; set; }
        public Boolean BsMailGönder { get; set; }
        public Boolean BsSil { get; set; }
        public Boolean BsSmsGönder { get; set; }

        public Boolean CariOluştur { get; set; }
        public Boolean CariDüzenle { get; set; }
        public Boolean CariMailGönder { get; set; }
        public Boolean CariSil { get; set; }
        public Boolean CariSmsGönder { get; set; }
    }

    public class YetkiMetadata
    {

        public Boolean BaOluştur { get; set; }
        public Boolean BaDüzenle { get; set; }
        public Boolean BaMailGönder { get; set; }
        public Boolean BaSil { get; set; }
        public Boolean BaSmsGönder { get; set; }

        public Boolean BsOluştur { get; set; }
        public Boolean BsDüzenle { get; set; }
        public Boolean BsMailGönder { get; set; }
        public Boolean BsSil { get; set; }
        public Boolean BsSmsGönder { get; set; }

        public Boolean CariOluştur { get; set; }
        public Boolean CariDüzenle { get; set; }
        public Boolean CariMailGönder { get; set; }
        public Boolean CariSil { get; set; }
        public Boolean CariSmsGönder { get; set; }


    }
}