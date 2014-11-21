$(document).ready(function () {
    $(".categories-wrap").delegate(".category:last-child", "change", function (e) {
        if ($(this).val() != "") {
            $("#CategoryId").val($(this).val());
        }
    });
})