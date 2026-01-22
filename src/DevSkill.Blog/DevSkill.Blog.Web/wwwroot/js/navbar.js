document.addEventListener('DOMContentLoaded', () => {
    const subNavbar = document.querySelector('.sub-navbar');
    if (!subNavbar) return;

    let isDragging = false;
    let startX = 0;
    let scrollStart = 0;

    subNavbar.addEventListener('mousedown', (e) => {
        isDragging = true;
        subNavbar.classList.add('dragging');
        startX = e.pageX - subNavbar.offsetLeft;
        scrollStart = subNavbar.scrollLeft;
    });

    ['mouseup', 'mouseleave'].forEach(event => {
        subNavbar.addEventListener(event, () => {
            isDragging = false;
            subNavbar.classList.remove('dragging');
        });
    });

    subNavbar.addEventListener('mousemove', (e) => {
        if (!isDragging) return;

        e.preventDefault();
        const x = e.pageX - subNavbar.offsetLeft;
        const distance = (x - startX) * 1.2;
        subNavbar.scrollLeft = scrollStart - distance;
    });
});

