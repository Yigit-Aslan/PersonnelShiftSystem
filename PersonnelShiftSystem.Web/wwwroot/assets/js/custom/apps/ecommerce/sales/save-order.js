//"use strict";
//var ktappecommercesalessaveorder = function () {
//    var e, t;
//    return {
//        init: function () {
//            (() => {
//                $("#kt_ecommerce_edit_order_date").flatpickr({
//                    altınput: !0,
//                    altformat: "d f, y",
//                    dateformat: "y-m-d"
//                });
//                const r = e => {
//                    if (!e.id)
//                        return e.text;
//                    var t = document.createelement("span")
//                        , r = "";
//                    return r += '<img src="' + e.element.getattribute("data-kt-select2-country") + '" class="rounded-circle h-20px me-2" alt="image"/>',
//                        r += e.text,
//                        t.innerhtml = r,
//                        $(t)
//                }
//                    ;
//                $("#kt_ecommerce_edit_order_billing_country").select2({
//                    placeholder: "select a country",
//                    minimumresultsforsearch: 1 / 0,
//                    templateselection: r,
//                    templateresult: r
//                }),
//                    $("#kt_ecommerce_edit_order_shipping_country").select2({
//                        placeholder: "select a country",
//                        minimumresultsforsearch: 1 / 0,
//                        templateselection: r,
//                        templateresult: r
//                    }),
//                    e = document.queryselector("#kt_ecommerce_edit_order_product_table"),
//                    t = $(e).datatable({
//                        dom: 'lbfrtip',
//                        order: [],
//                        scrollcollapse: false,
//                        paging: true,
//                        "drawcallback": function () {
//                            $('.datatables_paginate > .pagination').addclass('custom-pagination pagination-simple');
//                        },
//                        info: false,
//                        ordering: true,
//                        language: {
//                            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json'
//                        },
//                        initcomplete: function () {


//                            $('.dt-buttons').appendto($('#datable-device_wrapper'));

//                        },
//                        buttons: [
//                            {
//                                extend: 'copyhtml5',
//                                exportoptions: {
//                                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
//                                }
//                            },
//                            {
//                                extend: 'excelhtml5',
//                                exportoptions: {
//                                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
//                                }
//                            },
//                            {
//                                extend: 'pdfhtml5',
//                                exportoptions: {
//                                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
//                                }
//                            },


//                        ],
//                    })
//            }
//            )(),
//                document.queryselector('[data-kt-ecommerce-edit-order-filter="search"]').addeventlistener("input", (function (event) {
//                    t.search(event.target.value).draw();
//                }
//                )),
//                (() => {
//                    const e = document.getelementbyıd("kt_ecommerce_edit_order_shipping_form");
//                document.getelementbyıd("same_as_billing").addeventlistener("change", (t => {
//                    t.target.checked ? e.classlist.add("d-none") : e.classlist.remove("d-none")
//                    }
//                    ))
//                }
//                )(),
//                (() => {
//                    const t = e.queryselectorall('[type="checkbox"]')
//                        , r = document.getelementbyıd("kt_ecommerce_edit_order_selected_products")
//                        , o = document.getelementbyıd("kt_ecommerce_edit_order_total_price");
//                    t.foreach((e => {
//                        e.addeventlistener("change", (t => {
//                            const o = e.closest("tr").queryselector('[data-kt-ecommerce-edit-order-filter="product"]').clonenode(!0)
//                                , i = document.createelement("div")
//                                , n = o.innerhtml
//                                , a = ["d-flex", "align-items-center"];
//                            o.classlist.remove(...a),
//                                o.classlist.add("col", "my-2"),
//                                o.innerhtml = "",
//                                i.classlist.add(...a),
//                                i.classlist.add("border", "border-dashed", "rounded", "p-3", "bg-body"),
//                                i.innerhtml = n,
//                                o.appendchild(i);

