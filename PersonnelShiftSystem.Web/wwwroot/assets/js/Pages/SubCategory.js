
var postButton = document.getElementById("postButton");
$(document).ready(postButton.addEventListener('click', async function (event) {
    event.preventDefault();

    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("CategoryId", document.getElementById("CategoryId").value);
    formData.append("SubCategoryName", document.getElementById("SubCategoryName").value);
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
            title: "Kayıt Eklendi",
            text: result.message,
            icon: "success"
        }).then(function () {
            window.location.href = result.url;
        });
    }
    else {
        await Swal.fire({
            title: "Kayıt Eklenemedi",
            text: result.message,
            icon: "warning"
        }).then(function () {
            window.location.reload();
        });
    }
}));

async function deleteItem() {
    // Burada AJAX veya başka bir yöntemle silme işlemini gerçekleştirin
    // Örneğin, jQuery AJAX kullanarak:

    var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
    var headers = {
        'RequestVerificationToken': antiForgeryToken
    };

    var action = '/SubCategories/SubCategoryList?handler=DeleteSubCategory';

    var formData = new FormData();
    formData.append("Id", $('#deleteItemId').val());

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


async function confirmDelete(itemId) {
    document.getElementById('deleteItemId').value = itemId;

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
