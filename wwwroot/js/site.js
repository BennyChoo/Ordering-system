// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function successMessage(isSuccess, msgError, msgOk) {


    var toast = new ej.notifications.Toast({
        target: document.body,
        position: { X: 'Center', Y: 'Top' }
    });


    if (isSuccess === false) {


        var error = "Error: " + msgError;
        toast.cssClass = "e-toast-danger";
        toast.title = "Error !";
        toast.content = error;
        toast.appendTo("#toast_type");
        toast.show();

    } else {

        toast.cssClass = 'e-toast-success';
        toast.title = "Success !";
        toast.appendTo("#toast_type");
        toast.show();

    }

}

function actionSuccess(e) {
    var res = e.data.result;


    if (typeof res.success != "undefined") {
        successMessage(res.success, res.Value);
    }


    //var toastObj = new ej.notifications.Toast({
    //    position: {
    //        X: "Right"
    //    },
    //    target: document.body
    //});
    //toastObj.appendTo("#toast_type");

    //if (typeof res.success != "undefined") {
    //    if (res.success === false) {

    //        toasts[0].content = "Not saved . Error code " + res.Value;
    //        toastObj.show(toasts[0]);

    //    } else {

    //        toasts[1].content = "Saved";
    //        toastObj.show(toasts[1]);

    //    }
    //}

}

function actionFailure(e) {

}