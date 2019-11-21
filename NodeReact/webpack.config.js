const path = require('path');

module.exports = [
	{
		entry: './Resources/interop.js',
		output: {
			filename: '[name].generated.js',
            path: path.resolve(__dirname, 'Resources/'),
            libraryTarget: 'commonjs2',
		},
		mode: 'development',
        target: 'node'
	},
	{
        entry: './Resources/interop.js',
		output: {
			filename: '[name].generated.min.js',
			path: path.resolve(__dirname, 'Resources/'),
            libraryTarget: 'commonjs2',
		},
		mode: 'production',
        target: 'node',
	},
];