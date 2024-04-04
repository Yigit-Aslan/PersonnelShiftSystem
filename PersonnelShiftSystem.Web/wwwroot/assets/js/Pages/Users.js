$(document).ready(function () {
    $("#UserPhone").mask("(999) 999 - 9999");

});


var postData = document.getElementById("postUser");
$(document).ready(postData.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("UserName", document.getElementById("UserName").value);
    formData.append("UserSurname", document.getElementById("UserSurname").value);
    formData.append("UserMail", document.getElementById("UserMail").value);
    formData.append("UserPassword", document.getElementById("UserPassword").value);
    formData.append("UserPhone", document.getElementById("UserPhone").value);

    var action = $('#postUserForm').attr('action');


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
    document.getElementById('deleteUserId').value = itemId;

    Swal.fire({
        title: 'Emin misiniz?',
        text: 'Bu öğeyi silmek istediğinizden emin misiniz?',
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

    var action = '/Users/UserList?handler=DeleteUser';

    var formData = new FormData();
    formData.append("Id", $('#deleteUserId').val());

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
            await Swal.fire('Silindi!', 'Öğe başarıyla silindi.', 'success').then(function () {
                window.location.reload();
            });
        }
    }
    catch (e) {
        console.log(e)
    }

}


