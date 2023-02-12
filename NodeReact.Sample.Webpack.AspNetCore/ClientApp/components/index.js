import HelloWorld from "./HelloWorld";
import RootComponent from "./RootComponent";

export const components = {
  HelloWorld,
  RootComponent,
};

try {
  module.exports = components;
} catch {}
