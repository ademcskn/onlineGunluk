using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using OnlineGunluk.Models;
namespace OnlineGunluk.Controllers
{
    //Tokensız işlem yaptırmaması için...
    [Authorize]
    public class MainController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public IEnumerable<NotBaslikDto> GetBasliklar()
        {
            string loggedInUserID = User.Identity.GetUserId();

            return db.Notlar.Where(x => x.ApplicationUserID == loggedInUserID).Select(x => new NotBaslikDto { ID = x.ID, Baslik = x.Baslik }).ToList();
        }

        public IHttpActionResult GetNot(int id)
        {
            string loggedInUserID = User.Identity.GetUserId();
            Not not = db.Notlar.FirstOrDefault(x => x.ApplicationUserID == loggedInUserID && x.ID == id);

            if (not == null)
                return NotFound();

            return Ok(not);
        }

        [HttpPost]
        public HttpResponseMessage Save(NotKaydetDto not)
        {
            string loggedInUserID = User.Identity.GetUserId();
            NotIslemBilgiDto mesaj = new NotIslemBilgiDto();
            if (not.ID == 0) //yeni kayıt
            {
                Not n = new Not();
                n.ApplicationUserID = loggedInUserID;
                n.EklenmeTarihi = DateTime.Now;
                n.Baslik = not.Baslik;
                n.Icerik = not.Icerik;

                db.Notlar.Add(n);
                db.SaveChanges();

                mesaj.ID = n.ID;
                mesaj.Mesaj = "Eklendi (" + n.EklenmeTarihi.Value.ToLongTimeString() + ")";

            }
            else //güncelleme
            {
                Not n = db.Notlar.FirstOrDefault(x => x.ApplicationUserID == loggedInUserID && x.ID == not.ID);

                if (n == null)
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bir hata oluştu!");

                n.Baslik = not.Baslik;
                n.Icerik = not.Icerik;
                n.SonGuncellenmeTarihi = DateTime.Now;

                db.SaveChanges();

                mesaj.ID = n.ID;
                mesaj.Mesaj = "Güncellendi(" + n.SonGuncellenmeTarihi.Value.ToLongDateString() + ")";
            }
            return Request.CreateResponse(HttpStatusCode.OK, mesaj);
        }

        [HttpPost]
        public HttpResponseMessage Delete(int id)
        {
            string loggedInUserID = User.Identity.GetUserId();
            Not not = db.Notlar.FirstOrDefault(x => x.ApplicationUserID == loggedInUserID && x.ID == id);
            string baslik = not.Baslik;

            db.Notlar.Remove(not);
            db.SaveChanges();

            NotIslemBilgiDto mesaj = new NotIslemBilgiDto()
            {
                ID = id,
                Mesaj = baslik + "baslikli not silindi."
            };
            return Request.CreateResponse(HttpStatusCode.OK, mesaj);
        }
    }
}
