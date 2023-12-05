using Microsoft.EntityFrameworkCore;
using QFD.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QFD.Logic
{
    public class TableRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;

        public TableRepository(ApplicationDbContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<(bool OnError, int ErrorCode, List<TableList> TableList)> GetTableList()
        {
            var data = new List<TableList>();
            var errorCode = 0;
            var onError = false;

            try
            {
                data = await _db.TableList.FromSqlInterpolated($"GetTableList").AsNoTracking().ToListAsync();
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

        public async Task<(bool OnError, int ErrorCode, int TableId)> AddNewTable(int tableNumber, string location, string description, int totalPerson)
        {
            var onError = false;
            var errorCode = 0;
            var tableId = 0;

            try
            {

                using(var transaction = _db.Database.BeginTransaction())
                {
                    var table = new Table
                    {
                        Location = location,
                        Description = description,
                        TableNumber = tableNumber,
                        TotalPerson = totalPerson,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                    };

                    await _db.Table.AddAsync(table);
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    tableId = table.TableId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error code:2004.Message : {ex.Message} Stack : {ex.StackTrace} ");
                onError = true;
                errorCode = 2004;
            }

            return (onError, errorCode, tableId);
        }

        public async Task<(bool OnError, int ErrorCode, TableDetail? TableDetail)> GetTableDetail(int id)
        {
            var onError = false;
            var errorCode = 0;
            TableDetail data = default;

            try
            {
                var dList = await _db.TableDetail.FromSqlInterpolated($"GetTableDetail {id}").AsNoTracking()
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

        public async Task<(bool OnError, int ErrorCode, string Value)> UpdateTableDetail(int tableId,string updatedBy, string newValue, string column)
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

                    //var userId = new Guid(updatedByUserId);

                    if (entity != null)
                    {
                        switch (column)
                        {
                            case "Location":
                                {
                                    entity.Location = returnValue = newValue.ToUpper();
                                    _db.Table.Update(entity);
                                }
                                break;
                            case "Description":
                                {
                                    entity.Description = returnValue = newValue.ToUpper();
                                    _db.Table.Update(entity);
                                }
                                break;
                            case "TotalPerson":
                                {
                                    int.TryParse(newValue, out var newTotalPerson);
                                    returnValue = newValue;
                                    entity.TotalPerson = newTotalPerson;
                                    _db.Table.Update(entity);
                                }
                                break;
                            case "TableNumber":
                                {
                                    int.TryParse(newValue, out var newTableNumber);
                                    returnValue = newValue;
                                    entity.TableNumber = newTableNumber;
                                    _db.Table.Update(entity);
                                }
                                break;
                            case "IsActive":
                                {
                                    bool.TryParse(newValue, out var isActive);
                                    returnValue = newValue;
                                    entity.IsActive = isActive;
                                    _db.Table.Update(entity);
                                }
                                break;
                            default:
                                {
                                    onError = true;
                                    errorCode = 2006;
                                    _logger.LogError($"Error code:2006.No match found for TableId : {tableId}");
                                }
                                break;
                        }


                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    else
                    {
                        _logger.LogError($"Error code:2007.No match found for TableId :{tableId}");
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


        public async Task<(bool OnError, int ErrorCode, List<TableQrCodeListView> Data)> RemoveSelectedQrCode(List<string> qrIdList, string updatedByUserId)
        {
            var onError = false;
            var errorCode = 0;
            var returnValue = new List<TableQrCodeListView>();

            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    foreach (var qrId in qrIdList)
                    {
                        var qrIdInt = Convert.ToInt32(qrId);

                        var entity =
                        await _db.TableQrCode.SingleOrDefaultAsync(a => a.QrId == qrIdInt && a.DeletedDate == null);

                        if (entity != null)
                        {
                            entity.DeletedDate = DateTime.UtcNow;

                        }
                        else
                        {
                            _logger.LogError($"Error code:2007.No match found for PersonId :{qrId}");
                            errorCode = 2007;
                            onError = true;
                        }

                    }
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    //get active qr code
                    var afterRemoveData = await GetTableQrList();

                    if (!afterRemoveData.OnError && afterRemoveData.TableQrCodeList.Count > 0)
                    {
                        foreach (var qr in afterRemoveData.TableQrCodeList)
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

        public async Task<(bool OnError, int ErrorCode, List<TableQrCodeListView> Data)> RemoveQrCode(int qrId, string updatedByUserId)
        {
            var onError = false;
            var errorCode = 0;
            var returnValue = new List<TableQrCodeListView>();

            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {

                    var entity =
                    await _db.TableQrCode.SingleOrDefaultAsync(a => a.QrId == qrId && a.DeletedDate == null);

                    if (entity != null)
                    {
                        entity.DeletedDate = DateTime.UtcNow;

                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();


                        //get active qr code
                        var afterRemoveData = await GetTableQrList();

                        if (!afterRemoveData.OnError && afterRemoveData.TableQrCodeList.Count > 0)
                        {
                            foreach (var qr in afterRemoveData.TableQrCodeList)
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
                        _logger.LogError($"Error code:2007.No match found for PersonId :{qrId}");
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
