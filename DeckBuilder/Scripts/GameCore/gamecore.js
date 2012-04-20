function GameCore() {
    this.FPS = 30;
    this.SECONDS_BETWEEN_FRAMES = 1 / this.FPS;
    this.canvas = null;
    this.spriteContext = null;
    this.currentTime = 0;


    this.refreshView = true;
    this.mouseDown = false;
    this.prevMousePos = null;

    this.draw = null;
    this.update = null;
};


GameCore.prototype.init = function () {
    this.canvas = document.getElementById('canvas');
    this.canvas.addEventListener("mousemove", vMath.mouseMoveListener, false);
    this.canvas.addEventListener("mousedown", vMath.mouseDownListener, false);
    this.canvas.addEventListener("mouseup", vMath.mouseUpListener, false);
    this.spriteContext = new SpriteContext(this.canvas.getContext('2d'), this.canvas.width, this.canvas.height);
    var T = this;
    setInterval(function () { T.updateLoop(); }, this.SECONDS_BETWEEN_FRAMES);
};

GameCore.prototype.updateLoop = function () {
    if (vMath.mouseDown == false) {
        this.mouseDown = false;
    }

    if (this.update != null) {
        this.update();
    }

    if (this.refreshView == false && vMath.mouseDown == false) {
        return;
    }
    if (this.mouseDown == true) {
        return
    }

    if (vMath.mouseDown == true) {
        this.mouseDown = true;
    }
    this.refreshView = false;

    this.currentTime += gameCore.SECONDS_BETWEEN_FRAMES;
    this.spriteContext.clear();

    if (this.draw != null) {
        this.draw();
    }
};

