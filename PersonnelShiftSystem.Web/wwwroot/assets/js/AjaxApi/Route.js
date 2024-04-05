
var postLogin = document.getElementById("postLogin");
$(document).ready(postLogin.addEventListener('click', async function (event) {
    event.preventDefault();

    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("MailAddress", document.getElementById("MailAddress").value);
    formData.append("Password", document.getElementById("Password").value);

    var action = $('#postForm').attr('action');

    var result = await $.ajax({
        type: 'post',
        url: action,
        data: formData,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        processData: false,
        contentType: false,
        datatype: "json"
    });
    if (result.isSuccess) {
        await Swal.fire({
            title: "Başarılı",
            text: result.message,
            icon: "success",
            timer: 1000
        }).then(function () {
            window.location.href = result.url;
        });
    }
    else {
        await Swal.fire({
            title: "Giriş Başarısız",
            text: result.message,
            icon: "warning"
        });
    }
}));





