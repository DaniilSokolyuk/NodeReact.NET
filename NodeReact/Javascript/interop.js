var requireFiles = process.env.NODEREACT_REQUIREFILES.split(',').map(t => t.replace(/\\/g, '/'));

function evalCode(callback, code) {
    requireFiles.map(__non_webpack_require__);

    try {
        var result = eval(code);
        callback(null, result);
    }
    catch (err) {
        callback(err, null);
    }
}

module.exports = { evalCode };