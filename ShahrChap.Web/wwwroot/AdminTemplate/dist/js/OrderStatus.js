
$(function () {
    var el = document.getElementById('sortable-body');
    Sortable.create(el, {
        handle: '.handle',
        animation: 150,
        onEnd: function () {
            var ids = [];
            $('#sortable-body tr').each(function () {
                ids.push(parseInt($(this).data('id')));
            });

            $.ajax({
                url: '?handler=UpdateOrder',
                type: 'POST',
                contentType: 'application/json',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                data: JSON.stringify(ids),
                success: function (res) {
                    console.log("ترتیب ذخیره شد");
                }
            });
        }
    });
});