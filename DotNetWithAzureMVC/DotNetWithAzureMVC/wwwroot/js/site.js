// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const apiUrl = "https://localhost:44359/api/TaskItems";

$(document).ready(function () {

    loadItems();


    // =========================
    // ADD
    // =========================

    $("#btnAdd").click(function () {

        clearForm();

        $("#modalTitle").text("Add Task");

        $("#taskModal").fadeIn();

    });


    // =========================
    // SAVE
    // =========================

    $("#btnSave").click(function () {

        const id = $("#taskId").val();

        if (id) {

            updateTask();

        }
        else {

            addTask();

        }

    });


    // =========================
    // CANCEL
    // =========================

    $("#btnCancel").click(function () {

        closeModal();

    });

});


// ==========================================
// GET TASKS
// ==========================================

function loadItems() {

    $.ajax({

        url: apiUrl,

        type: "GET",

        success: function (items) {

            renderTable(items);

        },

        error: function (xhr) {

            console.error(xhr);

            alert("Unable to load tasks.");

        }

    });

}


// ==========================================
// RENDER TABLE
// ==========================================

function renderTable(items) {

    $("#taskTableBody").empty();

    $("#itemCount").text(items.length);


    $.each(items, function (index, item) {

        const createdOn =
            new Date(item.createdOnUtc)
                .toLocaleString();

        const updatedOn =
            item.updatedOnUtc
                ? new Date(item.updatedOnUtc)
                    .toLocaleString()
                : "-";


        const row = `
                    <tr>

                        <td>
                            ${escapeHtml(item.title)}
                        </td>

                        <td>
                            ${escapeHtml(item.description)}
                        </td>

                        <td>
                            ${createdOn}
                        </td>

                        <td>
                            ${updatedOn}
                        </td>

                        <td>

                            <button
                                class="btn btn-sm btn-warning"
                                onclick="editTask('${item.id}')">
                                Update
                            </button>

                            <button
                                class="btn btn-sm btn-danger"
                                onclick="deleteTask('${item.id}')">
                                Delete
                            </button>

                        </td>

                    </tr>
                `;

        $("#taskTableBody").append(row);

    });

}


// ==========================================
// EDIT
// ==========================================

function editTask(id) {

    $.ajax({

        url: apiUrl + "/" + id,

        type: "GET",

        success: function (item) {

            $("#taskId").val(item.id);

            $("#txtTitle").val(item.title);

            $("#txtDescription")
                .val(item.description);

            $("#modalTitle")
                .text("Update Task");

            $("#taskModal").fadeIn();

        },

        error: function (xhr) {

            console.error(xhr);

            alert("Unable to load task.");

        }

    });

}


// ==========================================
// UPDATE
// ==========================================

function updateTask() {

    const id = $("#taskId").val();

    const item = {

        id: id,

        title: $("#txtTitle").val(),

        description:
            $("#txtDescription").val()

    };


    $.ajax({

        url: apiUrl + "/" + id,

        type: "PUT",

        contentType: "application/json",

        data: JSON.stringify(item),

        success: function () {

            closeModal();

            loadItems();

        },

        error: function (xhr) {

            console.error(xhr);

            alert("Unable to update task.");

        }

    });

}


// ==========================================
// ADD
// ==========================================

function addTask() {

    const item = {

        title: $("#txtTitle").val(),

        description:
            $("#txtDescription").val()

    };


    $.ajax({

        url: apiUrl,

        type: "POST",

        contentType: "application/json",

        data: JSON.stringify(item),

        success: function () {

            closeModal();

            loadItems();

        },

        error: function (xhr) {

            console.error(xhr);

            alert("Unable to create task.");

        }

    });

}


// ==========================================
// DELETE
// ==========================================

function deleteTask(id) {

    if (!confirm("Are you sure you want to delete this task?")) {
        return;
    }


    $.ajax({

        url: apiUrl + "/" + id,

        type: "DELETE",

        success: function () {

            loadItems();

        },

        error: function (xhr) {

            console.error(xhr);

            alert("Unable to delete task.");

        }

    });

}


// ==========================================
// CLEAR FORM
// ==========================================

function clearForm() {

    $("#taskId").val("");

    $("#txtTitle").val("");

    $("#txtDescription").val("");

}


// ==========================================
// CLOSE MODAL
// ==========================================

function closeModal() {

    clearForm();

    $("#taskModal").fadeOut();

}


// ==========================================
// HTML ENCODING
// ==========================================

function escapeHtml(value) {

    if (!value) {
        return "";
    }

    return $("<div>")
        .text(value)
        .html();

}