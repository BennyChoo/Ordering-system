using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using QFD.Models;
using Syncfusion.EJ2;

namespace QFD.Controllers
{
    public class DataManager
    {
        public bool requiresCounts { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
        public List<Wheres> where { get; set; }
    }

    public class Wheres
    {
        public string field { get; set; }
        public bool ignoreCase { get; set; }

        public bool isComplex { get; set; }

        public string value { get; set; }
        public string Operator { get; set; }
    }

    public class LookupController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<LookupController> _logger;

        public LookupController(ApplicationDbContext db, ILogger<LookupController> logger)
        {
            _db = db;
            _logger = logger;
        }


        [HttpPost]
        public ActionResult ProductCategory([FromBody] DataManager dm)
        {
            var data = (from c in _db.ProductCategory
                        select new
                        {
                            c.ProductCategoryId,
                            c.Category
                        }).ToList();

            if (dm.where != null && dm.where.Count == 1) //Filtering
                if (dm.where[0].value != null)
                {
                    var filterData = data.SingleOrDefault(s => s.Category == dm.where[0].value);
                    return dm.requiresCounts ? Json(new { result = filterData, count = 1 }) : Json(filterData);
                }

            return dm.requiresCounts ? Json(new { result = data, count = data.Count }) : Json(data);
        }
    }
}
