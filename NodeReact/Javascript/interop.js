const filewatcher = require('filewatcher');

const requireFiles = process.env.NODEREACT_REQUIREFILES.split(',').map(t => t.replace(/\\/g, '/'));
const fileWatcherDebounce = parseInt(process.env.NODEREACT_FILEWATCHERDEBOUNCE);

const watcher = filewatcher({
    debounce: fileWatcherDebounce, // debounce events in non-polling mode by 10ms
});

requireFiles.map(t => watcher.add(t));

watcher.on('change', () => {
    process.exit(0);
});

requireFiles.map(__non_webpack_require__);

const evalCode = (callback, code) => {
    try {
        const result = eval(code);

        callback(null, result);
    } catch (err) {
        callback(err, null);
    }
}

module.exports = { evalCode };