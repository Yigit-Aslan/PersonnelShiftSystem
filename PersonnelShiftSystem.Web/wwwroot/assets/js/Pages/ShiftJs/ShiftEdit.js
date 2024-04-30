var postShiftEdit = document.getElementById("postShiftEdit");
$(document).ready(postShiftEdit.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("ShiftName", document.getElementById("ShiftName").value);
    formData.append("StartingDate", document.getElementById("StartingDate").value);
    formData.append("EndingDate", document.getElementById("EndingDate").value);
    formData.append("ShiftStart", document.getElementById("ShiftStart").value);
    formData.append("ShiftEnd", document.getElementById("ShiftEnd").value);

    var action = $('#postShiftEditForm').attr('action');


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
            title: "İşlem Başarılı",
            text: result.message,
            icon: "success"
        }).then(function () {
            window.location.href = result.url;
        });
    }
    else {
        await Swal.fire({
            title: "İşlem Başarısız",
            text: result.message,
            icon: "warning"
        }).then(function () {
            window.location.reload();
        });
    }
}));