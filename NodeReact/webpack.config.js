const path = require('path');

module.exports = [
    {
        entry: './Resources/worker.js',
        output: {
            filename: 'worker.generated.js',
            path: path.resolve(__dirname, 'Resources/'),
            libraryTarget: 'commonjs2',
        },
        mode: 'production',
        target: 'node'
    },
	{
		entry: './Resources/interop.js',
		output: {
			filename: 'interop.generated.js',
            path: path.resolve(__dirname, 'Resources/'),
            libraryTarget: 'commonjs2',
		},
		mode: 'development',
        target: 'node'
	},
	{
        entry: './Resources/interop.js',
		output: {
			filename: 'interop.generated.min.js',
			path: path.resolve(__dirname, 'Resources/'),
            libraryTarget: 'commonjs2',
		},
		mode: 'production',
        target: 'node',
    },
    {
        entry: './Resources/react.js',
        output: {
            filename: 'react.generated.min.js',
            path: path.resolve(__dirname, 'Resources/'),
            libraryTarget: 'commonjs2',
        },
        mode: 'production',
        target: 'node',
    },
];