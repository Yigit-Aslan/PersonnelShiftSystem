$(document).ready(async function () {
    $('.upload-file-link').on('click', async function (e) {
        e.preventDefault();
        const orderId = $(this).data('order-id');
        await showFileUploadAlert(orderId);
    });
});
async function showFileUploadAlert(orderId) {
    await Swal.fire({
        title: 'Dosya Yükleme',
        html: '<input type="file" id="fileInput" class="swal2-input"  style="width:100%;"  accept=".pdf">',
        showCancelButton: true,
        confirmButtonText: 'Yükle',
        cancelButtonText: 'İptal',
        preConfirm: async () => {
            return new Promise(async (resolve) => {
                const fileInput = document.getElementById('fileInput');
                const selectedFile = fileInput.files[0];
                if (!selectedFile) {
                    await Swal.showValidationMessage('Dosya seçiniz.');
                    resolve(false);
                } else {
                    resolve(selectedFile);
                }
            });
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const uploadedFile = result.value;
            uploadFileToServer(uploadedFile, orderId);
        }
    });
}

async function uploadFileToServer(file, orderId) {
    var formData = new FormData();
    formData.append("File", file);
    formData.append("Id", orderId);
    var result = await $.ajax({
        type: 'post',
        url: '/Orders/OrderList?handler=UploadFile',  // Sunucu tarafında dosyanın alındığı endpoint'i belirtin
        data: formData,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        processData: false,
        contentType: false,
    });
    if (result.isSuccess) {
        await Swal.fire('Başarılı!', 'Dosya başarıyla yüklendi!', 'success');
        window.location.reload();

    }
    else {
        await Swal.fire('Hata!', 'Dosya yükleme sırasında bir hata oluştu.', 'error');
        window.location.reload();

    }
}

document.addEventListener('DOMContentLoaded', function () {

    var emailData = document.getElementById("emailPost");

    if (emailData) {

        $(document).ready(emailData.addEventListener('click', async function (event) {
            event.preventDefault();

            const { value: email } = await Swal.fire({
                title: "Kullanıcı Bilgilerini Getir",
                input: "email",
                inputLabel: "Email Adresi:",
                inputPlaceholder: "Email adresi giriniz",
                showCancelButton: true,
                confirmButtonText: "Getir",
                cancelButtonText: "İptal",
                preConfirm: (value) => {
                    if (!value) {
                        Swal.showValidationMessage("Lütfen bir e-posta adresi girin!");
                    }
                    return value;
                },
            });

            if (email) {
                // C# endpoint'ine AJAX isteği gönder
                var result = await $.ajax({
                    method: 'GET',
                    url: '/Orders/OrderAdd/?handler=UserAddressDropdown&email=' + email,
                    contentType: 'application/json; charset=utf-8',

                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    datatype: 'json',

                });

                if (result.isSuccess) {
                    await updateAddressDropdown(result.userAddressList);

                }
                else {
                    await Swal.fire({
                        title: "Kayıt Eklenemedi",
                        text: result.message,
                        icon: "warning"
                    });
                }
            }

        }));
    }
});
function updateAddressDropdown(userAddressList) {
    var $addressDropdown = $('#AddressId'); // Subcategory dropdown'ının ID'si
    //$addressDropdown.empty(); // Mevcut option'ları temizleyin
    // Yeni option'ları ekleyin
    $.each(userAddressList, function (index, address) {
        $addressDropdown.append($('<option>', {
            value: address.id,
            text: address.addressTitle
        }));
    });
    Swal.fire({
        title: "Güncelleme Başarılı",
        text: "Adresler başarıyla getirildi",
        icon: "success"
    });
}


$(document).ready(async function () {
    $('#AddressId').change(async function () {
        var antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
        var headers = new Headers({
            'RequestVerificationToken': antiForgeryToken
        });
        var selectedValue = await $(this).val();
        var result = await $.ajax({
            type: 'GET',
            url: '/Orders/OrderAdd?handler=FilterAddress&addressId=' + selectedValue,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            headers: headers,
        });
        if (result.isSuccess) {
            await updateAddressFields(result.addressModel);
        }
    });
});

