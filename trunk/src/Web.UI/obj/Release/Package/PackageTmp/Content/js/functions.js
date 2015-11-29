function onFocusCheckEntry(which, content) {
    if (which.value == content) {
        which.value = "";
        $(which).removeClass("blurredEntry");
    }
}

function onBlurCheckEntry(which, content) {
    if (which.value == "") {
        which.value = content;
        $(which).addClass("blurredEntry");
    }
}

function disableEnterKey(e) {
    var key;
    if (window.event)
        key = window.event.keyCode; //IE
    else
        key = e.which; //firefox     

    return (key != 13);
}