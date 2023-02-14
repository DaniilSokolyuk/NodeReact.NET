import React from "react";
import RootComponent from "./RootComponent";
import HelloWorld from "./HelloWorld";

const App = ({...props}) => {
    return (
        <html>
        <head>
            <meta charSet="utf-8"/>
            <meta name="viewport" content="width=device-width, initial-scale=1"/>
            <title>{props.title}</title>
            { //DONT use defer or preload for hydration SCRIPTS, use async https://github.com/reactwg/react-18/discussions/114 --> 
                 }
            <script src="client.bundle.js" async></script>
        </head>
        <body>
        <HelloWorld name={props.title}></HelloWorld>
        <RootComponent location={props.location}/>
        </body>
        </html>
    )
};

export default App;