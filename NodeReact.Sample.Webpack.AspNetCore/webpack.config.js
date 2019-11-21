module.exports = {
  resolve: {
    extensions: ['*', '.js', '.jsx']
  },
  entry: {
    server: './ClientApp/server.js',
    client: './ClientApp/client.js'
  },
  output: {
    filename: './wwwroot/[name].bundle.js'
  },
  module: {
    rules: [
      {
        test: /\.jsx?$/,
        exclude: /node_modules/,
        loader: 'babel-loader'
      }
    ]
  },
  resolveLoader: {
    moduleExtensions: ['-loader']
  }
};
