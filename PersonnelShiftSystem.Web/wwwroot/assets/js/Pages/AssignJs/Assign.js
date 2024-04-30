var postAssign = document.getElementById("postAssign");
$(document).ready(postAssign.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("ShiftId", document.getElementById("ShiftId").value);
    var selectElement = document.getElementById("TeamIds");
    var options = selectElement && selectElement.options;

    for (var i = 0; i < options.length; i++) {
        if (options[i].selected) {
            formData.append("TeamIds[]", options[i].value);
        }
    }


    var action = $('#postAssignForm').attr('action');


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




async function confirmDelete(itemId) {
    document.getElementById('deleteAssignId').value = itemId;

    Swal.fire({
        title: 'Emin misiniz?',
        text: 'Bu Takımın Vardiyasını iptal etmek istediğinizden emin misiniz?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Evet, iptal et!',
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

    var action = '/AssignShiftTeams/AssignShiftTeamList?handler=DeleteAssign';

    var formData = new FormData();
    formData.append("Id", $('#deleteAssignId').val());

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
            await Swal.fire('İptal Edildi!', 'Takımın vardiyası başarıyla iptal edildi.', 'success').then(function () {
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