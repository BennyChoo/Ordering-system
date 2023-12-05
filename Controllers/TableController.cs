using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QFD.Logic;
using QFD.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class SubmitModel
{
    public string Name { get; set; }
    public string PrimaryKey { get; set; }
    public string Value { get; set; }
    public bool Success { get; set; }
}

public class GenerateQrCodeModel
{
    public List<string> AllTableId { get; set; }
}

public class QrReturnModel
{
    public bool Success { get; set; }
    public List<TableQrCodeListView> Data { get; set; }
}

public class TableActiveState
{
    public string Id { get; set; }
    public bool IsActive { get; set; }

}

namespace QFD.Controllers
{
    public class TableController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public TableController(ApplicationDbContext db, ILogger<HomeController> logger)
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
                foreach (var table in dataResult.TableList)
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

        [HttpGet]
        public IActionResult NewTable()
        {


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> NewTable(Table data)
        {
            var repo = new TableRepository(_db, _logger);


            var createResult = await repo.AddNewTable(data.TableNumber, data.Location, data.Description, data.TotalPerson);

            if (createResult.OnError || createResult.TableId == 0)
            {
                _logger.LogDebug("Debug code:3003.Add of member failed");
                return View();
            }

            return RedirectToAction("NewTable", "Table", new { isSuccess = true });
        }


        [HttpGet]
        public async Task<IActionResult> TableDetail(string id)
        {
            var data = new TableDetailView();

            try
            {
                var repo = new TableRepository(_db, _logger);

                if (!string.IsNullOrEmpty(id))
                {
                    int.TryParse(id, out var tableId);

                    var detailResult = await repo.GetTableDetail(tableId);

                    if (!detailResult.OnError)
                    {
                        data = new TableDetailView()
                        {
                            TableId = detailResult.TableDetail.TableId,
                            Location = detailResult.TableDetail.Location,
                            Description = detailResult.TableDetail.Description,
                            TableNumber = detailResult.TableDetail.TableNumber,
                            TotalPerson = detailResult.TableDetail.TotalPerson,
                            IsActive = detailResult.TableDetail.IsActive,
                            ProtectedTableId = detailResult.TableDetail.TableId.ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTableDetail([FromBody] SubmitModel value)
        {
            var repo = new TableRepository(_db, _logger);

            try
            {
                if (!string.IsNullOrEmpty(value.PrimaryKey))
                {
                    int.TryParse(value.PrimaryKey, out var tableId);

                    var qUser = string.Empty;

                    var updateResult = await repo.UpdateTableDetail(tableId, qUser, value.Value, value.Name);

                    if (updateResult.OnError)
                    {
                        value.Success = false;
                        value.Value = updateResult.ErrorCode.ToString();
                    }
                    else
                    {
                        value.Success = true;
                        value.Value = updateResult.Value;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(value);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTable(string id)
        {
            var repo = new TableRepository(_db, _logger);

            try
            {
                int.TryParse(id, out var tableId);

                var qUser = string.Empty;

                var updateResult = await repo.RemoveTablePermanent(tableId, qUser);

                return LocalRedirect("/Home/"); ;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error code:3008.Message: {ex.Message} Stack: {ex.StackTrace}");
                return null;
            }

        }

        [HttpPost]
        public async Task<IActionResult> RemoveSelectedQrCode(string id)
        {
            var repo = new TableRepository(_db, _logger);
            var returnModel = new QrReturnModel();

            try
            {
                var qrIdList = JsonConvert.DeserializeObject<List<string>>(id);

                //int.TryParse(id, out var qrId);

                var qUser = string.Empty;

                var updateResult = await repo.RemoveSelectedQrCode(qrIdList, qUser);

                if (!updateResult.OnError && updateResult.Data != null)
                {
                    returnModel.Data = updateResult.Data;
                    returnModel.Success = true;
                    return Json(returnModel);
                }

                returnModel.Data = updateResult.Data;
                returnModel.Success = false;
                return Json(returnModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error code:3008.Message: {ex.Message} Stack: {ex.StackTrace}");
                return null;
            }

        }

        [HttpPost]
        public async Task<IActionResult> RemoveQrCode(string id)
        {
            var repo = new TableRepository(_db, _logger);
            var returnModel = new QrReturnModel();

            try
            {

                int.TryParse(id, out var qrId);

                var qUser = string.Empty;

                var updateResult = await repo.RemoveQrCode(qrId, qUser);

                if (!updateResult.OnError && updateResult.Data != null)
                {
                    returnModel.Data = updateResult.Data;
                    returnModel.Success = true;
                    return Json(returnModel);
                }

                returnModel.Data = updateResult.Data;
                returnModel.Success = false;
                return Json(returnModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error code:3008.Message: {ex.Message} Stack: {ex.StackTrace}");
                return null;
            }

        }

        [HttpPost]
        public async Task<IActionResult> GenerateQrCode(string id)
        {
            var repo = new TableRepository(_db, _logger);
            var returnModel = new QrReturnModel();

            try
            {
                var tableIdList = JsonConvert.DeserializeObject<List<string>>(id);

                var result = await repo.GenerateQrCode(tableIdList);

                if (!result.OnError && result.Data != null)
                {
                    returnModel.Data = result.Data;
                    returnModel.Success = true;
                    return Json(returnModel);
                }

                returnModel.Data = result.Data;
                returnModel.Success = false;
                return Json(returnModel);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error code:3008.Message: {ex.Message} Stack: {ex.StackTrace}");
                return null;
            }

        }

        //[HttpPost]
        //public async Task<IActionResult> TableActive([FromBody] TableActiveState tableState)
        //{
        //    var repo = new TableRepository(_db, _logger);

        //    //var res = ExtractAndParse(loginState.Id);
        //    //if (res.OnError)
        //    //{
        //    //    return Json(new { isSuccess = false, errorCode = res.ErrorCode });
        //    //}

        //    var updateResult = await repo.TableActive(res.PersonId, res.AccountId, loginState.LoginActive);

        //    return Json(new { isSuccess = !updateResult.OnError, errorCode = updateResult.ErrorCode });
        //}
    }
}
