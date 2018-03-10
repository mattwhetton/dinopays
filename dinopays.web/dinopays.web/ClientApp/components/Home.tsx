import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface IHomeState {
    health: number,
    totalIncoming: number,
    totalOutgoing: number
}

interface IPet {
    id: string,
    health: number,
    summary: IPetHealthSummary
}

interface IPetHealthSummary {
    totalIncoming: number,
    totalOutgoing: number,
    positiveOutgoing: number,
    negativeOutgoing: number,
    positiveTransactions: IPetTransaction[],
    negativeTransactions: IPetTransaction[]
}

interface IPetTransaction {
    createdAt: string,
    description: string,
    amount: number,
    direction: string,
    spendingCategory: string,
    positivityCategory: string
}

export class Home extends React.Component<RouteComponentProps<{}>, IHomeState> {
    constructor() {
        super();

        this.state = {
            health: 3,
            totalIncoming: 0,
            totalOutgoing: 0
        };

        this.renderDino = this.renderDino.bind(this);
        this.refresh = this.refresh.bind(this);
        this.poll = this.poll.bind(this);

        setTimeout(this.poll, 10);
    }

    async poll() {
        await this.refresh();
        setTimeout(this.poll, 1000);
    }

    async refresh() {
        let response = await fetch('/api/pets/8721EDE4-EDD5-4321-B42A-1208BEED3FA1');
        let data = await response.json() as IPet;
        //console.debug(`Health: ${data.health}`);
        let health = Math.round(data.health / 2);

        this.sethealth(health, data.summary.totalIncoming, data.summary.totalOutgoing);
    }

    sethealth(health: number, totalIncoming: number, totalOutgoing: number) {
        this.setState({
            health: health,
            totalIncoming: totalIncoming,
            totalOutgoing: totalOutgoing
        });
    }

    renderDino() {
        let dinoSrc = `/dino-${this.state.health}.png`;
        return (
            <img src={dinoSrc}/>
        );
    }

    getDinoBg() {
        if(this.state.health < 2)
            return 'row dino-row-wrapper rain';
        if (this.state.health < 3)
            return 'row dino-row-wrapper dull';
        return 'row dino-row-wrapper';
    }

    public render() {
        return (
            <div>
                <div className={this.getDinoBg()}>
                    <div className='col-sm-12'>
                        <div className="text-center dino-row">
                            {this.renderDino()}
                        </div>
                        
                    </div>
                </div>

                <div className='row'>
                    <div className="col-sm-6">
                        <div className="summary-item">
                            <div>
                                <span>Incoming:</span>&nbsp;
                                <span className="pound">&pound;</span>
                                <span className="value">{this.state.totalIncoming.toLocaleString('en-GB')}</span>
                            </div>
                        </div>
                    </div>
                    <div className="col-sm-6">
                        <div className="summary-item">
                            <div>
                                <span>Outgoing:</span>&nbsp;
                                <span className="pound">&pound;</span>
                                <span className="value">{this.state.totalOutgoing.toLocaleString('en-GB')}</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div className='row'>
                    <div className="col-sm-12">
                        <button onClick={this.refresh}>
                            <i className="fas fa-sync-alt"></i>
                        </button>
                    </div>
                </div>
            </div>
        );
    }
}