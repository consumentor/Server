Type.registerNamespace('Sys.Extended.UI.Seadragon');

Sys.Extended.UI.Seadragon.Rect = function(x, y, width, height) {
    // Properties

    this.x = typeof (x) == "number" ? x : 0;
    this.y = typeof (y) == "number" ? y : 0;
    this.width = typeof (width) == "number" ? width : 0;
    this.height = typeof (height) == "number" ? height : 0;
}
Sys.Extended.UI.Seadragon.Rect.prototype = {
    getAspectRatio: function() {
        return this.width / this.height;
    },

    getTopLeft: function() {
    return new Sys.Extended.UI.Seadragon.Point(this.x, this.y);
    },

    getBottomRight: function() {
    return new Sys.Extended.UI.Seadragon.Point(this.x + this.width, this.y + this.height);
    },

    getCenter: function() {
    return new Sys.Extended.UI.Seadragon.Point(this.x + this.width / 2.0,
                        this.y + this.height / 2.0);
    },

    getSize: function() {
    return new Sys.Extended.UI.Seadragon.Point(this.width, this.height);
    },

    equals: function(other) {
        return (other instanceof Seadragon.Rect) &&
                (this.x === other.x) && (this.y === other.y) &&
                (this.width === other.width) && (this.height === other.height);
    },

    toString: function() {
        return "[" + this.x + "," + this.y + "," + this.width + "x" +
                this.height + "]";
    }
}
Sys.Extended.UI.Seadragon.Rect.registerClass('Sys.Extended.UI.Seadragon.Rect', null, Sys.IDisposable);
