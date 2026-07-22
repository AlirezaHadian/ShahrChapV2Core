$(document).ready(function () {

    // ==============================
    // ارسال فرم (هم برای کامنت جدید، هم برای ویرایش)
    // ==============================
    $(document).on('submit', '#comment-form', function (e) {
        e.preventDefault();

        const productId = $('#comment-product-id').val();
        const parentId = $('#comment-parent-id').val();
        const editId = $('#comment-edit-id').val();
        const text = $('#comment-text').val();

        const isEditMode = editId !== '';

        const url = isEditMode ? '/Product/EditComment' : '/Product/CreateComment';
        const data = isEditMode
            ? { CommentID: editId, Text: text }
            : { ProductID: productId, ParentID: parentId || null, Text: text };

        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            success: function (html) {
                $('#listComment').html(html);
                resetCommentForm();
            },
            error: function (xhr) {
                const message = xhr.responseText || 'خطایی رخ داد، دوباره تلاش کنید.';
                Swal.fire('خطا', message, 'error');
            }
        });
    });

    function resetCommentForm() {
        $('#comment-text').val('');
        $('#comment-parent-id').val('');
        $('#comment-edit-id').val('');
        $('#reply-context').hide();
        $('#edit-context').hide();
        $('#comment-submit-text').text('ارسال دیدگاه');
    }

    // ==============================
    // باز کردن فرم در حالت "پاسخ"
    // ==============================
    $(document).on('click', '.open-reply-form', function () {
        const id = $(this).data('id');
        const name = $(this).data('name');

        $('#comment-edit-id').val('');
        $('#edit-context').hide();
        $('#comment-submit-text').text('ارسال دیدگاه');

        $('#comment-parent-id').val(id);
        $('#reply-context-name').text(name);
        $('#reply-context').show();

        scrollToCommentForm();
    });

    $(document).on('click', '#cancel-reply', function () {
        $('#comment-parent-id').val('');
        $('#reply-context').hide();
    });

    // ==============================
    // باز کردن فرم در حالت "ویرایش"
    // ==============================
    $(document).on('click', '.btn-edit-comment', function () {
        const id = $(this).data('id');
        const text = $(this).data('text');

        $('#comment-parent-id').val('');
        $('#reply-context').hide();

        $('#comment-edit-id').val(id);
        $('#comment-text').val(text);
        $('#edit-context').show();
        $('#comment-submit-text').text('ذخیره‌ی ویرایش');

        scrollToCommentForm();
    });

    $(document).on('click', '#cancel-edit', function () {
        resetCommentForm();
    });

    function scrollToCommentForm() {
        $('html, body').animate(
            { scrollTop: $('#comment-form').offset().top - 100 },
            300
        );
        $('#comment-text').focus();
    }

    // ==============================
    // حذف کامنت (soft delete، با تایید + انیمیشن)
    // ==============================
    $(document).on('click', '.btn-delete-comment', function () {
        const commentId = $(this).data('id');
        const productId = $('#comment-product-id').val();
        const $commentItem = $('#comment-' + commentId);

        Swal.fire({
            title: 'حذف دیدگاه؟',
            text: 'این دیدگاه حذف خواهد شد.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'بله، حذف شود',
            cancelButtonText: 'انصراف'
        }).then((result) => {
            if (result.isConfirmed) {
                $commentItem.addClass('removing');

                setTimeout(function () {
                    $.ajax({
                        url: '/Product/DeleteComment',
                        type: 'POST',
                        data: { commentId: commentId, productId: productId },
                        success: function (html) {
                            $('#listComment').html(html);
                        },
                        error: function () {
                            Swal.fire('خطا', 'حذف با مشکل مواجه شد', 'error');
                            $commentItem.removeClass('removing');
                        }
                    });
                }, 280);
            }
        });
    });

});
