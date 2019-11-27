// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function openPublicKeyPopup() {
    document.getElementsByClassName("key-edit-popup")[0].setAttribute("style", "visibility: visible");
}

function closePublicKeyPopup() {
    document.getElementsByClassName("key-edit-popup")[0].setAttribute("style", "visibility: none");
}

$("#submit-new-key-button").click(function () {
    console.log($("#new-key").val());
    $.ajax({
        type: "POST",
        url: "/api/Key/add",
        data: JSON.stringify({
            KeyData: $("#new-key").val(),
            KeyNumber: "1",
            Active: "true"
        }),
        contentType: 'application/json; charset=utf-8',
        success: function (resultData) {
            alert("Save Complete");
            closePublicKeyPopup();
        },
        error: function (jqXhr, textstatus, errorThrown) {

            alert(errorThrown);

        }
    });
}); 