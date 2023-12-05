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

function showDeleteSect() {
    var deleteSection = document.getElementById("deleteSectionParent");
    var deleteParent = document.querySelector(".deleteParent");

    if (deleteSection.hidden) {
        deleteSection.hidden = false;

        //hide delete button
        deleteParent.hidden = true;
    }
}

function hideDeleteSect() {
    var deleteParent = document.querySelector(".deleteParent");
    var deleteSection = document.getElementById("deleteSectionParent");
    var acknowledgeErr = document.getElementById("acknowledgeErr");

    var deleteCheckbox = document.getElementById("DeleteAcknowledge");

    var proceedBtn = document.getElementById("proceedBtn")
    var cancelBtn = document.getElementById("cancelBtn")

    if (!deleteSection.hidden) {
        //hide delete section
        deleteSection.hidden = true;

        //show delete button
        deleteParent.hidden = false;

        acknowledgeErr.textContent = "";
        //acknowledgeErr.hidden = true;

        //uncheck checkbox
        deleteCheckbox.checked = false;

        //enable proceed & delete btn

        proceedBtn.disabled = false;
        cancelBtn.disabled = false;

    }
}



function proceedDelete(event) {
    var deleteAcknowledgeCheckbox = document.getElementById("DeleteAcknowledge");
    var acknowledgeErr = document.getElementById("acknowledgeErr");

    var deleteForm = document.getElementById("deleteForm");
    var proceedBtn = document.getElementById("proceedBtn")
    var cancelBtn = document.getElementById("cancelBtn")

    event.preventDefault();
    proceedBtn.disabled = true;
    cancelBtn.disabled = true;


    if (!deleteAcknowledgeCheckbox.checked) {
        acknowledgeErr.textContent = "You must check this to proceed";
    }
    else {
        acknowledgeErr.textContent = "";
        //acknowledgeErr.hidden = true;
    }

    if (deleteAcknowledgeCheckbox.checked) {


        deleteForm.submit();
    }

    cancelBtn.disabled = false;
}

function enableProceedBtn() {
    var deleteAcknowledgeCheckbox = document.getElementById("DeleteAcknowledge");
    var proceedBtn = document.getElementById("proceedBtn")

    if (deleteAcknowledgeCheckbox.checked) {
        proceedBtn.disabled = false;
    }
}


function tableActive(arg) {


    //var id = document.getElementById("XYPPST67").value;
    //const securityToken = $("[name=__RequestVerificationToken]").val();
    var protectedTableId = document.getElementById("protectedTableId").value
    const submitModel = { PrimaryKey: protectedTableId, Name: "IsActive", Success: false, Value: arg.checked.toString() }
    //const data = { Id: id, LoginActive: arg.checked };

    var path = "/Table/UpdateTableDetail";
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