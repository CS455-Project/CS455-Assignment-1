window.playSound1 = function () {
    var sound = document.getElementById("hitSound");
    sound.currentTime = 0; // Reset to start
    sound.play();
};

window.playSound2 = function () {
    var sound = document.getElementById("missSound");
    sound.currentTime = 0; // Reset to start
    sound.play();
};

