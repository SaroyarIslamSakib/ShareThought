const pencil = document.getElementById("pencil");
const textWrapper = document.querySelector(".text-wrapper");
const writeArea = document.querySelector(".write-area");

function start() {
    let pos = -20;
    const max = writeArea.offsetWidth;
    pencil.style.opacity = "1";

    function frame() {
        if (pos <= max) {
            pos += 1.5;
            pencil.style.left = pos + "px";
            textWrapper.style.width = pos + "px";
            requestAnimationFrame(frame);
        } else {

            pencil.style.opacity = "0";

            setTimeout(() => {
                pencil.style.left = "-20px";
                textWrapper.style.width = "0px";

                setTimeout(() => {
                    start();
                }, 500);
            }, 2000);
        }
    }

    frame();
}

start();