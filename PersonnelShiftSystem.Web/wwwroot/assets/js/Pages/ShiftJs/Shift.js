var postShift = document.getElementById("postShift");
$(document).ready(postShift.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("ShiftName", document.getElementById("ShiftName").value);
    formData.append("StartingDate", document.getElementById("StartingDate").value);
    formData.append("EndingDate", document.getElementById("EndingDate").value);
    formData.append("ShiftStart", document.getElementById("ShiftStart").value);
    formData.append("ShiftEnd", document.getElementById("ShiftEnd").value);

    var action = $('#postShiftForm').attr('action');


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


async function confirmDelete(itemId) {
    document.getElementById('deleteShiftId').value = itemId;

    Swal.fire({
        title: 'Emin misiniz?',
        text: 'Bu Vardiyayı silmek istediğinizden emin misiniz?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'Hayır, vazgeç',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            // Formu gönder
            deleteItem();
        }
    });
}

async function deleteItem() {
    // Burada AJAX veya başka bir yöntemle silme işlemini gerçekleştirin
    // Örneğin, jQuery AJAX kullanarak:

    var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
    var headers = {
        'RequestVerificationToken': antiForgeryToken
    };

    var action = '/Shifts/ShiftList?handler=DeleteShift';

    var formData = new FormData();
    formData.append("Id", $('#deleteShiftId').val());

    try {
        var result = await $.ajax({

            type: 'post',
            url: action,
            //headers: headers,
            data: formData,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            processData: false,
            contentType: false,
            dataType: 'json'
        }); // JSON yanıt beklendiğini belirtin
        if (result.isSuccess) {
            await Swal.fire('Silindi!', 'Vardiya başarıyla silindi.', 'success').then(function () {
                window.location.reload();
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
    }
    catch (e) {
        console.log(e)
    }

}