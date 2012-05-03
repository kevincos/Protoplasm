var blarg = 5;

function SpriteContext(context2d, width, height) {
    this.context2d = context2d;
    this.width = width;
    this.height = height;
};

SpriteContext.prototype.clear = function () {
    this.context2d.clearRect(0, 0, this.width, this.height);
}

SpriteContext.prototype.draw = function (sprite, /*vector*/v, w, h, rotate, scale) {
    this.context2d.save();
    this.context2d.translate(v.x, v.y);
    this.context2d.rotate(rotate);
    this.context2d.scale(scale, scale);
    this.context2d.translate(-w / 2, -h / 2);
    this.context2d.drawImage(sprite.image, 0, 0, w, h);
    this.context2d.restore();
}

SpriteContext.prototype.text = function (text, v) {
    this.context2d.font = '14px Verdana';    
    this.context2d.fillText(text, v.x, v.y);
}

SpriteContext.prototype.formattedText = function (text, v, font, color) {
    this.context2d.font = font;
    this.context2d.fillStyle = color;
    this.context2d.fillText(text, v.x - this.context2d.measureText(text).width / 2, v.y);
}

SpriteContext.prototype.setTextType = function (font, color) {
    this.context2d.font = font;
    this.context2d.fillStyle = color;
}

SpriteContext.prototype.textWrap = function (text, v, maxWidth, lineHeight) {
    var words = text.split(" ");
    var line = "";
    var x = v.x;
    var y = v.y;

    for (var n = 0; n < words.length; n++) {
        var testLine = line + words[n] + " ";
        var metrics = this.context2d.measureText(testLine);
        var testWidth = metrics.width;
        if (testWidth > maxWidth) {
            this.context2d.fillText(line, x, y);
            line = words[n] + " ";
            y += lineHeight;
        }
        else {
            line = testLine;
        }
    }
    this.context2d.fillText(line, x, y);
}


