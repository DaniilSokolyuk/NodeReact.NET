import React, { PureComponent } from 'react';

export default class HelloWorld extends PureComponent {
    render() {
        return (<h1>Hello {this.props.name}</h1>);
    }
}