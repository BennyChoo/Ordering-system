using Microsoft.EntityFrameworkCore;
using QFD.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QFD.Logic
{
    public class ProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;

        public ProductRepository(ApplicationDbContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<(bool OnError, int ErrorCode, List<ProductList> ProductList)> GetProductList()
        {
            var data = new List<ProductList>();
            var errorCode = 0;
            var onError = false;

            try
            {
                data = await _db.ProductList.FromSqlInterpolated($"GetProductList").AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Code:2001.Message : {ex.Message}  Stack : {ex.StackTrace}");

                onError = true;
                errorCode = 2001;
            }

            return (onError, errorCode, data);
        }

        public async Task<(bool OnError, int ErrorCode, List<TableQrCodeList> TableQrCodeList)> GetTableQrList()
        {
            var data = new List<TableQrCodeList>();
            var errorCode = 0;
            var onError = false;

            try
            {
                data = await _db.TableQrCodeList.FromSqlInterpolated($"GetActiveTableQrCode").AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Code:2001.Message : {ex.Message}  Stack : {ex.StackTrace}");

                onError = true;
                errorCode = 2001;
            }

            return (onError, errorCode, data);
        }

        public async Task<(bool OnError, int ErrorCode, int ProductId)> AddNewProduct(AddNewProduct data)
        {
            var onError = false;
            var errorCode = 0;
            var productId = 0;

            try
            {

                using(var transaction = _db.Database.BeginTransaction())
                {
                    var product = new Product
                    {
                        ProductName = data.ProductName,
                        ProductCategoryId = data.ProductCategoryId,
                        ProductDescription = data.ProductDescription,
                        IsAvailableForSale = data.IsAvailableForSale,
                        CreatedDate = DateTime.UtcNow,
                    };

                    await _db.Product.AddAsync(product);
                    await _db.SaveChangesAsync();

                    var productPrice = new ProductPrice
                    {
                        ProductId = product.ProductId,
                        Cost = data.ProductCost,
                        ValidFromDate = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                    };

                    await _db.ProductPrice.AddAsync(productPrice);
                    await _db.SaveChangesAsync();


                    await transaction.CommitAsync();

                    productId = product.ProductId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error code:2004.Message : {ex.Message} Stack : {ex.StackTrace} ");
                onError = true;
                errorCode = 2004;
            }

            return (onError, errorCode, productId);
        }

        public async Task<(bool OnError, int ErrorCode, ProductDetail? ProductDetail)> GetProductDetail(int id)
        {
            var onError = false;
            var errorCode = 0;
            ProductDetail data = default;

            try
            {
                var dList = await _db.ProductDetail.FromSqlInterpolated($"GetProductDetail {id}").AsNoTracking()
                .ToListAsync();

                data = dList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error code:2004.Message : {ex.Message} Stack : {ex.StackTrace} ");
                onError = true;
                errorCode = 2004;
            }

            return (onError, errorCode, data);
        }

        public async Task<(bool OnError, int ErrorCode, string Value)> UpdateProductDetail(int productId,string updatedBy, string newValue, string column)
        {
            var onError = false;
            var errorCode = 0;
            var returnValue = string.Empty;

            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    var entity =
                        await _db.Product.SingleOrDefaultAsync(a => a.ProductId == productId && a.DeletedDate == null);

                    //var userId = new Guid(updatedByUserId);

                    if (entity != null)
                    {
                        switch (column)
                        {
                            case "IsAvailableForSale":
                                {
                                    bool.TryParse(newValue, out var isAvailableForSale);
                                    returnValue = newValue;
                                    entity.IsAvailableForSale = isAvailableForSale;
                                    _db.Product.Update(entity);
                                }
                                break;

                            case "ProductName":
                                {
                                    returnValue = newValue;
                                    entity.ProductName = newValue.ToUpper();
                                    _db.Product.Update(entity);
                                }
                                break;

                            case "ProductDescription":
                                {
                                    returnValue = newValue;
                                    entity.ProductDescription = newValue.ToUpper();
                                    _db.Product.Update(entity);
                                }
                                break;

                            case "ProductCategory":
                                {
                                    int.TryParse(newValue, out var categoryId);

                                    returnValue = newValue;

                                    entity.ProductCategoryId = categoryId;
                                    _db.Product.Update(entity);
                                }
                                break;

                            case "ProductPrice":
                                {
                                    var productPriceEntity = await _db.ProductPrice.SingleOrDefaultAsync(a => a.ProductId == productId && a.DeletedDate == null);

                                    returnValue = newValue;

                                    if (productPriceEntity != null)
                                    {
                                        decimal.TryParse(newValue, out var productPrice);
                                        productPriceEntity.Cost = productPrice;
                                        _db.ProductPrice.Update(productPriceEntity);
                                    }
                                    
                                }
                                break;

                            default:
                                {
                                    onError = true;
                                    errorCode = 2006;
                                    _logger.LogError($"Error code:2006.No match found for ProductId : {productId}");
                                }
                                break;
                        }


                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    else
                    {
                        _logger.LogError($"Error code:2007.No match found for TableId :{productId}");
                        errorCode = 2007;
                        onError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error code:2008.Message : {ex.Message}.Stack : {ex.StackTrace}");
                errorCode = 2008;
                onError = true;
            }

            return (onError, errorCode, returnValue);
        }

        public async Task<(bool OnError, int ErrorCode, string Value)> RemoveTablePermanent(int tableId, string updatedByUserId)
        {
            var onError = false;
            var errorCode = 0;
            var returnValue = string.Empty;

            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    var entity =
                        await _db.Table.SingleOrDefaultAsync(a => a.TableId == tableId && a.DeletedDate == null);

                    //qr code that is still active
                    var qrIsActive = await _db.TableQrCode.AnyAsync(a => a.TableId == tableId && a.DeletedDate == null);

                    if (entity != null && !qrIsActive)
                    {
                        var table = await _db.Table.SingleOrDefaultAsync(a => a.TableId == tableId);

                        if (table != null)
                        {
                            _db.Table.Remove(table);
                        }


                        //remove tableQrCode table with the same tableId 
                        var tableQr = await _db.TableQrCode.Where(a => a.TableId == tableId).AsNoTracking().ToListAsync();

                        if (tableQr != null && tableQr.Count > 0)
                        {
                            _db.TableQrCode.RemoveRange(tableQr);
                        }


                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    else
                    {
                        _logger.LogError($"Error code:2007.No match found for PersonId :{tableId}");
                        errorCode = 2007;
                        onError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error code:2008.Message : {ex.Message}.Stack : {ex.StackTrace}");
                errorCode = 2008;
                onError = true;
            }

            return (onError, errorCode, returnValue);
        }

        public async Task<(bool OnError, int ErrorCode, List<ProductListView> Data)> RemoveMenu(int productId, string updatedByUserId)
        {
            var onError = false;
            var errorCode = 0;
            var returnValue = new List<ProductListView>();

            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    var entity =
                        await _db.Product.SingleOrDefaultAsync(a => a.ProductId == productId && a.DeletedDate == null);

                    if (entity != null)
                    {
                        entity.DeletedDate = DateTime.UtcNow;

                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();

                        //get active product
                        var afterRemoveData = await GetProductList();

                        if (!afterRemoveData.OnError && afterRemoveData.ProductList.Count > 0)
                        {
                            foreach(var product in afterRemoveData.ProductList)
                            {
                                returnValue.Add(new ProductListView
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
                    }
                    else
                    {
                        _logger.LogError($"Error code:2007.No match found for PersonId :{productId}");
                        errorCode = 2007;
                        onError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error code:2008.Message : {ex.Message}.Stack : {ex.StackTrace}");
                errorCode = 2008;
                onError = true;
            }

            return (onError, errorCode, returnValue);
        }


        public async Task<(bool OnError, int ErrorCode, List<TableQrCodeListView> Data)> GenerateQrCode(List<string> tableIdList)
        {
            var onError = false;
            var errorCode = 0;
            var returnValue = new List<TableQrCodeListView>();

            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    var qrTable = new List<TableQrCode>();

                    if (tableIdList != null && tableIdList.Count > 0)
                    {
                        foreach(var tableId in tableIdList)
                        {
                            Guid qrGuid = Guid.NewGuid();

                            qrTable.Add(new TableQrCode
                            {
                                QrCode = qrGuid,
                                TableId = Convert.ToInt32(tableId.ToString()),
                                ExpiryDate = DateTime.UtcNow.AddDays(1),
                                CreatedDate = DateTime.UtcNow,
                            });
                        }

                        await _db.AddRangeAsync(qrTable);
                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();

                        //get active qr code
                        var afterGenerateData = await GetTableQrList();

                        if (!afterGenerateData.OnError && afterGenerateData.TableQrCodeList.Count > 0)
                        {
                            foreach (var qr in afterGenerateData.TableQrCodeList)
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

                                returnValue.Add(new TableQrCodeListView
                                {
                                    qrId = qr.QrId,
                                    qrCode = qr.QrCode,
                                    tableId = qr.TableId,
                                    tableNumber = qr.TableNumber,
                                    expiryDate = formatDate,
                                    protectedQrId = qr.QrId.ToString()
                                });
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError($"Error code:2007.No match found forTableId :{tableIdList}");
                        errorCode = 2007;
                        onError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error code:2008.Message : {ex.Message}.Stack : {ex.StackTrace}");
                errorCode = 2008;
                onError = true;
            }

            return (onError, errorCode, returnValue);
        }
    }
}
