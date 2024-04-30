var postData = document.getElementById("postTeam");
$(document).ready(postData.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("TeamName", document.getElementById("TeamName").value);
    formData.append("TeamLeadId", document.getElementById("TeamLeadId").value);
    var selectElement = document.getElementById("PersonnelIds");
    var options = selectElement && selectElement.options;

    for (var i = 0; i < options.length; i++) {
        if (options[i].selected) {
            formData.append("PersonnelIds[]", options[i].value);
        }
    }


    var action = $('#postTeamForm').attr('action');


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

$(document).ready(function () {
    $('#TeamLeadId').change(function () {
        var antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
        var headers = new Headers({
            'RequestVerificationToken': antiForgeryToken
        });
        var selectedValue = $(this).val();
        $.ajax({
            type: 'GET',
            url: '/Teams/TeamAdd?handler=FilterPersonnel&teamLeadId=' + selectedValue,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            headers: headers,
            success: function (response) {
                if (response.result === "Success") {
                    updatePersonnelDropdown(response.personnelModel);
                    console.log(response.result);
                }
            },
            error: function (xhr, status, error) {
                console.log("Ajax error:", status, error);
                console.log(xhr.responseText);
            }
        });
    });
});

function updatePersonnelDropdown(personnelModel) {
    var $personnelDropdown = $('#PersonnelIds'); // Subcategory dropdown'ının ID'si
    $personnelDropdown.empty(); // Mevcut option'ları temizleyin
    // Yeni option'ları ekleyin
    $.each(personnelModel, function (index, personnel) {
        $personnelDropdown.append($('<option>', {
            value: personnel.id,
            text: personnel.name + ' ' + personnel.surname,
        }));
    });
}




async function confirmDelete(itemId) {
    document.getElementById('deleteTeamId').value = itemId;

    Swal.fire({
        title: 'Emin misiniz?',
        text: 'Bu Takımı silmek istediğinizden emin misiniz?',
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

    var action = '/Teams/TeamList?handler=DeleteTeam';

    var formData = new FormData();
    formData.append("Id", $('#deleteTeamId').val());

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
            await Swal.fire('Silindi!', 'Takım başarıyla silindi.', 'success').then(function () {
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

