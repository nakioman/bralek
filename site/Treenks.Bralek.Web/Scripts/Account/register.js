﻿$(function() {
    Common.FormInlineValidationSetup('registerForm');

    $('#formAlert a').click(function() {
        $('#formAlert').hide(200);
    });

    if ($('#formAlert').text().trim() != 'x') {
        $('#formAlert').show();
    }
})