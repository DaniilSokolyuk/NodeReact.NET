
var workerpool = require('workerpool');

var requireFiles = process.env.NODEREACT_REQUIREFILES.split(',').map(t => t.replace(/\\/g, '/'));
var workerMaxUsages = parseInt(process.env.NODEREACT_MAXUSAGESPERFWORKER);

var usages = 0;

function execCode(code) {
    if (workerMaxUsages > 0 && usages++ > workerMaxUsages) {
        process.exit(25);
    }

    requireFiles.map(__non_webpack_require__);

    var result = eval(code);

    return result;
}

workerpool.worker({
    execCode: execCode
});
