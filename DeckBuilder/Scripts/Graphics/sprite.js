var spriteCache = {};

function Sprite(imageSrc) {
    if (spriteCache[imageSrc] == undefined) {
        this.image = new Image();
        this.image.src = imageSrc;
        spriteCache[imageSrc] = this.image;
    }
    else {
        this.image = spriteCache[imageSrc];
    }
};