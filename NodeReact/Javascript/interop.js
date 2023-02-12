var filewatcher = require('filewatcher');

var requireFiles = process.env.NODEREACT_REQUIREFILES.split(',').map(t => t.replace(/\\/g, '/'));
var fileWatcherDebounce = parseInt(process.env.NODEREACT_FILEWATCHERDEBOUNCE);

var watcher = filewatcher({
    debounce: fileWatcherDebounce, // debounce events in non-polling mode by 10ms
});
requireFiles.map(t => watcher.add(t));
watcher.on('change', function (file, stat) {
    process.exit(0)
});

requireFiles.map(__non_webpack_require__);

function evalCode(callback, code) {
    usages++;
    
    try {
        const result = eval(code);
        callback(null, result);
    }
    catch (err) {
        callback(err, null);
    }
}

module.exports = { evalCode };