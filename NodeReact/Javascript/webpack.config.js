const Path = require('path');

module.exports = env => {
    let mode = env.mode.toLowerCase() === 'release'
        ? 'production'
        : 'development'; // Default to development, production mode minifies scripts
    console.log(`Mode: ${mode}.`);

    return [
        {
            entry: './worker.js',
            output: {
                filename: 'workerFileTemplate.js',
                path: Path.join(__dirname, 'bin'),
                libraryTarget: 'commonjs2',
            },
            mode: mode,
            target: 'node',
        },
        {
            entry: './interop.js',
            output: {
                filename: env.bundleName,
                path: Path.join(__dirname, 'bin', env.mode),
                libraryTarget: 'commonjs2',
            },
            mode: mode,
            target: 'node',
            module: {
                rules: [
                    {
                        test: /\\workerFileTemplate.js/,
                        loader: 'raw-loader'
                    }
                ]
            }
        },
        //{
        //    entry: './react.js',
        //    output: {
        //        filename: 'react.generated.js',
        //        path: path.resolve(__dirname, 'Resources/'),
        //        libraryTarget: 'commonjs2',
        //    },
        //    mode: 'production',
        //    target: 'node',
        //},
    ];
};