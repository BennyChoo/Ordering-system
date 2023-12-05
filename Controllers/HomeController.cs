using Microsoft.AspNetCore.Mvc;
using QFD.Logic;
using QFD.Models;
using System.Diagnostics;

namespace QFD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewData = new List<TableListView>();
            var repo = new TableRepository(_db, _logger);

            var dataResult = await repo.GetTableList();

            if (!dataResult.OnError && dataResult.TableList.Count > 0)
            {
                foreach(var table in dataResult.TableList)
                {
                    viewData.Add(new TableListView
                    {
                        Location = table.Location,
                        Description = table.Description,
                        TableNumber = table.TableNumber,
                        TotalPerson = table.TotalPerson,
                        IsActive = table.IsActive,
                        ProtectedTableId = table.TableId.ToString()
                    });
                }
            }

            var qrData = new List<TableQrCodeListView>();

            var qrResult = await repo.GetTableQrList();

            if (!qrResult.OnError && qrResult.TableQrCodeList.Count > 0)
            {
                foreach (var qr in qrResult.TableQrCodeList)
                {
                    var formatDate = qr.ExpiryDate.ToString("dd/MM/yyyy h");

                    if (qr.ExpiryDate.ToString("tt") == "AM")
                    {
                        formatDate += "a.m.";
                    }
                    else
                    {
                        formatDate += "p.m.";
                    }

                    qrData.Add(new TableQrCodeListView
                    {
                        qrId = qr.QrId,
                        qrCode = qr.QrCode,
                        tableNumber = qr.TableNumber,
                        tableId = qr.TableId,
                        expiryDate = formatDate,
                        protectedQrId = qr.QrId.ToString()
                    });
                }

                ViewBag.QrList = qrData;
            }

            return View(viewData);
        }

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}