//                            const c = o.getattribute("data-kt-ecommerce-edit-order-id");
//                            if (t.target.checked)
//                                r.appendchild(o);
//                            else {
//                                const e = r.queryselector('[data-kt-ecommerce-edit-order-id="' + c + '"]');
//                                e && r.removechild(e)
//                            }
//                            d()
//                        }
//                        ))
//                    }
//                    ));
//                    const d = () => {
//                        const e = r.queryselector("span")
//                            , t = r.queryselectorall('[data-kt-ecommerce-edit-order-filter="product"]');
//                        t.length < 1 ? (e.classlist.remove("d-none"),
//                            o.innertext = "0.00") : (e.classlist.add("d-none"),
//                                i(t))
//                    }
//                        , i = e => {
//                            let t = 0;
//                            e.foreach((e => {
//                                const r = parsefloat(e.queryselector('[data-kt-ecommerce-edit-order-filter="price"]').innertext);
//                                t = parsefloat(t + r)
//                            }
//                            )),
//                                o.innertext = t.tofixed(2)
//                        }
//                }
//                )()


//        }
//    }
//}();
//ktutil.ondomcontentloaded((function () {
//    ktappecommercesalessaveorder.init()
//}
//));




"use strict";

var KTAppEcommerceSalesSaveOrder = function () {
    var dataTable, selectedProductsContainer, totalAmountContainer, checkboxes;

    return {
        init: function () {
            (() => {
                $("#kt_ecommerce_edit_order_date").flatpickr({
                    altInput: true,
                    altFormat: "d F, Y",
                    dateFormat: "Y-m-d"
                });

                const templateSelection = (e) => {
                    if (!e.id)
                        return e.text;

                    var t = document.createElement("span"),
                        r = "";
                    r += '<img src="' + e.element.getAttribute("data-kt-select2-country") + '" class="rounded-circle h-20px me-2" alt="image"/>';
                    r += e.text;
                    t.innerHTML = r;

                    return $(t);
                };

                $("#kt_ecommerce_edit_order_billing_country").select2({
                    placeholder: "Select a country",
                    minimumResultsForSearch: 1 / 0,
                    templateSelection: templateSelection,
                    templateResult: templateSelection
                });

                $("#kt_ecommerce_edit_order_shipping_country").select2({
                    placeholder: "Select a country",
                    minimumResultsForSearch: 1 / 0,
                    templateSelection: templateSelection,
                    templateResult: templateSelection
                });

                dataTable = $("#kt_ecommerce_edit_order_product_table").DataTable({
                    dom: 'lBfrtip',
                    order: [],
                    scrollCollapse: false,
                    paging: true,
                    "drawCallback": function () {
                        $('.dataTables_paginate > .pagination').addClass('custom-pagination pagination-simple');
                    },
                    info: false,
                    ordering: true,
                    language: {
                        url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json'
                    },
                    initComplete: function () {
                        $('.dt-buttons').appendTo($('#datable-device_wrapper'));
                    },
                    buttons: [
                        {
                            extend: 'copyHtml5',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
                            }
                        },
                        {
                            extend: 'excelHtml5',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
                            }
                        },
                        {
                            extend: 'pdfHtml5',
                            exportOptions: {
                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
                            }
                        },
                    ],
                });
            })();

            document.querySelector('[data-kt-ecommerce-edit-order-filter="search"]').addEventListener("input", (function (event) {
                dataTable.search(event.target.value).draw();
            }));

            (() => {
                const shippingForm = document.getElementById("kt_ecommerce_edit_order_shipping_form");
                document.getElementById("same_as_billing").addEventListener("change", (event => {
                    event.target.checked ? shippingForm.classList.add("d-none") : shippingForm.classList.remove("d-none");
                }));
            })();

            (() => {
                checkboxes = dataTable.$('[type="checkbox"]');
                selectedProductsContainer = document.getElementById("kt_ecommerce_edit_order_selected_products");
                totalAmountContainer = document.getElementById("kt_ecommerce_edit_order_total_price");

                checkboxes.each((index, checkbox) => {
                    checkbox.addEventListener("change", (event => {
                        const productRow = checkbox.closest("tr").querySelector('[data-kt-ecommerce-edit-order-filter="product"]').cloneNode(true);
                        const productDiv = document.createElement("div");
                        const productHTML = productRow.innerHTML;
                        const classesToRemove = ["d-flex", "align-items-center"];

                        productRow.classList.remove(...classesToRemove);
                        productRow.classList.add("col", "my-2");
                        productRow.innerHTML = "";

                        productDiv.classList.add(...classesToRemove);
                        productDiv.classList.add("border", "border-dashed", "rounded", "p-3", "bg-body");
                        productDiv.innerHTML = productHTML;
                        productRow.appendChild(productDiv);

                        const productId = productRow.getAttribute("data-kt-ecommerce-edit-order-id");
                        if (event.target.checked)
                            selectedProductsContainer.appendChild(productRow);
                        else {
                            const existingProduct = selectedProductsContainer.querySelector('[data-kt-ecommerce-edit-order-id="' + productId + '"]');
                            existingProduct && selectedProductsContainer.removeChild(existingProduct);
                        }

                        // Güncellenmiş fiyatı hesapla ve göster
                        updateTotalAmount();
                    }));
                });

                // Otomatik olarak işaretli checkbox'ları kontrol et
                checkboxes.each((index, checkbox) => {
                    if (checkbox.checked) {
                        checkbox.dispatchEvent(new Event('change'));
                    }
                });

                var checkedProducts;
                //DataTable'a veri ekleme
                checkboxes.each((index, checkbox) => {
                    checkbox.addEventListener("change", (event => {
                        var productCode = event.target.dataset.productCode; // Checkbox'un dataset özelliğini kullanarak data-product-code değerini al
                        var rowToRemove = document.querySelector("tr[data-product-id='" + productCode + "']");
                        if (event.target.checked) {
                            // Checkbox checked olduğunda değeri diziye ekle
                            checkedProducts = event.target.value;
                            // AJAX isteğini gönderin
                            sendAjaxRequest();
                        }
                        else {
                            if (rowToRemove) {

                                rowToRemove.parentNode.removeChild(rowToRemove); // Satırı DOM'dan kaldır
                                updateTableAmount();
                            }
                        }
                    }));
                });

                function updateTableAmount() {
                    // Tablodan tüm satırları seç
                    var rows = document.querySelectorAll("#orderSummaryTable tbody tr");

                    // Toplam fiyatı tutacak değişkeni tanımla
                    var totalPrice = 0;

                    // Her bir satır için döngü
                    rows.forEach(function (row) {
                        // Satırdaki stock ve productPrice değerlerini al
                        var stock = parseFloat(row.cells[3].innerText); // 4. sütun (0'dan başlayarak) stock değerini içerir
                        var productPrice = parseFloat(row.cells[4].innerText); // 5. sütun productPrice değerini içerir

                        // Satırın toplam fiyatını hesapla ve totalPrice değişkenine ekle
                        totalPrice += stock * productPrice;
                    });

                    // Toplam fiyatı HTML içine yerleştir
                    var totalPriceElement = document.getElementById("TotalPrice");
                    totalPriceElement.innerText = totalPrice.toFixed(2) + " ₺"; // Toplam fiyatı topluca ayarla
                    totalPriceElement.setAttribute("data-value", totalPrice.toFixed(2)); // Veriyi depolamak için data-value özelliğini güncelle
                }
                // Güncellenmiş toplam fiyat fonksiyonu
                function updateTotalAmount() {
                    const amountElement = selectedProductsContainer.querySelector("span");
                    const productElements = selectedProductsContainer.querySelectorAll('[data-kt-ecommerce-edit-order-filter="product"]');

                    // Sıfırla
                    let totalPrice = 0;

                    // Tüm seçili ürünleri kontrol et ve fiyatları topla
                    productElements.forEach((element => {
                        const price = parseFloat(element.querySelector('[data-kt-ecommerce-edit-order-filter="price"]').innerText);
                        totalPrice = parseFloat(totalPrice + price);
                    }));

                    // Göster
                    totalAmountContainer.innerText = totalPrice.toFixed(2);
                }

                function sendAjaxRequest() {
                    var result = $.ajax({
                        type: "GET",
                        url: "/Orders/OrderAdd?handler=TableData&stockId=" + checkedProducts, // Sunucu tarafındaki metodu çağırın
                        contentType: "application/json", // Gönderilen verinin tipini belirtin
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (response) {
                            if (response.result === "Success") {
                                var newRow = '<tr data-product-id="' + response.productModel.productCode + '">' +
                                    '<td>' +
                                    '<div class="symbol symbol-50px"><img class="symbol-label" src="/assets/media/ProductAvatar/' + response.productModel.avatarLogo + '" /></div>' +
                                    '</td>' +
                                    '<td>' + response.productModel.productName + '</td>' +
                                    '<td>' + response.productModel.productCode + '</td>' +
                                    '<td><input type="number" class="form-control" value="' + response.productModel.stock + '" onchange="updateTotalPrices()"></td>' + // Stock için input kutusu
                                    '<td>' + response.productModel.productPrice + '</td>' +
                                    '<td>' + (response.productModel.stock * response.productModel.productPrice) + '</td>' +
                                    '</tr>';


                                // Toplam tutarı güncelleyin

                                // Satırı tabloya ekle
                                $("#orderSummaryTable tbody").append(newRow);
                                var sum = document.getElementById("TotalPrice").getAttribute("data-value");

                                var totalPrice = parseFloat(sum) + (response.productModel.stock * response.productModel.productPrice);

                                $("#TotalPrice").attr("data-value", totalPrice.toFixed(2));
                                $("#TotalPrice").text(totalPrice.toFixed(2) + " ₺");

                               
                            }
                        },
                        error: function (xhr, status, error) {
                            // Başarısız yanıt geldiğinde bir hata mesajı gösterebilirsiniz
                            console.error(result.message);
                        }
                    });
                }
            })();
        },

        updateTotalAmount: function () {
            const amountElement = selectedProductsContainer.querySelector("span");
            const productElements = selectedProductsContainer.querySelectorAll('[data-kt-ecommerce-edit-order-filter="product"]');

            if (productElements.length < 1) {
                amountElement.classList.remove("d-none");
                totalAmountContainer.innerText = "0.00";
            } else {
                amountElement.classList.add("d-none");
                this.calculateTotal(productElements);
            }

            // Eğer ürün yoksa, fiyat bilgisini sıfırla
            if (productElements.length === 0) {
                totalAmountContainer.innerText = "0.00";
            }
        },

        calculateTotal: function (productElements) {
            let total = 0;
            productElements.forEach((element => {
                const price = parseFloat(element.querySelector('[data-kt-ecommerce-edit-order-filter="price"]').innerText);
                total = parseFloat(total + price);
            }));

            totalAmountContainer.innerText = total.toFixed(2);
        }
    }
}();

