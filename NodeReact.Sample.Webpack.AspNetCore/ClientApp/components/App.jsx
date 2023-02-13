import React from "react";
import RootComponent from "./RootComponent";

const App = ({...props}) => {
    return (
        <html>
        <head>
            <meta charSet="utf-8"/>
            <meta name="viewport" content="width=device-width, initial-scale=1"/>
            <title>{props.title}</title>
        </head>
        <body>
        <h1>{props.title}</h1>
        <RootComponent location={props}/>
        </body>
        </html>
    )
};

export default App;