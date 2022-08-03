var canvas = document.getElementById("unity-canvas");
var body = document.body;

applyScreenSize();
body.onresize = applyScreenSize;

function applyScreenSize() {
    var style = calculateStyle();
    canvas.setAttribute("style", createCss(style));    
}

function createCss(style) {
    return Object.keys(style).reduce(function (acc, key) {
        acc += key + ": " + style[key] + ";";
        return acc;
    }, "");
}

function calculateStyle() {
    return {
        "width": window.innerWidth + "px",
        "height": window.innerHeight + "px",
        "cursor": "default"
    };
}