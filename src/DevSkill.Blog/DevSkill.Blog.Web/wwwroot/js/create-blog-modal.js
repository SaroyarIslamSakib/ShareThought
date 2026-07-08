document.addEventListener("DOMContentLoaded", function () {

    const modal = document.getElementById("createBlogModal");

    modal.addEventListener("shown.bs.modal", function () {

        const pencil = modal.querySelector(".pencil-modal");
        const textWrapper = modal.querySelector(".text-wrapper-modal");
        const title = modal.querySelector(".modal-title");

        textWrapper.style.width = "0px";
        pencil.style.left = "0px";
        pencil.style.opacity = "1";

        const maxWidth = title.scrollWidth;
        let currentWidth = 0;

        function animate() {

            if (currentWidth <= maxWidth) {

                currentWidth += 1; // speed control

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

        textWrapper.style.width = "0px";
        pencil.style.left = "0px";
        pencil.style.opacity = "0";
    });

});
