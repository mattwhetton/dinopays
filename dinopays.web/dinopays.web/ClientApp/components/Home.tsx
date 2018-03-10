import * as React from 'react';
import * as moment from 'moment';
import { RouteComponentProps } from 'react-router';

interface IHomeState {
    health: number,
    totalIncoming: number,
    totalOutgoing: number,
    recentBonusTransactions: IPetTransaction[]
}

interface IPet {
    id: string,
    health: number,
    bonusHealth: number,
    summary: IPetHealthSummary
}

interface IPetHealthSummary {
    totalIncoming: number,
    totalOutgoing: number,
    positiveOutgoing: number,
    negativeOutgoing: number,
    recentBonusTransactions: IPetTransaction[],
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
            totalOutgoing: 0,
            recentBonusTransactions: []
        };

        this.renderDino = this.renderDino.bind(this);
        this.refresh = this.refresh.bind(this);
        this.poll = this.poll.bind(this);
        this.getTransactions = this.getTransactions.bind(this);
        this.getTransaction = this.getTransaction.bind(this);
        this.getHealthDescription = this.getHealthDescription.bind(this);

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
        let health = Math.min(Math.round((data.health + data.bonusHealth) / 2), 5);

        this.sethealth(health, data.summary.totalIncoming, data.summary.totalOutgoing, data.summary.recentBonusTransactions);
    }

    sethealth(health: number, totalIncoming: number, totalOutgoing: number, recentBonusTransactions: IPetTransaction[]) {
        this.setState({
            health: health,
            totalIncoming: totalIncoming,
            totalOutgoing: totalOutgoing,
            recentBonusTransactions: recentBonusTransactions
        });
    }

    renderDino() {
        let dinoSrc = `/dino-${this.state.health}.png`;
        return (
            <img src={dinoSrc}/>
        );
    }

    getDinoBg() {
        if (this.state.health < 2)
            return 'row dino-row-wrapper rain';
        if (this.state.health < 3)
            return 'row dino-row-wrapper dull';
        return 'row dino-row-wrapper';
    }

    getTransaction(txn: IPetTransaction) {
        let date = moment(txn.createdAt);

        if (txn.positivityCategory === "Positive") {
            return (
                <div className="transaction positive" key={txn.createdAt}>
                    <span className="thumb green">
                        <i className="fas fa-thumbs-up"></i>
                    </span> &nbsp; You put &pound;{txn.amount.toLocaleString('en-GB')} towards {this.translateSpendingCategory(txn.spendingCategory)} {date.fromNow()} ({txn.description})
                </div>
            );
        } else {
            return (
                <div className="transaction negative" key={txn.createdAt}>
                    <span className="thumb red">
                        <i className="fas fa-thumbs-down"></i>
                    </span> &nbsp; You spent &pound;{txn.amount.toLocaleString('en-GB')} on {this.translateSpendingCategory(txn.spendingCategory)} {date.fromNow()} ({txn.description})
                </div>
            );
        }
    }

    translateSpendingCategory(cat: string) {
        if (cat === "EATING_OUT")
            return "eating out";
        return cat.toLowerCase();
    }

    getTransactions() {
        return (
            <div >
                <h3 className="transactions-title">Why your dinopet is <span className="green">{this.getHealthDescription()}</span></h3>
                <div className="transactions">
                    {this.state.recentBonusTransactions.map(this.getTransaction)}
                </div>
            </div>
        );
    }

    getHealthDescription() {
        if (this.state.health === 0)
            return 'very unhappy';
        if (this.state.health === 1)
            return 'unhappy';
        if (this.state.health === 2)
            return 'a little unhappy';
        if (this.state.health === 3)
            return 'quite happy';
        if (this.state.health === 4)
            return 'happy';
        if (this.state.health === 5)
            return 'ecstatic';
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
                    <div className="col-sm-10 col-sm-offset-1">
                        {this.getTransactions()}
                    </div>
                </div>

            </div>
        );
    }
}