window.playSound1 = function () {
    const sound = document.getElementById("hitSound");
    sound.currentTime = 0; // Reset to start
    sound.play();
};

window.playSound2 = function () {
    const sound = document.getElementById("missSound");
    sound.currentTime = 0; // Reset to start
    sound.play();
};

