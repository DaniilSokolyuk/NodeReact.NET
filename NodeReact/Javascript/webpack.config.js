const Path = require('path');

module.exports = env => {
    let mode = env.mode.toLowerCase() === 'release'
        ? 'production'
        : 'development'; // Default to development, production mode minifies scripts
    console.log(`Mode: ${mode}.`);

    return [
        {
            entry: './interop.js',
            output: {
                filename: env.bundleName,
                path: Path.join(__dirname, 'bin', env.mode),
                libraryTarget: 'commonjs2',
            },
            mode: mode,
            target: 'node',
        },
    ];
};