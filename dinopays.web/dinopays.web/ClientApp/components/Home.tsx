import * as React from 'react';
import * as moment from 'moment';
import { RouteComponentProps } from 'react-router';

interface IHomeState {
    health: number,
    totalIncoming: number,
    totalOutgoing: number,
    recentBonusTransactions: IPetTransaction[],
    goals: IPetGoalSummary[],
    pettingCount: number,
    isPetting: number
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
    goals: IPetGoalSummary[]
}

interface IPetGoalSummary {
    onTarget: boolean,
    goal: IPetGoal
}

interface IPetGoal {
    id: string,
    name: string,
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
            recentBonusTransactions: [],
            goals: [],
            pettingCount: 0,
            isPetting: 0
        };

        this.renderDino = this.renderDino.bind(this);
        this.refresh = this.refresh.bind(this);
        this.poll = this.poll.bind(this);
        this.getTransactions = this.getTransactions.bind(this);
        this.getTransaction = this.getTransaction.bind(this);
        this.getHealthDescription = this.getHealthDescription.bind(this);
        this.getGoal = this.getGoal.bind(this);
        this.getGoals = this.getGoals.bind(this);
        this.petDino = this.petDino.bind(this);
        this.resetPettingCount = this.resetPettingCount.bind(this);
        this.saveState = this.saveState.bind(this);
        this.loadState = this.loadState.bind(this);
        this.resetIsPettingCount = this.resetIsPettingCount.bind(this);

        this.loadState();
    }

    loadState() {
        let stateStr = localStorage.getItem("sd");
        if (stateStr) {
            let state = JSON.parse(stateStr);
            state.isPetting = 0;
            state.pettingCount = 0;
            this.state = state;
        }
    }

    saveState() {
        let stateStr = JSON.stringify(this.state);
        localStorage.setItem("sd", stateStr);
    }

    componentDidMount() {
        setTimeout(this.poll, 10);
    }

    async poll() {
        await this.refresh();
        setTimeout(this.poll, 1000);
    }

    async refresh() {
        let response = await fetch('/api/pets/dave');
        let data = await response.json() as IPet;
        //console.debug(`Health: ${data.health}`);
        let health = Math.min(Math.round((data.health + data.bonusHealth) / 2), 5);

        this.sethealth(health, data.summary.totalIncoming, data.summary.totalOutgoing, data.summary.recentBonusTransactions, data.summary.goals);
    }

    sethealth(health: number, totalIncoming: number, totalOutgoing: number, recentBonusTransactions: IPetTransaction[], goals: IPetGoalSummary[]) {
        health = Math.max(Math.min(health, 5), 0);
        this.setState({
            health: health,
            totalIncoming: totalIncoming,
            totalOutgoing: totalOutgoing,
            recentBonusTransactions: recentBonusTransactions,
            goals: goals
        });
        this.saveState();
    }

    petDino() {
        let newPettingCount = this.state.pettingCount + 1;
        let newIsPettingCount = this.state.isPetting + 1;
        this.setState({
            pettingCount: newPettingCount,
            isPetting: newIsPettingCount
        });

        setTimeout(this.resetIsPettingCount, 500);

        if (newPettingCount > 2) {
            setTimeout(this.resetPettingCount, 3000);
        }
    }

    resetIsPettingCount() {
        let newIsPettingCount = this.state.isPetting - 1;
        this.setState({
            isPetting: newIsPettingCount
        });
    }

    resetPettingCount() {
        this.setState({
            pettingCount: 0
        });
    }

    renderDino() {
        let className = "";
        if (this.state.isPetting > 0) {
            className = "petting";
        }

        let dinoSrc = `/dino-${this.state.health}.png`;
        if (this.state.pettingCount > 2) {
            dinoSrc = `/dino-kiss.png`;
        }

        return (
            <img src={dinoSrc} onClick={this.petDino} className={className} />
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
                <h3 className="transactions-title">
                    Why your dinopet is <span className="green">{this.getHealthDescription()}</span>
                </h3>
                <div className="transactions">
                    {this.state.recentBonusTransactions.map(this.getTransaction)}
                </div>
            </div>
        );
    }

    getGoal(goal: IPetGoalSummary) {
        if (goal.onTarget) {
            return (
                <div className="transaction positive" key={goal.goal.id}>
                    <span className="thumb green">
                        <i className="fas fa-thumbs-up"></i>
                    </span> &nbsp; {goal.goal.name}
                </div>
            );
        }
        return (
            <div className="transaction negative" key={goal.goal.id}>
                <span className="thumb red">
                    <i className="fas fa-thumbs-down"></i>
                </span> &nbsp; {goal.goal.name}
            </div>
        );
    }

    getGoals() {
        return (
            <div>

                <div className="right">
                    <a className="edit-btn" href="/add-goal">add goal</a>
                </div>
                <h3 className="transactions-title">
                    Your goals
                </h3>
                <div className="transactions">
                    {this.state.goals.map(this.getGoal)}
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
                    <div className="col-sm-8 col-sm-offset-2 text-center">
                        <h2 className="dino-desc">Your dinopet is <span className="green">{this.getHealthDescription()}</span></h2>
                    </div>
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
                        {this.getGoals()}
                    </div>
                </div>
                <div className='row'>
                    <div className="col-sm-10 col-sm-offset-1">
                        {this.getTransactions()}
                    </div>
                    <div className="right">
                        <a className="edit-btn" href="/categories">edit categories</a>
                    </div>
                </div>

            </div>
        );
    }
}