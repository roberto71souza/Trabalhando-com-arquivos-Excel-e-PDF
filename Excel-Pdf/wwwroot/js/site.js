$('.drop-icon > img').on('click', function () {
    var dataImg = $(this).data('click'),
        dropOptions = $('.drop-option');

    dropOptions.each(function () {
        if ($(this).data('option') === dataImg) {
            $(this).toggleClass('active');
        } else {
            $(this).removeClass('active');
        }
    });

});

$(document).on('change', 'input#inputImportar', function () {

    var formularioFile = $('#formImportarExcel');
    formularioFile.submit();
    console.log('Entrou');

});

function openModal() {
    var modal = $('#exampleModalCenter');
    $.ajax({ url: '/open-modal-adicionar-produto', method: 'GET' })
        .done(function (response) {
            var modalBody = modal.find('.modal-body');
            modalBody.html(response);
            modal.modal('show');
        })
        .fail(function (error) {
            console.log(error);
            modal.modal('hide');
        });
}
