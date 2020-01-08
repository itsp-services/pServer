// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function openEditKeyPopup(i, keyValue) {
    alert("edit");
    document.getElementById("edit-key-value").setAttribute("value", keyValue);
    document.getElementById("edit-key-id").setAttribute("value", i);
    document.getElementById("edit-key-popup").setAttribute("style", "visibility: visible");
}

function closeEditKeyPopup() {
    $("#edit-key-popup")[0].setAttribute("style", "visibility: none");
}

function openAddKeyPopup() {
    $("#add-key-popup")[0].setAttribute("style", "visibility: visible");
}

function closeAddKeyPopup() {
    $("#add-key-popup")[0].setAttribute("style", "visibility: none");
}

function openManagePopup() {
    $("#manage-popup")[0].setAttribute("style", "visibility: visible");
}

function closeManagePopup() {
    $("#manage-popup")[0].setAttribute("style", "visibility: none");
}

$("#submit-new-key-button").click(function () {
    $.ajax({
        type: "POST",
        url: "/api/Key/add",
        data: JSON.stringify({
            KeyData: $("#new-key").val(),
            KeyNumber: "-1",
            Active: "true"
        }),
        contentType: 'application/json; charset=utf-8'

    });
});

$("#submit-edit-key-button").click(function () {
    var id = $("#edit-key-id").val();
    $.ajax({
        type: "POST",
        url: "/api/Key/edit/" + id,
        data: JSON.stringify({
            KeyData: $("#edit-key-value").val(),
            KeyNumber: id,
            Active: "true"
        }),
        contentType: 'application/json; charset=utf-8'

    });
});

function activateKey(i, keyValue) {
    var id = $("#edit-key-id").val();
    $.ajax({
        type: "POST",
        url: "/api/Key/edit/" + id,
        data: JSON.stringify({
            KeyData: keyValue,
            KeyNumber: id,
            Active: "true"
        }),
        contentType: 'application/json; charset=utf-8'
    });
}

function deactivateKey(i, keyValue) {
    var id = $("#edit-key-id").val();
    $.ajax({
        type: "POST",
        url: "/api/Key/edit/" + id,
        data: JSON.stringify({
            KeyData: keyValue,
            KeyNumber: id,
            Active: "false"
        }),
        contentType: 'application/json; charset=utf-8'
    });
}