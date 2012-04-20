var spriteCache = {};

function Sprite(imageSrc) {
    if (spriteCache[imageSrc] == undefined) {
        this.image = new Image();
        this.image.src = imageSrc;
        this.image.onload = function () {
            if (gameCore != undefined && gameCore != null) {
                gameCore.refreshView = true;
            }
        };
        spriteCache[imageSrc] = this.image;
    }
    else {
        this.image = spriteCache[imageSrc];
    }
};