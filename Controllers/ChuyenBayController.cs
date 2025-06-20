using Microsoft.AspNetCore.Mvc;

public class ChuyenBayController : Controller
{
    private readonly DataContext _context;

    public ChuyenBayController(DataContext context)
    {
        _context = context;
    }

    // GET: ChuyenBay/Search
    public IActionResult Search()
    {
        return View();
    }

    // POST: ChuyenBay/Details
    [HttpGet]

    public IActionResult Details(string maChuyen)
    {
        var chuyenBay = _context.GetChuyenBay(maChuyen);
        return View(chuyenBay);
    }

    [HttpPost]
    public IActionResult DeleteHK(string mahk, string mach)
    {
        _context.DeleteHK(mahk, mach);
        return RedirectToAction("Details", new { maChuyen = mach });
    }

    [HttpGet]
    public IActionResult UpdateHK(string mahk, string tenhk, string mach)
    {
        ViewBag.mahk = mahk;
        ViewBag.tenhk = tenhk;
        ViewBag.mach = mach;
        return View();
    }
    [HttpPost]
    public IActionResult UpdateHK(string MAHK, string MACH, string SOGHE, bool LOAIGHE)
    {
        _context.UpdateHK(MAHK, MACH, SOGHE, LOAIGHE);
        return RedirectToAction("Details", new { maChuyen = MACH });
    }

    [HttpGet]
    public IActionResult ThemHK(string maChuyen)
    {
        var chuyenBay = _context.GetChuyenBay(maChuyen);

        return View(chuyenBay);
    }

    [HttpPost]
    public IActionResult ThemHK(string MAHK, string MACH, string SOGHE, bool LOAIGHE)
    {
        _context.ThemHK(MAHK, MACH, SOGHE, LOAIGHE);
        return RedirectToAction("Details", new { maChuyen = MACH });
    }

}