async function updateAddressFields(addressModel) {
    // //$("#texteditor").val(result.AddressModel.addressDetails).trigger('input');

    tinymce.get("texteditor").setContent(addressModel.addressDetails)
    // // İl dropdownunu güncelle
    $("#ProvinceId").val(addressModel.provinceId);

    // // İlçe dropdownunu result.addressModel.DistrictId);
    $("#DistrictId").val(addressModel.districtId);

    // // Posta Kodu inputunu güncelle
    $("#PostCodeId").val(addressModel.postCode);
}


$(document).ready(function () {
    $('#ProvinceId').change(function () {
        var antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
        var headers = new Headers({
            'RequestVerificationToken': antiForgeryToken
        });
        var selectedValue = $(this).val();
        $.ajax({
            type: 'GET',
            url: '/Orders/OrderAdd?handler=FilterDistrict&provinceId=' + selectedValue,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            headers: headers,
            success: function (response) {
                if (response.result === "Success") {
                    updateDistrictDropdown(response.districtModel);
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

function updateDistrictDropdown(districtModel) {
    var $districtDropdown = $('#DistrictId'); // Subcategory dropdown'ının ID'si
    $districtDropdown.empty(); // Mevcut option'ları temizleyin
    // Yeni option'ları ekleyin
    $.each(districtModel, function (index, district) {
        $districtDropdown.append($('<option>', {
            value: district.id,
            text: district.districtName
        }));
    });
}




var postData = document.getElementById("postData");

if (postData) {
    $(document).ready(postData.addEventListener('click', async function (event) {
        event.preventDefault();

        var selectedIds = [];

        // Checkbox durumu "checked" olanları bul
        $('input[type="checkbox"]:checked').each(function () {
            // Ürün ID'sini al ve listeye ekle
            var productId = $(this).val();
            selectedIds.push(productId);
        });

        var formData = new FormData();

        // Diğer form verilerini de FormData'ya ekleyebilirsiniz
        formData.append("AddressId", document.getElementById("AddressId").value);
        formData.append("Address", tinymce.get("texteditor").getContent());
        formData.append("Province", document.getElementById("ProvinceId").value);
        formData.append("District", document.getElementById("DistrictId").value);
        formData.append("PostCode", document.getElementById("PostCodeId").value);
        var sum = document.getElementById("TotalPrice").dataset.value;
        var sumFloat = parseFloat(sum.replace(",", "."));

        formData.append("TotalPrice", sumFloat);
        formData.append("DiscountPercent", document.getElementById("DiscountId").value);
        formData.append("UserMail", document.getElementById("mailId").value);

        for (var i = 0; i < selectedIds.length; i++) {
            formData.append('productId', selectedIds[i]);
        }


        var result = await $.ajax({
            type: 'post',
            url: "/Orders/OrderAdd?handler=AddOrder",
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
                title: "Sipariş Oluşturuldu",
                text: "Sipariş başarıyla oluşturuldu.",
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
            });
        }

    }));

}




var cartPost = document.getElementById("cartPost");

if (cartPost) {
    $(document).ready(cartPost.addEventListener('click', async function (event) {
        event.preventDefault();

        const { value: email } = await Swal.fire({
            title: "Kullanıcı Sepet Bilgilerini Getir",
            input: "email",
            inputLabel: "Email Adresi:",
            inputPlaceholder: "Email adresi giriniz",
            showCancelButton: true,
            confirmButtonText: "Getir",
            cancelButtonText: "İptal",
            preConfirm: (value) => {
                if (!value) {
                    Swal.showValidationMessage("Lütfen bir e-posta adresi girin!");
                }
                return value;
            },
        });

        if (email) {
            // C# endpoint'ine AJAX isteği gönder
            var result = await $.ajax({
                method: 'GET',
                url: '/Orders/OrderAdd/?handler=UserCart&email=' + email,
                contentType: 'application/json; charset=utf-8',

                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
                },

            });

            if (result.isSuccess) {
                updateOrderSummaryTable(result.orderSummary); // Implement this function
                checkProductsInDataTable(result.orderSummary);
                //    displaySelectedProducts();
            }
            else {
                await Swal.fire({
                    title: "Başarısız İşlem",
                    text: result.message,
                    icon: "warning"
                });
            }
        }

    }));


}



// Function to update the HTML table with the new data
function updateOrderSummaryTable(orderSummary) {
    orderSummary.forEach(function (item) {
        var newRow = '<tr data-product-id="' + item.productCode + '">' +
            '<td>' +
            '<div class="symbol symbol-50px"><img class="symbol-label" src="/assets/media/ProductAvatar/' + item.avatarLogo + '" /></div>' +
            '</td>' +
            '<td>' + item.productName + '</td>' +
            '<td>' + item.productCode + '</td>' +
            '<td><input type="number" class="form-control" value="' + item.stock + '" onchange="updateTotalPrices()"></td>' + // Stock için input kutusu
            '<td>' + item.productPrice + '</td>' +
            '<td>' + (item.stock * item.productPrice) + '</td>' +
            '</tr>';


        // Toplam tutarı güncelleyin

        // Satırı tabloya ekle
        $("#orderSummaryTable tbody").append(newRow);
        var sum = document.getElementById("TotalPrice").getAttribute("data-value");

        var totalPrice = parseFloat(sum) + (item.stock * item.productPrice);

        $("#TotalPrice").attr("data-value", totalPrice.toFixed(2));
        $("#TotalPrice").text(totalPrice.toFixed(2) + " ₺");
    });


}
// Function to clear existing table rows
function clearOrderSummaryTable() {
    // Get the table element by ID
    var table = document.getElementById("orderSummaryTable");

    // Clear existing rows
    while (table.rows.length > 0) {
        table.deleteRow(0);
    }
}

function checkProductsInDataTable(orderSummary) {
    // Get the DataTable instance
    var dataTable = $('#kt_ecommerce_edit_order_product_table').DataTable();

    // Clear the selected products container
    var selectedProductsContainer = $('#kt_ecommerce_edit_order_selected_products');
    //selectedProductsContainer.empty();

    // Initialize total price
    var totalPrice = 0;

    // Loop through each row in the DataTable
    dataTable.rows().every(function (index, element) {
        // Assuming the checkbox is in the first column (index 0)
        var checkbox = $(this.node()).find('.form-check-input')[0];

        var productId = checkbox.value;

        // Check if the current product ID is in the cartProducts array
        var isProductInCart = orderSummary.some(function (orderItem) {
            return orderItem.productId == productId; // Adjust this based on your actual structure
        });

        // If the product is in the cart, add it to the selected products container
        if (isProductInCart) {
            // Check the checkbox if it's not already checked
            if (!checkbox.checked) {
                checkbox.checked = true;
            }

            // Clone and add the product to the selected products container
            var productRow = $(this.node()).find('[data-kt-ecommerce-edit-order-filter="product"]').clone();
            selectedProductsContainer.append(productRow);

            // Add the price to the total
            var priceElement = productRow.find('[data-kt-ecommerce-edit-order-filter="price"]');
            if (priceElement.length > 0) {
                totalPrice += parseFloat(priceElement.text());
            }
        }

        return true; // Continue to the next row
    });

    // Display the total price
    var totalAmountContainer = $('#kt_ecommerce_edit_order_total_price');
    totalAmountContainer.text(totalPrice.toFixed(2));
}


function openUpdateDialog() {
    // AJAX ile backend'ten dropdown verilerini al
    $.ajax({
        url: '/OrderList?handler=LoadDropdownData', // Bu URL'yi kendi projenize göre güncelleyin
        type: 'GET',
        success: function (data) {
            var dropdownOptions = data.map(option => ({ value: option.value, text: option.text }));

            // SweetAlert2 ile özel formu oluştur
            Swal.fire({
                title: 'Sipariş Durumu Güncelle',
                html: '<select id="statusDropdown" class="swal2-input">' +
                    dropdownOptions.map(option => `<option value="${option.value}">${option.text}</option>`).join('') +
                    '</select>',
                showCancelButton: true,
                confirmButtonText: 'Güncelle',
                cancelButtonText: 'İptal',
                focusConfirm: false,
                preConfirm: () => {
                    // Seçilen değeri al
                    var selectedValue = $('#statusDropdown').val();

                    // Ajax isteği ile seçilen değeri backend'e gönder
                    $.ajax({
                        url: '/your-backend-endpoint', // Backend endpointinizi buraya ekleyin
                        type: 'POST',
                        data: { selectedValue: selectedValue },
                        success: function (response) {
                            // Başarılı cevap
                            Swal.fire('Başarılı', 'Sipariş durumu güncellendi!', 'success');
                        },
                        error: function () {
                            // Hata durumu
                            Swal.fire('Hata', 'Sipariş durumu güncellenirken bir hata oluştu!', 'error');
                        }
                    });
                }
            });
        },
        error: function () {
            // Hata durumu
            Swal.fire('Hata', 'Dropdown verileri alınırken bir hata oluştu!', 'error');
        }
    });
}



document.addEventListener('DOMContentLoaded', function () {
    var postCargo = document.getElementById("postCargo");

    if (postCargo) {


        $(document).ready(postCargo.addEventListener('click', async function (event) {
            event.preventDefault();


            var formData = new FormData();

            // Diğer form verilerini de FormData'ya ekleyebilirsiniz
            formData.append("CargoCode", document.getElementById("CargoCode").value);
            formData.append("StatusId", document.getElementById("StatusId").value);

            var action = $('#postCargoForm').attr('action');


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
                    title: "Sipariş Durumu Güncellendi",
                    text: "Sipariş Başarıyla Güncellendi",
                    icon: "success"
                }).then(function () {
                    window.location.href = result.url;
                });
            }
            else {
                await Swal.fire({
                    title: "Sipariş Durumu Güncellenmedi",
                    text: result.message,
                    icon: "warning"
                });
            }

        }));
    }
});


// Sayfa tamamen yüklendiğinde çalışacak olan kod bloğu
document.addEventListener("DOMContentLoaded", function () {
    // "Getir" butonunu al
    var updateCartPost = document.getElementById("updateCartPost");

    // "Getir" butonuna tıklandığında çalışacak olan işlem
    if (updateCartPost) {
        updateCartPost.addEventListener('click', async function (event) {
            event.preventDefault();


            const { value: cartCode } = await Swal.fire({
                title: "Kullanıcı Sepetini Güncelle",
                input: "text",
                inputLabel: "Sepet Kodu:",
                inputPlaceholder: "Sepet kodu giriniz",
                showCancelButton: true,
                confirmButtonText: "Güncelle",
                cancelButtonText: "İptal",
                preConfirm: (value) => {
                    if (!value) {
                        Swal.showValidationMessage("Lütfen bir sepet kodu giriniz!");
                    }
                    return value;
                },
            });

            if (cartCode) {
                // Tablodaki verileri alma ve model oluşturma
                var products = [];
                var tableRows = document.querySelectorAll('#orderSummaryTable tbody tr');
                tableRows.forEach(function (row) {
                    var productName = row.cells[1].innerText;
                    var productCode = row.cells[2].innerText;
                    var stock = parseFloat(row.cells[3].querySelector('input').value);
                    var product = { ProductName: productName, ProductCode: productCode, Stock: stock };
                    products.push(product);
                });

                // Verileri JSON formatına çevirme
                var jsonData = JSON.stringify(products);

                // AJAX isteğiyle C# tarafına gönderme
                try {
                    var result = await $.ajax({
                        type: "POST",
                        url: "/Orders/OrderAdd/?handler=UpdateShoppingCart&cartCode=" + cartCode,
                        data: jsonData,
                        contentType: 'application/json; charset=utf-8',
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        datatype: 'json',
                    });

                    if (result.isSuccess) {
                        await Swal.fire({
                            title: "Sepet Güncellendi",
                            text: "Sepet başarıyla güncellendi",
                            icon: "success"
                        });
                    } else {
                        await Swal.fire({
                            title: "Sepet Güncellenemedi",
                            text: "Sepet Bulunamadı",
                            icon: "warning"
                        });
                    }
                } catch (error) {
                    await Swal.fire({
                        title: "Hata",
                        text: "Beklenmedik hatalar oluştu.",
                        icon: "warning"
                    });
                }
            }
            
        });
    }
});