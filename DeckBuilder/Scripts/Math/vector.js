var vMath = {
    vector2: function (x, y) {
        this.x = x;
        this.y = y;
    },

    add: function (v1, v2) {
        return new vector2(v1.x + v2.x, v1.y + v2.y);
    },

    subtract: function (v1, v2) {
        return new vector2(v1.x - v2.x, v1.y - v2.y);
    },

    scale: function (k, v) {
        return new vector2(k * v.x, k * v.y);
    },

    length: function (v) {
        return Math.sqrt(v.x * v.x + v.y * v.y);
    },

    mousePos: new vector2(0, 0),

    mouseDown: false,

    mouseUpListener: function (event) {
        vMath.mouseDown = false;
    },

    mouseDownListener: function (event) {
        vMath.mouseDown = true;
    },

    mouseMoveListener: function (event) {
        var x = new Number();
        var y = new Number();
        var canvas = document.getElementById("canvas");

        if (event.x != undefined && event.y != undefined) {
            x = event.x + document.body.scrollLeft +
                  document.documentElement.scrollLeft;
            y = event.y + document.body.scrollTop +
                  document.documentElement.scrollTop;
        }
        else // Firefox method to get the position
        {
            x = event.clientX + document.body.scrollLeft +
                  document.documentElement.scrollLeft;
            y = event.clientY + document.body.scrollTop +
                  document.documentElement.scrollTop;
        }

        x -= canvas.offsetLeft;
        y -= canvas.offsetTop;


        vMath.mousePos = new vector2(x, y);
    }
};

function vector2(x, y) {
        this.x = x;
        this.y = y;
}


