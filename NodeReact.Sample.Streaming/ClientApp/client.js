import React from 'react';
import ReactDOM from 'react-dom/client';
import { components } from './components';
import App from "./components/App";

window.React = React;
window.ReactDOM = ReactDOM;

window.Components = components;

// in view streaming we send to client the entire document and in 
// Object.values(window["__nrp"])[0] we always have props from .NET backend
// by default in NodeReactionViewOptions property BootstrapScriptContent = "window.__nrpBoot ? __nrpBoot() : (window.__nrpReady = true)"
// with this approach, hydration is done as quickly as possible 
// you can change property BootstrapScriptContent by INodeReactViewOptionsProvider implementation 
// read more here: https://github.com/reactwg/react-18/discussions/114

window.__nrpBoot = function() {
    ReactDOM.hydrateRoot(document, <App {...Object.values(window["__nrp"])[0]} />)
}

if (window.__nrpReady) {
    window.__nrpBoot();
}

