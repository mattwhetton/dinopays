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
                            <h2 className="text-center">you're friendly, judgmental, penny pinching dinosaur</h2>
                       </div>
                   </div>

                   { this.props.children }
               </div>

    }
}