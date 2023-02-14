const filewatcher = require("filewatcher");

const requireFiles = process.env.NODEREACT_REQUIREFILES.split(",").map((t) =>
  t.replace(/\\/g, "/")
);
const fileWatcherDebounce = parseInt(process.env.NODEREACT_FILEWATCHERDEBOUNCE);

const watcher = filewatcher({
  debounce: fileWatcherDebounce, // debounce events in non-polling mode by 10ms
});

requireFiles.map((t) => watcher.add(t));

watcher.on("change", () => {
  process.exit(0);
});

requireFiles.map(__non_webpack_require__);

const renderComponent = (callback, componentId, options, props) => {
  try {
    const component = resolveComponent(global, options.componentName);

    if (options.serverOnly) {
      const res = ReactDOMServer.renderToStaticNodeStream(
        React.createElement(
          component,
          Object.assign(props, {
            location: options.location || "",
            context: {},
          })
        )
      );

      callback(null, res);
    } else {
      let context = {};
      let error;
      let bootstrapScriptContent = "";

      if (!options.disableBootstrapPropsInPlace) {
        if (options.bootstrapScriptContent) {
          bootstrapScriptContent = `(window.__nrp = window.__nrp || {})['${componentId}'] = ${JSON.stringify(
            props
          )}; ${options.bootstrapScriptContent}`;
        } else {
          bootstrapScriptContent = `(window.__nrp = window.__nrp || {})['${componentId}'] = ${JSON.stringify(
            props
          )};`;
        }
      } else {
        bootstrapScriptContent = options.bootstrapScriptContent;
      }

      const { pipe } = ReactDOMServer.renderToPipeableStream(
        React.createElement(
          component,
          Object.assign(props, {
            location: options.location || "",
            context: context,
          })
        ),
        {
          bootstrapScriptContent: bootstrapScriptContent,
          onShellReady() {
            if (!options.disableStreaming) {
              callbackPipe(callback, pipe, error, context);
            }
          },
          onShellError(error) {
            callback(error, null);
          },
          onAllReady() {
            if (options.disableStreaming) {
              callbackPipe(callback, pipe, error, context);
            }
          },
          onError(err) {
            error = err;
            console.error(err);
          },
        }
      );
    }
  } catch (err) {
    callback(err, null);
  }
};

const callbackPipe = (callback, pipe, error, context) => {
  callback(error, null, (res) => {
    if (context.url) {
      res.setHeader("RspUrl", context.url);
    }

    if (context.status) {
      res.setHeader("RspCode", context.status);
    }

    pipe(res);

    return true;
  });
};

const resolveComponent = (object, path, defaultValue) => {
  let current = object;
  const pathArray = typeof path === "string" ? path.split(".") : path;

  for (const prop of pathArray) {
    if (current == null) {
      return defaultValue;
    }

    current = current[prop];
  }

  return current == null ? defaultValue : current;
};

export { renderComponent };
