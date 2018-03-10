import * as React from 'react';
import { NavMenu } from './NavMenu';

export interface LayoutProps {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
    public render() {
        return <div className='container-fluid'>
            <div className="row">
                <div className="col-sm-12 header">
                    <h1 className="text-center">
                        DIN<span className="small">o</span>PAY<span className="big">$</span>
                    </h1>
                </div>
            </div>
            <div className='row'>
                <div className='col-sm-12'>
                    { this.props.children }
                </div>
            </div>
        </div>;
    }
}
