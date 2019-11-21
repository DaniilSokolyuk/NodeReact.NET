var workerpool = require('workerpool');

var enginePool = null;

function execCode(requireList, code) {
    var xs = requireList.map(__non_webpack_require__);

    var result = eval(code);

    return result;
}

module.exports = {
    evalCode: function (callback, requireList, code) {

        try {
            var result = execCode(requireList, code);
            callback(null, result);
        } catch (err) {
            callback(err, null);
        }


        //try {
        //    if (enginePool === null) {
        //        enginePool = workerpool.pool('./interop.js');
        //    }

        //    enginePool.exec('execCode', [requireList, code])
        //        .then(function (result) {
        //            callback(null /* errors */, result);
        //        })
        //        .catch(function (err) {
        //            callback(err, null);
        //        });
        //}
        //catch (err) {
        //    callback(err, null);
        //}
    }
};