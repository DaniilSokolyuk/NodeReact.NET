const path = require("path");
const { merge } = require("webpack-merge");

/**
 * @param {string} name
 * @returns {string}
 */
const fromRoot = (name) => path.resolve(__dirname, ".", name);

const babelServer = {
  loader: 'babel-loader',
  options: {
    presets: [
      [
        '@babel/preset-env',
        {
          modules: 'commonjs',
          useBuiltIns: 'entry',
          corejs: 3,
          targets: { node: true },
        },
      ],
      '@babel/preset-react',
    ],
  },
}

const babelClient = {
  loader: 'babel-loader',
  options: {
    presets: [
      [
        '@babel/preset-env',
        {
          modules: false,
          useBuiltIns: 'usage',
          corejs: 3,
        },
      ],
      '@babel/preset-react',
    ],
  },
}

const common = {
  resolve: {
    extensions: ["*", ".js", ".jsx", ".cjs"],
  },
  output: {
    path: fromRoot("wwwroot"),
    filename: "./[name].bundle.js",
  },
  module: {
    rules: [
      {
        test: /\.jsx?$/,
        exclude: /node_modules/,
        loader: "babel-loader",
      },
    ],
  },
};
const client = merge(common, {
  entry: { client: "./ClientApp/client.js" },
  target: 'web',
  module: {
    rules: [
      {
        test: /\.jsx?$/,
        exclude: /node_modules/,
        use: babelClient,
      },
    ],
  },
});

const server = merge(common, {
  entry: { server: "./ClientApp/server.js" },
  target: 'node',
  module: {
    rules: [
      {
        test: /\.jsx?$/,
        exclude: /node_modules/,
        use: babelServer,
      },
    ],
  },
});

module.exports = [client, server];
