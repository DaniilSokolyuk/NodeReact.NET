import * as stream from 'stream';

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

const renderComponent = (callback, componentId, componentName, serverOnly, props) => {
    try {
        const component = eval(componentName)

        if (serverOnly) {
            const res = ReactDOMServer.renderToStaticNodeStream(React.createElement(component, props))
            callback(null, res);
        } else {
            // renderToPipeableStream is not working with Jering.Javascript.NodeJS
            // because it is not a stream.Readable
            const res = ReactDOMServer.renderToString(React.createElement(component, props));
            callback(null, res);
        }
    } catch (err) {
        callback(err, null);
    }
}

const renderRouter = (callback, componentId, componentName, serverOnly, props, path) => {
    try {
        const component = eval(componentName)

        let context = {};
        if (serverOnly) {
            
        } else {
           
        }
    } catch (err) {
        callback(err, null);
    }
}


export {renderComponent, renderRouter};