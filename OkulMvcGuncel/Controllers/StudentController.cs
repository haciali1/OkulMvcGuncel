using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sube2.HelloMvc.Models;

namespace Sube2.HelloMvc.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            using (var ctx = new OkulDbContext())
            {
                var lst = ctx.Ogrenciler.ToList();
                return View(lst);
            }
        }

        [HttpGet]
        public IActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddStudent(Ogrenci ogr)
        {
            if (ogr != null)
            {
                using (var ctx = new OkulDbContext())
                {
                    ctx.Ogrenciler.Add(ogr);
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult EditStudent(int id)
        {
            using (var ctx = new OkulDbContext())
            {
                var ogr = ctx.Ogrenciler.Find(id);
                return View(ogr);
            }
        }

        [HttpPost]
        public IActionResult EditStudent(Ogrenci ogr)
        {
            if (ogr != null)
            {
                using (var ctx = new OkulDbContext())
                {
                    ctx.Entry(ogr).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteStudent(int id)
        {
            using (var ctx = new OkulDbContext())
            {
                var ogrenci = ctx.Ogrenciler.Find(id);
                if (ogrenci != null)
                {
                    ctx.Ogrenciler.Remove(ogrenci);
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult GetByIdOgrenciDers(int id)
        {
            using (var ctx = new OkulDbContext())
            {
                var ogrenciDersler = ctx.OgrenciDersler
                    .Include(od => od.Ders)
                    .Where(od => od.OgrenciId == id)
                    .ToList();

                ViewBag.OgrenciId = id;
                ViewBag.AllDersler = ctx.Dersler.ToList();
                return View(ogrenciDersler);
            }
        }

        [HttpPost]
        public IActionResult AddDersToOgrenci(int ogrenciId, int[] dersIds)
        {
            using (var ctx = new OkulDbContext())
            {
                foreach (var dersId in dersIds)
                {
                    if (!ctx.OgrenciDersler.Any(od => od.OgrenciId == ogrenciId && od.DersId == dersId))
                    {
                        ctx.OgrenciDersler.Add(new OgrenciDers
                        {
                            OgrenciId = ogrenciId,
                            DersId = dersId
                        });
                    }
                }
                ctx.SaveChanges();
            }

            return RedirectToAction("GetByIdOgrenciDers", new { id = ogrenciId });
        }

        [HttpPost]
        public IActionResult RemoveDersFromOgrenci(int ogrenciId, int dersId)
        {
            using (var ctx = new OkulDbContext())
            {
                var ogrenciDers = ctx.OgrenciDersler
                    .FirstOrDefault(od => od.OgrenciId == ogrenciId && od.DersId == dersId);

                if (ogrenciDers != null)
                {
                    ctx.OgrenciDersler.Remove(ogrenciDers);
                    ctx.SaveChanges();
                }
            }

            return RedirectToAction("GetByIdOgrenciDers", new { id = ogrenciId });
        }
    }
}
