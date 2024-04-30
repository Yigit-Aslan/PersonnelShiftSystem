$(document).ready(function () {
    $("#PhoneNumber").mask("(999) 999 - 9999");

});


var postUserEdit = document.getElementById("postUserEdit");
$(document).ready(postUserEdit.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("Name", document.getElementById("Name").value);
    formData.append("Surname", document.getElementById("Surname").value);
    formData.append("MailAddress", document.getElementById("MailAddress").value);
    formData.append("Password", document.getElementById("Password").value);
    formData.append("PasswordAgain", document.getElementById("PasswordAgain").value);
    formData.append("PhoneNumber", document.getElementById("PhoneNumber").value);
    formData.append("RoleId", document.getElementById("RoleId").value);


    var action = $('#postUserEditForm').attr('action');


    var result = await $.ajax({
        type: 'post',
        url: action,
        data: formData,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        processData: false,
        contentType: false,
        datatype: 'json',

    });
    if (result.isSuccess) {
        await Swal.fire({
            title: result.title,
            text: result.title,
            icon: "success"
        }).then(function () {
            window.location.href = result.url;
        });
    }
    else {
        await Swal.fire({
            title: result.title,
            text: result.message,
            icon: "warning"
        }).then(function () {
            window.location.reload();
        });
    }
}));

