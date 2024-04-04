async function Convert() {
    var firstText = await document.getElementById("ProductName").value;

    // Türkçe karakterleri İngilizce harf formatına çevir
    var FormatConverter = await firstText.toLowerCase()
        .replace(/ğ/g, 'g')
        .replace(/ü/g, 'u')
        .replace(/ş/g, 's')
        .replace(/ı/g, 'i')
        .replace(/ö/g, 'o')
        .replace(/ç/g, 'c')
        .replace(/\s+/g, '-'); // Boşlukları tire ile değiştir

    // İkinci metin kutusuna yaz
    document.getElementById("secondText").value = FormatConverter;
    document.getElementById("slugNameHidden").value = FormatConverter;
}


$(document).ready(function () {
    $('#CategoryId').change(function () {
        var antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
        var headers = new Headers({
            'RequestVerificationToken': antiForgeryToken
        });
        var selectedValue = $(this).val();
        $.ajax({    
            type: 'GET',
            url: '/Products/ProductAdd?handler=FilterSub&categoryId=' + selectedValue,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            headers: headers,
            success: function (response) {
                if (response.result === "Success") {
                    updateSubcategoryDropdown(response.subcategoryModel);
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

function updateSubcategoryDropdown(subcategoryModel) {
    var $subCategoryDropdown = $('#SubcategoryId'); // Subcategory dropdown'ının ID'si
    $subCategoryDropdown.empty(); // Mevcut option'ları temizleyin
    // Yeni option'ları ekleyin
    $.each(subcategoryModel, function (index, subcategory) {
        $subCategoryDropdown.append($('<option>', {
            value: subcategory.id,
            text: subcategory.subCategoryName
        }));
    });
}



var postUpdate = document.getElementById("postUpdate");
$(document).ready(postUpdate.addEventListener('click', async function (event) {
    event.preventDefault();

    var formData = new FormData();


    // Diğer form verilerini de FormData'ya ekleyebilirsiniz
    formData.append("ProductName", document.getElementById("ProductName").value);
    formData.append("CategoryId", document.getElementById("CategoryId").value);
    formData.append("SubCategoryId", document.getElementById("SubcategoryId").value);
    formData.append("GenderId", document.getElementById("GenderId").value);
    formData.append("ColorId", document.getElementById("ColorId").value);
    formData.append("SizeId", document.getElementById("SizeId").value);
    formData.append("Price", document.getElementById("Price").value);
    formData.append("Stock", document.getElementById("Stock").value);
    // TinyMCE editörünü al
    var editor = tinyMCE.get('texteditor');
    // Editör içeriğini al
    var content = editor.getContent();
    // İçeriği kullanarak işlem yapabilirsiniz
    formData.append("Description", content);
    formData.append("DiscountPercent", document.getElementById("DiscountPercent").value);
    formData.append("SlugName", document.getElementById("secondText").value);


    var action = document.getElementById('postForm').action

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
            title: "Kayıt Güncellendi",
            text: result.message,
            icon: "success"
        }).then(function () {
            window.location.href = result.url;
        });
    }
    else {
        await Swal.fire({
            title: "Kayıt Güncellenemedi",
            text: result.message,
            icon: "warning"
        });
    }
}));