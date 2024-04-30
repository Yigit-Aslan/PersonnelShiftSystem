var postTransfer = document.getElementById("postTransfer");
$(document).ready(postTransfer.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("TeamId", document.getElementById("TeamId").value);
    formData.append("TeamTransferId", document.getElementById("TeamTransferId").value);
    formData.append("PersonnelId", document.getElementById("PersonnelId").value);

    var action = $('#postTransferForm').attr('action');


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
$(document).ready(function () {
    $('#TeamId').change(function () {
        var antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
        var selectedValue = $(this).val();
        $.ajax({
            type: 'GET',
            url: '/Teams/TeamTransfer?handler=FilterTeamPersonnel&teamId=' + selectedValue,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('RequestVerificationToken', antiForgeryToken);
            }, success: function (response) {
                if (response.result === "Success") {
                    updateEditPersonnelDropdown(response.personnelModel);
                    updateTransferTeamDropdown(selectedValue);
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
    var $personnelDropdown = $('#PersonnelId'); // Subcategory dropdown'ının ID'si
    $personnelDropdown.empty(); // Mevcut option'ları temizleyin
    // Yeni option'ları ekleyin
    $.each(personnelModel, function (index, personnel) {
        $personnelDropdown.append($('<option>', {
            value: personnel.id,
            text: personnel.name + ' ' + personnel.surname,
        }));
    });
}

function updateTransferTeamDropdown(selectedValue) {
    $.ajax({
        type: 'GET',
        url: '/Teams/TeamTransfer?handler=FilterOtherTeams&teamId=' + selectedValue,
        success: function (response) {
            if (response.result === "Success") {
                updateOtherTeamsDropdown(response.teamModel);
            }
        },
        error: function (xhr, status, error) {
            console.log("Ajax error:", status, error);
            console.log(xhr.responseText);
        }
    });
}

function updateOtherTeamsDropdown(teamModel) {
    var $teamTransferDropdown = $('#TeamTransferId');
    $teamTransferDropdown.empty();
    $.each(teamModel, function (index, team) {
        $teamTransferDropdown.append($('<option>', {
            value: team.id,
            text: team.teamName,
        }));
    });
}