KTUtil.onDOMContentLoaded((function () {
    KTAppEcommerceSalesSaveOrder.init()
}));



//EN DOĞRUSU BU

//"use strict";

//var KTAppEcommerceSalesSaveOrder = function () {
//    var dataTable, selectedProductsContainer, totalAmountContainer, checkboxes;

//    // Fonksiyonu burada tanımlayabilir veya ayrı bir dosyada tutabilirsiniz
//    function displaySelectedProducts(selectedProducts) {
//        var selectedProductsContainer = $('#selectedProductsContainer');
//        selectedProductsContainer.empty();

//        for (var i = 0; i < selectedProducts.length; i++) {
//            var product = selectedProducts[i];
//            var listItem = $('<li>').text(product.productName);
//            selectedProductsContainer.append(listItem);
//        }
//    }

//    return {
//        init: function () {
//            (() => {
//                $("#kt_ecommerce_edit_order_date").flatpickr({
//                    altInput: true,
//                    altFormat: "d F, Y",
//                    dateFormat: "Y-m-d"
//                });

//                const templateSelection = (e) => {
//                    if (!e.id)
//                        return e.text;

//                    var t = document.createElement("span"),
//                        r = "";
//                    r += '<img src="' + e.element.getAttribute("data-kt-select2-country") + '" class="rounded-circle h-20px me-2" alt="image"/>';
//                    r += e.text;
//                    t.innerHTML = r;

