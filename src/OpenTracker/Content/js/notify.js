function showCustomMessage(msg) {
    $.notifyBar({
        html: msg,
        delay: 2000,
        animationSpeed: "fast"
    });
}



function showSuccess(msg) {
    $.notifyBar({
        html: msg,
        cls: "success",
        delay: 4000,
        animationSpeed: "normal"
    });
}



function showError(msg) {
    $.notifyBar({
        html: msg,
        cls: "error",
        delay: 4000,
        animationSpeed: "normal"
    });
}