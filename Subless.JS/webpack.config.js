// Generated using webpack-cli https://github.com/webpack/webpack-cli
const Dotenv = require('dotenv-webpack');
const path = require("path");

const isProduction = process.env.NODE_ENV != "local";

const config = {
  entry: "./src/subless.ts",
  experiments: {
    outputModule: true,
  },
  output: {
    filename: '../../SublessSignIn/wwwroot/dist/subless.js',
    library: {
      type: "module"
    }
  },
  plugins: [
    new Dotenv({
      path: './environment/' + process.env.NODE_ENV + '.env',
      safe: true
    })
  ],
  module: {
    rules: [
      {
        test: /\.(ts|tsx)$/i,
        loader: "ts-loader",
        exclude: ["/node_modules/"],
      },
      {
        test: /\.(eot|svg|ttf|woff|woff2|png|jpg|gif)$/i,
        type: "asset",
      },

      // Add your rules for custom modules here
      // Learn more about loaders from https://webpack.js.org/loaders/
    ],
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
  },
};

module.exports = () => {
  if (isProduction) {
    config.mode = "production";
  } else {
    config.mode = "development";
  }
  return config;
};