//                    return $(t);
//                };

//                $("#kt_ecommerce_edit_order_billing_country").select2({
//                    placeholder: "Select a country",
//                    minimumResultsForSearch: 1 / 0,
//                    templateSelection: templateSelection,
//                    templateResult: templateSelection
//                });

//                $("#kt_ecommerce_edit_order_shipping_country").select2({
//                    placeholder: "Select a country",
//                    minimumResultsForSearch: 1 / 0,
//                    templateSelection: templateSelection,
//                    templateResult: templateSelection
//                });

//                dataTable = $("#kt_ecommerce_edit_order_product_table").DataTable({
//                    dom: 'lBfrtip',
//                    order: [],
//                    scrollCollapse: false,
//                    paging: true,
//                    "drawCallback": function () {
//                        $('.dataTables_paginate > .pagination').addClass('custom-pagination pagination-simple');
//                    },
//                    info: false,
//                    ordering: true,
//                    language: {
//                        url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json'
//                    },
//                    initComplete: function () {
//                        $('.dt-buttons').appendTo($('#datable-device_wrapper'));
//                    },
//                    buttons: [
//                        {
//                            extend: 'copyHtml5',
//                            exportOptions: {
//                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
//                            }
//                        },
//                        {
//                            extend: 'excelHtml5',
//                            exportOptions: {
//                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
//                            }
//                        },
//                        {
//                            extend: 'pdfHtml5',
//                            exportOptions: {
//                                columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
//                            }
//                        },
//                    ],
//                });
//            })();

