using Microsoft.AspNetCore.Mvc;

public class HanhKhachController : Controller
{
    private readonly DataContext _context;

    public HanhKhachController(DataContext context)
    {
        _context = context;
    }

    // GET: HanhKhach/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: HanhKhach/Create
    [HttpPost]
    public IActionResult Create(HanhKhach hanhKhach)
    {
        if (ModelState.IsValid)
        {
            _context.AddHanhKhach(hanhKhach);
            return RedirectToAction("Create");
        }
        return View(hanhKhach);
    }
}