$(function () {
    Common.FormInlineValidationSetup('importForm');

    $('#formAlert a').click(function () {
        $('#formAlert').hide(200);
    });

    if ($('#formAlert').text().trim() != 'x') {
        $('#formAlert').show();
    }

    $('#formInfo a').click(function () {
        $('#formInfo').hide(200);
    });

    if ($('#formInfo').text().trim() != 'x') {
        $('#formInfo').show();
    }
});