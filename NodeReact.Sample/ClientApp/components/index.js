import HelloWorld from "./HelloWorld";
import RootComponent from "./RootComponent";
import App from "./App";

export const components = {
  HelloWorld,
  RootComponent,
  App,
};

try {
  module.exports = components;
} catch {}
