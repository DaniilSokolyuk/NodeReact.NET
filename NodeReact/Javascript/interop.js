var workerpool = require('workerpool');
var filewatcher = require('filewatcher');
var tempWrite = require('temp-write');

var minWorkers = parseInt(process.env.NODEREACT_MINWORKERS);
var maxWorkers = parseInt(process.env.NODEREACT_MAXWORKERS);
var workerMaxUsages = parseInt(process.env.NODEREACT_MAXUSAGESPERFWORKER);  //used by workers
var requireFiles = process.env.NODEREACT_REQUIREFILES.split(',').map(t => t.replace(/\\/g, '/'));

var workerFileTemplate = require('./bin/workerFileTemplate.js').default;
var workerFilePath = tempWrite.sync(workerFileTemplate, 'worker.js');

function workerPoolFactory() {
    return workerpool.pool(workerFilePath, { minWorkers: minWorkers, maxWorkers: maxWorkers, workerType: 'thread' });
}

var enginePool = workerPoolFactory();

var watcher = filewatcher({
    debounce: 10, // debounce events in non-polling mode by 10ms
});
requireFiles.map(t => watcher.add(t));
watcher.on('change', function (file, stat) {
    var oldPool = enginePool;
    enginePool = workerPoolFactory();
    oldPool.terminate();
});

function evalCode(callback, code) {
    try {
        enginePool.exec('execCode', [code])
            .then(function (result) {
                callback(null, result);
            })
            .catch(function (err) {
                //worker usages exceeded
                if (err.message.includes("exitCode: `25`")) {
                    evalCode(callback, code);
                    return;
                }

                callback(err, null);
            });
    }
    catch (err) {
        callback(err, null);
    }
}

module.exports = { evalCode };