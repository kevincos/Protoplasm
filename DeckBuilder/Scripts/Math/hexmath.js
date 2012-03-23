var hexMath = {
    origin: new vMath.vector2(375, 385),
    hexOrigin: new vMath.vector2(6,5),
    theta: .0,
    r: 32,
    root3: 1.73205,

    hexTile: function (a, b) {
        this.a = a;
        this.b = b;
    },

    toScreenCoords: function (a, b) {
        //return new vector2(origin.x + a * r * root3, origin.y + a * r + 2 * b * r);

        return new vector2(hexMath.origin.x + (a - hexMath.hexOrigin.x) * hexMath.r * (hexMath.root3 * Math.cos(hexMath.theta) - Math.sin(hexMath.theta)) - 2 * (b - hexMath.hexOrigin.y) * hexMath.r * Math.sin(hexMath.theta),
            hexMath.origin.y + (a - hexMath.hexOrigin.x) * hexMath.r * (hexMath.root3 * Math.sin(hexMath.theta) + Math.cos(hexMath.theta)) + 2 * (b - hexMath.hexOrigin.y) * hexMath.r * Math.cos(hexMath.theta));
    },

    toHexCoords: function (x, y) {
        //return new vector2((x - origin.x) / r / root3, (y - origin.y) / 2 / r - (x - origin.x) / 2 / r / root3);
        var rx = x - hexMath.origin.x;
        var ry = y - hexMath.origin.y;
        return new vector2(hexMath.hexOrigin.x + (rx * Math.cos(hexMath.theta) + ry * Math.sin(hexMath.theta)) / hexMath.root3 / hexMath.r,
            hexMath.hexOrigin.y + 1 / hexMath.r / 2 * (-rx * Math.sin(hexMath.theta) - rx * Math.cos(hexMath.theta) / hexMath.root3 + ry * Math.cos(hexMath.theta) - ry * Math.sin(hexMath.theta) / hexMath.root3));

    }
};