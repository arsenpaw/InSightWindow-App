var startY;
var pulling = false;

window.addEventListener('touchstart', function (e) {
    if (window.scrollY === 0) {
        startY = e.touches[0].clientY;
        pulling = true;
    }
}, { passive: true });

window.addEventListener('touchmove', function (e) {
    if (pulling) {
        var currentY = e.touches[0].clientY;
        var diffY = currentY - startY;

        if (diffY > 0) {
            e.preventDefault();
        }
    }
}, { passive: false });

window.addEventListener('touchend', function (e) {
    if (pulling) {
        var currentY = e.changedTouches[0].clientY;
        var diffY = currentY - startY;

        if (diffY > 50) {
            location.reload();
        }
    }

    startY = null;
    pulling = false;
}, { passive: true });
