using Microsoft.AspNetCore.Mvc;
using QFD.Business;
using QFD.Logic;
using QFD.Models;
using QFD.Repository;
using System.Net.Mail;
using Attachment = QFD.Business.Attachment;


public class ProductReturnModel
{
    public bool Success { get; set; }
    public List<ProductListView> Data { get; set; }
}

namespace QFD.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly AppSpecificConfig _app;

        public ProductController(ApplicationDbContext db, ILogger<ProductController> logger, IConfiguration config)
        {
            _db = db;
            _logger = logger;
            _app = config.GetSection("FileUpload").Get<AppSpecificConfig>();
        }


        public async Task<IActionResult> Index()
        {
            var viewData = new List<ProductListView>();
            var repo = new ProductRepository(_db, _logger);

            var dataResult = await repo.GetProductList();

            if (!dataResult.OnError && dataResult.ProductList.Count > 0)
            {
                foreach (var product in dataResult.ProductList)
                {
                    viewData.Add(new ProductListView
                    {
                        productId = product.ProductId,
                        productName = product.ProductName,
                        productDescription = product.ProductDescription,
                        isAvailableForSale = product.IsAvailableForSale,
                        productCategory = product.ProductCategory,
                        productPrice = product.ProductPrice,
                        protectedProductId = product.ProductId.ToString()
                    });
                }
            }


            return View(viewData);
        }

        [HttpGet]
        public IActionResult NewProduct()
        {


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> NewProduct(AddNewProduct data)
        {
            var repo = new ProductRepository(_db, _logger);


            var createResult = await repo.AddNewProduct(data);

            if (createResult.OnError || createResult.ProductId == 0)
            {
                _logger.LogDebug("Debug code:3003.Add of member failed");
                return View();
            }

            //now check if there is an image
            //if (data.Image != null)
            //{
            //    var fileSaveResult = SaveImage(data.Image, createResult.ProductId, string.Empty);

            //    if (fileSaveResult.onError)
            //    {
            //        _logger.LogDebug("Exit New Inventory On Error");

            //        return View("Error");
            //    }
            //}

            return RedirectToAction("NewProduct", "Product", new { isSuccess = true });
        }


        [HttpGet]
        public async Task<IActionResult> ProductDetail(string id)
        {
            var data = new ProductDetailView();

            try
            {
                var repo = new ProductRepository(_db, _logger);

                if (!string.IsNullOrEmpty(id))
                {
                    int.TryParse(id, out var productId);

                    var detailResult = await repo.GetProductDetail(productId);

                    if (!detailResult.OnError)
                    {
                        data = new ProductDetailView()
                        {
                            ProductId = detailResult.ProductDetail.ProductId,
                            ProductName = detailResult.ProductDetail.ProductName,
                            ProductDescription = detailResult.ProductDetail.ProductDescription,
                            IsAvailableForSale = detailResult.ProductDetail.IsAvailableForSale,
                            ProductCategory = detailResult.ProductDetail.ProductCategory,
                            ProductPrice = detailResult.ProductDetail.ProductPrice,
                            ProtectedProductId = detailResult.ProductDetail.ProductId.ToString()
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
        public async Task<IActionResult> UpdateProductDetail([FromBody] SubmitModel value)
        {
            var repo = new ProductRepository(_db, _logger);

            try
            {
                if (!string.IsNullOrEmpty(value.PrimaryKey))
                {
                    int.TryParse(value.PrimaryKey, out var productId);

                    var qUser = string.Empty;

                    var updateResult = await repo.UpdateProductDetail(productId, qUser, value.Value, value.Name);

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
        public async Task<IActionResult> RemoveMenu(string id)
        {
            var repo = new ProductRepository(_db, _logger);
            var returnModel = new ProductReturnModel();

            try
            {
                int.TryParse(id, out var productId);

                var qUser = string.Empty;

                var updateResult = await repo.RemoveMenu(productId, qUser);

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

        private (bool onError, int ErrorCode, int AttachmentId) SaveImage(IFormFile image, int entityId, string user)
        {
            (bool OnError, bool FileSaved) fileSavedResult;
            fileSavedResult.FileSaved = false;

            var onError = false;
            var errorCode = 0;
            var attachmentId = 0;

            var fs = new Attachment(_logger);

            var filePath = fs.CreateFileName(_app.Path, 1, entityId);
            var resultDirectory = fs.IsDirectoryExists(filePath, true);

            if (!resultDirectory.OnError && resultDirectory.DirectoryExists)
            {
                var fileExistResult = fs.IsFileExists(filePath, image.FileName);
                // No errors
                if (!fileExistResult.OnError && !fileExistResult.FileExists)
                {
                    fileSavedResult = fs.SaveFile(image, filePath, image.FileName);
                }
                else
                {
                    onError = true;
                    errorCode = 7007;
                }
            }
            else
            {
                onError = true;
                errorCode = 7006;
            }

            if (fileSavedResult.FileSaved)
            {
                var data = new AddNewAttachmentView
                {
                    ContentType = image.ContentType,
                    FileSize = image.Length,
                    FileName = image.FileName,
                    EntityId = entityId,
                    EntityTypeId = 1,
                    Comment = "Item Image",
                    AttachmentTypeId = 1
                };
                var repo = new FileRepository(_db, _logger);

                var fileRes = repo.Add(data, filePath, user);
                if (!fileRes.OnError)
                {
                    attachmentId = fileRes.AttachmentId;
                }
                else
                {
                    onError = true;
                    errorCode = fileRes.ErrorCode;
                }
            }


            return (onError, errorCode, attachmentId);
        }
    }
}
