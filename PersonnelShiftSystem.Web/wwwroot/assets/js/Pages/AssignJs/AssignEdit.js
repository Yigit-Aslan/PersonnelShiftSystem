var postAssignEdit = document.getElementById("postAssignEdit");
$(document).ready(postAssignEdit.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("ShiftId", document.getElementById("ShiftId").value);
    formData.append("TeamId", document.getElementById("TeamId").value);

    var action = $('#postAssignEditForm').attr('action');


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
            title: "Düzenleme Başarılı",
            text: result.message,
            icon: "success"
        }).then(function () {
            window.location.href = result.url;
        });
    }
    else {
        await Swal.fire({
            title: "Düzenleme Başarısız",
            text: result.message,
            icon: "warning"
        }).then(function () {
            window.location.reload();
        });
    }
}));