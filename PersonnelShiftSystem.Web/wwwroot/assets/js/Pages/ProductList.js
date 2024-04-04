async function deleteItem() {
    // Burada AJAX veya başka bir yöntemle silme işlemini gerçekleştirin
    // Örneğin, jQuery AJAX kullanarak:

    var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
    var headers = {
        'RequestVerificationToken': antiForgeryToken
    };

    var action = '/Products/ProductList?handler=DeleteProduct';

    var formData = new FormData();
    formData.append("ProductCode", document.getElementById("ProductCode").value);

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
    document.getElementById('ProductCode').value = itemId;

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



        //success: function (result) {
        //    //console.log("Success Response: ", response);
        //    if (result.isSuccess) {
        //        Swal.fire('Silindi!', 'Öğe başarıyla silindi.', 'success');
        //        window.location.reload();
        //    }
        //    else {
        //        Swal.fire('Hata!', response.Message || 'Bir hata oluştu.', 'error');

        //    }

        //},
        //error: function (xhr, status, error) {
        //    console.log("XHR:", xhr);
        //    console.log("Error: ", error);
        //    console.log("status: ", status);
        //    Swal.fire('Hata!', 'Bir hata oluştu.', 'error');
        //}


$(document).ready(async function () {
    $('.update-stock-link').on('click', async function (e) {
        e.preventDefault();
        const productCode = $(this).data('product-code');
        const currentStock = $(this).data('stock');
        await showUpdateStockAlert(productCode, currentStock);
    });

    async function showUpdateStockAlert(productCode, currentStock) {
        Swal.fire({
            title: 'Stok Güncelleme',
            html: `<input type="number" id="newStock" placeholder="Yeni Stok Miktarı" class="swal2-input" value="${currentStock}">`,
            showCancelButton: true,
            confirmButtonText: 'Güncelle',
            cancelButtonText: 'İptal',
            preConfirm: () => {
                const newStock = $('#newStock').val();
                if (!newStock || isNaN(newStock)) {
                    Swal.showValidationMessage('Geçerli bir stok miktarı giriniz.');
                }
                return newStock;
            }
        }).then((result) => {
            if (result.isConfirmed) {
                const updatedStock = result.value;
                updateStockOnServer(productCode, updatedStock);
            }
        });
    }

    async function updateStockOnServer(productCode, updatedStock) {
        var formData = new FormData();
        formData.append("ProductCode", productCode);
        formData.append("Stock", updatedStock);
       var result = await $.ajax({
            type: 'post',
            url: '/Products/ProductList?handler=UpdateStock',
            data: formData,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            processData: false,
            contentType: false,
            dataType: 'json',
       });
        if (result.isSuccess) {
            Swal.fire('Başarılı!', 'Stok güncellendi: ' + updatedStock, 'success');
            window.location.reload();
        }
        else {
            Swal.fire('Hata!', 'Lütfen geçerli stok değeri giriniz.', 'error');

        }
    }
});


$(document).ready(function () {
    $('.add-size-link').on('click', function (e) {
        e.preventDefault();

        const productId = $(this).data('product-id');
        showUpdateStockAlert(productId);
    });

    function showUpdateStockAlert(productId) {
        // AJAX isteği göndererek dropdown için gerekli verileri backend'den alıyoruz
        $.ajax({
            type: 'GET',
            url: '/Products/ProductList?handler=Sizes&productId=' + productId, // Örnek bir endpoint
            dataType: 'json',
            success: function (response) {
                // Başarılı bir şekilde verileri aldıktan sonra Swal içerisinde dropdown oluşturuyoruz
                var sizeOptions = '';
                response.sizeModel.forEach(size => {
                    if (size.id == 0)
                        sizeOptions += `<option disabled value="${size.id}">${size.sizeName} - Kayıtlı</option>`;
                    else
                        sizeOptions += `<option value="${size.id}">${size.sizeName}</option>`;
                });
                Swal.fire({
                    title: 'Yeni Beden Ekleme',
                    html: `
            <select id="sizeDropdown" class="swal2-select">
                ${sizeOptions}
            </select>
            <input type="number" id="newStock" placeholder="Yeni Stok Miktarı" class="swal2-input">
        `,
                    showCancelButton: true,
                    confirmButtonText: 'Güncelle',
                    cancelButtonText: 'İptal',
                    preConfirm: () => {
                        const newStock = $('#newStock').val();
                        if (!newStock || isNaN(newStock)) {
                            Swal.showValidationMessage('Geçerli bir stok miktarı giriniz.');
                            return false;
                        }
                        const selectedSizeId = $('#sizeDropdown').val();
                        return { sizeId: selectedSizeId, stock: newStock };
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        const { sizeId, stock } = result.value;
                        updateStockOnServer(productId, sizeId, stock);
                    }
                });
            },
            error: function (error) {
                Swal.fire('Hata!', 'Beden eklenirken bir hata oluştu.', 'error');
            }
        });
    }

    function updateStockOnServer(productId, sizeId, updatedStock) {
        var formData = new FormData();
        formData.append("ProductId", productId);
        formData.append("SizeId", sizeId);
        formData.append("Stock", updatedStock);
        $.ajax({
            type: 'post',
            url: '/Products/ProductList?handler=AddSize',
            data: formData,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            processData: false,
            contentType: false,
            dataType: 'json',
            success: function (response) {
                Swal.fire('Başarılı!','Yeni Beden Eklendi ','success');
                window.location.reload();
            },
            error: function (error) {
                Swal.fire('Hata!', 'Beden Ekleme sırasında bir hata oluştu.', 'error');
            }
        });
    }
});
    

