
var workerpool = require('workerpool');

var requireFiles = process.env.NODEREACT_REQUIREFILES.split(',').map(t => t.replace(/\\/g, '/'));

requireFiles.map(__non_webpack_require__);

function execCode(code) {
    var result = eval(code);

    return result;
}

workerpool.worker({
    execCode: execCode
});