//            document.querySelector('[data-kt-ecommerce-edit-order-filter="search"]').addEventListener("input", (function (event) {
//                dataTable.search(event.target.value).draw();
//            }));

//            (() => {
//                const shippingForm = document.getElementById("kt_ecommerce_edit_order_shipping_form");
//                document.getElementById("same_as_billing").addEventListener("change", (event => {
//                    event.target.checked ? shippingForm.classList.add("d-none") : shippingForm.classList.remove("d-none");
//                }));
//            })();

//            (() => {
//                checkboxes = dataTable.$('[type="checkbox"]');
//                selectedProductsContainer = $('#kt_ecommerce_edit_order_selected_products');
//                totalAmountContainer = $('#kt_ecommerce_edit_order_total_price');

//                checkboxes.each((index, checkbox) => {
//                    checkbox.addEventListener("change", (event => {
//                        const productRow = checkbox.closest("tr").querySelector('[data-kt-ecommerce-edit-order-filter="product"]').cloneNode(true);
//                        const productDiv = document.createElement("div");
//                        const productHTML = productRow.innerHTML;
//                        const classesToRemove = ["d-flex", "align-items-center"];

//                        productRow.classList.remove(...classesToRemove);
//                        productRow.classList.add("col", "my-2");
//                        productRow.innerHTML = "";

//                        productDiv.classList.add(...classesToRemove);
//                        productDiv.classList.add("border", "border-dashed", "rounded", "p-3", "bg-body");
//                        productDiv.innerHTML = productHTML;
//                        productRow.appendChild(productDiv);

//                        const productId = productRow.getAttribute("data-kt-ecommerce-edit-order-id");
//                        if (event.target.checked)
//                            selectedProductsContainer.append(productRow);
//                        else {
//                            const existingProduct = selectedProductsContainer.find('[data-kt-ecommerce-edit-order-id="' + productId + '"]');
//                            existingProduct && existingProduct.remove();
//                        }
//                        this.updateTotalAmount();
//                    }));
//                });

//                // Otomatik olarak işaretli checkbox'ları kontrol et
//                checkboxes.each((index, checkbox) => {
//                    if (checkbox.checked) {
//                        checkbox.dispatchEvent(new Event('change'));
//                    }
//                });
//            })();
//        },

//        updateTotalAmount: function () {
//            const amountElement = selectedProductsContainer.find("span");
//            const productElements = selectedProductsContainer.find('[data-kt-ecommerce-edit-order-filter="product"]');

//            if (productElements.length < 1) {
//                amountElement.removeClass("d-none");
//                totalAmountContainer.text("0.00");
//            } else {
//                amountElement.addClass("d-none");
//                this.calculateTotal(productElements);
//            }
//        },

//        calculateTotal: function (productElements) {
//            let total = 0;
//            productElements.each((index, element) => {
//                const price = parseFloat($(element).find('[data-kt-ecommerce-edit-order-filter="price"]').text());
//                total = parseFloat(total + price);
//            });

//            totalAmountContainer.text(total.toFixed(2));
//        }
//    }
//}();

//KTUtil.onDOMContentLoaded((function () {
//    KTAppEcommerceSalesSaveOrder.init()
//}));




