var postData = document.getElementById("postTeam");
$(document).ready(postData.addEventListener('click', async function (event) {
    event.preventDefault();


    var formData = new FormData();

    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("TeamName", document.getElementById("TeamName").value);
    formData.append("TeamLeadId", document.getElementById("TeamLeadId").value);
    formData.append("PersonnelIds", document.getElementById("PersonnelIds").value);



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