document.addEventListener("DOMContentLoaded", function () {

    const pencil = document.querySelector(".pencil");
    const textWrapper = document.querySelector(".text-wrapper");
    const blogName = document.querySelector(".blog-name");

    function startWriting() {

        let pos = -20;
        const max = blogName.scrollWidth;

        pencil.style.opacity = "1";

        function frame() {
            if (pos <= max) {
                pos += 1;
                pencil.style.left = pos + "px";
                textWrapper.style.width = pos + "px";
                requestAnimationFrame(frame);
            } else {

                // Hide pencil immediately
                pencil.style.opacity = "0";

                // Pause while text remains visible
                setTimeout(() => {

                    // Reset everything
                    pencil.style.left = "-20px";
                    textWrapper.style.width = "0px";

                    // Restart
                    startWriting();

                }, 2000); // text pause duration
            }
        }

        frame();
    }

    startWriting();
});
