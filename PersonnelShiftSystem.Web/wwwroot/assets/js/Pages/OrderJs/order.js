"use strict";
var KTAppEcommerceProducts = function () {
    var t, e, n = () => {
        t.querySelectorAll('[data-kt-ecommerce-order-filter="delete_row"]').forEach((t => {
            t.addEventListener("click", (function (t) {
                t.preventDefault();
                const n = t.target.closest("tr")
                    , r = n.querySelector('[data-kt-ecommerce-order-filter="product_name"]').innerText;
                Swal.fire({
                    text: "Are you sure you want to delete " + r + "?",
                    icon: "warning",
                    showCancelButton: !0,
                    buttonsStyling: !1,
                    confirmButtonText: "Yes, delete!",
                    cancelButtonText: "No, cancel",
                    customClass: {
                        confirmButton: "btn fw-bold btn-danger",
                        cancelButton: "btn fw-bold btn-active-light-primary"
                    }
                }).then((function (t) {
                    t.value ? Swal.fire({
                        text: "You have deleted " + r + "!.",
                        icon: "success",
                        buttonsStyling: !1,
                        confirmButtonText: "Ok, got it!",
                        customClass: {
                            confirmButton: "btn fw-bold btn-primary"
                        }
                    }).then((function () {
                        e.row($(n)).remove().draw()
                    }
                    )) : "cancel" === t.dismiss && Swal.fire({
                        text: r + " was not deleted.",
                        icon: "error",
                        buttonsStyling: !1,
                        confirmButtonText: "Ok, got it!",
                        customClass: {
                            confirmButton: "btn fw-bold btn-primary"
                        }
                    })
                }
                ))
            }
            ))
        }
        ))
    }
        ;
    return {
        init: function () {
            let e, t;

            // DataTables başlatma
            t = document.querySelector("#kt_ecommerce_order_table");
            if (t) {
                e = $(t).DataTable({
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
                                columns: [0, 1, 2, 3, 4, 5, 6]
                            }
                        },
                        {
                            extend: 'excelHtml5',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6]
                            }
                        },
                        {
                            extend: 'pdfHtml5',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6]
                            }
                        },


                    ],
                });
                $('.dataTables_filter').css('display', 'none');

                // Arama kutusuna değer girildiğinde
                const searchInput = document.querySelector('[data-kt-ecommerce-order-filter="search"]');
                searchInput.addEventListener("input", function (event) {
                    // Arama modunu "smart" olarak ayarla
                    e.search(event.target.value).draw();
                });




            }
        }

    }
}();
KTUtil.onDOMContentLoaded((function () {
    KTAppEcommerceProducts.init()
}
));
