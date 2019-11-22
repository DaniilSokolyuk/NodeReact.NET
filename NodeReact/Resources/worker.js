
var workerpool = require('workerpool');

var requireFiles = process.env.NODEREACT_REQUIREFILES.split(',').map(t => t.replace(/\\/g, '/'));


function execCode(code) {

    requireFiles.map(__non_webpack_require__);

    var result = eval(code);

    return result;
}

workerpool.worker({
    execCode: execCode
});
