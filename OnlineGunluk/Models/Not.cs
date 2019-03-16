using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineGunluk.Models
{
    [Table("Notlar")]
    public class Not
    {
        public int ID { get; set; }

        public string ApplicationUserID { get; set; }

        public string Baslik { get; set; }

        public string Icerik { get; set; }

        public DateTime? EklenmeTarihi { get; set; }

        public DateTime? SonGuncellenmeTarihi { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}