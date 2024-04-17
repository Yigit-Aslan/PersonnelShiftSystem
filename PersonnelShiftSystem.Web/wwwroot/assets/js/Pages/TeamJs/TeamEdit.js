var postData = document.getElementById("postTeam");
$(document).ready(postData.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("TeamName", document.getElementById("TeamName").value);
    formData.append("TeamLeadId", document.getElementById("TeamLeadId").value);
    formData.append("PersonnelIds", document.getElementById("PersonnelIds").value);



    var action = $('#postEditTeamForm').attr('action');


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
        var pageId = document.getElementById('postEditTeamForm').getAttribute('data-id');
        var headers = new Headers({
            'RequestVerificationToken': antiForgeryToken
        });
        var selectedValue = $(this).val();
        $.ajax({
            type: 'GET',
            url: '/Teams/TeamEdit/' + pageId + '?handler=FilterPersonnel&teamLeadId=' + selectedValue,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('RequestVerificationToken', antiForgeryToken);
            },            success: function (response) {
                if (response.result === "Success") {
                    updateEditPersonnelDropdown(response.personnelModel);
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

function updateEditPersonnelDropdown(personnelModel) {
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