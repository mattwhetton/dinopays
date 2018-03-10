import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface ISomeShit {
    test: string
}

export class Home extends React.Component<RouteComponentProps<{}>, ISomeShit> {
    constructor() {
        super();
        this.state = { test: "hello" };
        this.state = { test: "hello" };
    }

    public render() {
        return (
            <div className="text-center dino-row">
                <img src="dino-normal.png" />
            </div>
        );
    }
}
