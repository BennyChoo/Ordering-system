$(document).ready(function () {
    //var proceedBtn = document.getElementById("proceedBtn")
    //var cancelBtn = document.getElementById("cancelBtn")
    //var deleteBtn = document.getElementById("delete")
    //var deleteAcknowledgeCheckbox = document.getElementById("DeleteAcknowledge");


    //proceedBtn.addEventListener("click", proceedDelete);
    //cancelBtn.addEventListener("click", hideDeleteSect);
    //deleteBtn.addEventListener("click", showDeleteSect);

    //deleteAcknowledgeCheckbox.addEventListener("change", enableProceedBtn);

})


function productActive(arg) {


    //var id = document.getElementById("XYPPST67").value;
    //const securityToken = $("[name=__RequestVerificationToken]").val();
    var protectedProductId = document.getElementById("protectedProductId").value
    const submitModel = { PrimaryKey: protectedProductId, Name: "IsAvailableForSale", Success: false, Value: arg.checked.toString() }
    //const data = { Id: id, LoginActive: arg.checked };

    var path = "/Product/UpdateProductDetail";
    $.ajax({
        url: path,
        type: "Post",
        data: JSON.stringify(submitModel),
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            if (typeof response.isSuccess !== "undefined") {
                successMessage(response.isSuccess, response.errorCode);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            const msg = xhr.responseText + thrownError;
            console.log(msg);
        }
    });
}