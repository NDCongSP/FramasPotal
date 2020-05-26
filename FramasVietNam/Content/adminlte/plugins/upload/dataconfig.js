function focusConfigTextarea(className) {
    setTimeout(function () {
        $("." + className).focus();
    }, 100);
}

function toggleHtmlConfigTextarea(className) {
    var id = $("." + className).attr("id");
    for (var i = tinymce.editors.length - 1; i > -1; i--) {
        var ed_id = tinymce.editors[i].id;

        if (id != ed_id)
            tinyMCE.execCommand("mceRemoveEditor", true, ed_id);
    }

    tinyMCE.execCommand('mceToggleEditor', false, id);
}

function initSaveConfigConfirm() {
    $('[data-save=confirm]').confirmation({
        singleton: true,
        popout: true,
        placement: "left",
        trigger: "click",
        title: "Để lưu thay đổi, hệ thống cần thoát ra rồi đăng nhập lại. Bạn có chắc muốn lưu những thay đổi không?",
        btnOkLabel: "<i class='icon-ok-sign icon-white'></i> Có, lưu lại",
        btnCancelLabel: "<i class='icon-remove-sign icon-white'></i> Không, trở về",
        btnOkClass: "btn-success",
        btnCancelClass: 'btn-danger',
        onConfirm: function () { },
        onCancel: function () { }
    });
}

function autoHeightTextarea() {
    $.each($('textarea.auto-height'), function () {
        $(this).on('keyup input focus', function () { autoResizeTextarea(this); });
    });
}

function autoResizeTextarea(e) {
    var offset = e.offsetHeight - e.clientHeight;
    $(e).css('height', 'auto').css('height', e.scrollHeight + offset);
}

function autoInitHtmlEditor() {
    var folder = $(".tinyeditor").data("folder");

    if (!folder || folder == "")
        folder = "/FileUploads/Commons";

    try {
        tinyMCEInit(".tinyeditor", folder);
    } catch (e) {}
};

$(function () {
    autoHeightTextarea();
    initSaveConfigConfirm();
    autoInitHtmlEditor();

    try {
        var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
        pageRequestManager.add_endRequest(function () {
            autoHeightTextarea();
            initSaveConfigConfirm();
            autoInitHtmlEditor();
        });
    } catch (e) {}
});

