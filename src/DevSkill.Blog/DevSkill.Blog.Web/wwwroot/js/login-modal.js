document.addEventListener("DOMContentLoaded", function () {

    const modals = document.querySelectorAll(".modal");

    modals.forEach(function (modal) {

        modal.addEventListener("shown.bs.modal", function () {

            const pencil = modal.querySelector(".pencil-modal");
            const textWrapper = modal.querySelector(".text-wrapper-modal");
            const title = modal.querySelector(".modal-title");

            if (!pencil || !textWrapper || !title) return;

            textWrapper.style.width = "0px";
            pencil.style.left = "0px";
            pencil.style.opacity = "1";

            const maxWidth = title.scrollWidth;
            let currentWidth = 0;

            function animate() {
                if (currentWidth <= maxWidth) {
                    currentWidth += 1;
                    textWrapper.style.width = currentWidth + "px";
                    pencil.style.left = currentWidth + "px";
                    requestAnimationFrame(animate);
                }
            }

            animate();
        });

        modal.addEventListener("hidden.bs.modal", function () {

            const pencil = modal.querySelector(".pencil-modal");
            const textWrapper = modal.querySelector(".text-wrapper-modal");

            if (!pencil || !textWrapper) return;

            textWrapper.style.width = "0px";
            pencil.style.left = "0px";
            pencil.style.opacity = "0";
        });

    });

});
