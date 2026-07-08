
function closeTopAlert() {
    const alert = document.getElementById("topAlert");
    if (alert) {
        alert.classList.remove("show");

        setTimeout(() => {
            alert.remove();
        }, 200);
    }
}

// auto close after 3 seconds
document.addEventListener("DOMContentLoaded", function () {
    setTimeout(() => {
        closeTopAlert();
    }, 3000);
});
