import * as React from 'react';
import * as moment from 'moment';
import { RouteComponentProps } from 'react-router';

interface IAddGoalState {
    goalName: string,
    transactions: string,
    goalType: string,
    limit: number,
    frequency: string
}

export class AddGoal extends React.Component<RouteComponentProps<{}>, IAddGoalState> {
    constructor() {
        super();

        this.state = {
            goalName: '',
            transactions: '',
            goalType: 'Under',
            limit: 10,
            frequency: 'Daily'
        };

        this.onGoalNameChange = this.onGoalNameChange.bind(this);
        this.onTransactionsChange = this.onTransactionsChange.bind(this);
        this.onGoalTypeChange = this.onGoalTypeChange.bind(this);
        this.onLimitChange = this.onLimitChange.bind(this);
        this.onFrequencyChange = this.onFrequencyChange.bind(this);
        this.save = this.save.bind(this);
    }

    onGoalNameChange(event: React.FormEvent<HTMLInputElement>) {
        this.setState({
            goalName: event.currentTarget.value
        });
    }
    onTransactionsChange(event: React.FormEvent<HTMLInputElement>) {
        this.setState({
            transactions: event.currentTarget.value
        });
    }
    onGoalTypeChange(event: React.FormEvent<HTMLSelectElement>) {
        this.setState({
            goalType: event.currentTarget.value
        });
    }
    onLimitChange(event: React.FormEvent<HTMLInputElement>) {
        this.setState({
            limit: +event.currentTarget.value
        });
    }
    onFrequencyChange(event: React.FormEvent<HTMLSelectElement>) {
        this.setState({
            frequency: event.currentTarget.value
        });
    }

    renderButton() {
        let isValid = this.state.goalName && this.state.transactions && this.state.frequency && this.state.limit && this.state.goalType;
        if (isValid) {
            return (
                <button className="btn btn-primary" onClick={this.save}>Save Goal</button>
            );
        } else {
            return (
                <button className="btn btn-primary disabled" disabled>Save Goal</button>
            );
        }
    }

    cancel(event: any) {
        event.preventDefault();
        window.location.href = "/";
    }

    async save(event: any) {
        event.preventDefault();

        let txnTypes = this.state.transactions.split(',');


        let data = {
            name: this.state.goalName,
            matchingTransactions: txnTypes,
            threshold: this.state.limit,
            goalDirection: this.state.goalType,
            frequency: this.state.frequency
        }

        let result = await fetch('/api/dave/goals',
            {
                body: JSON.stringify(data),
                method: 'POST',
                headers: {
                    'content-type': 'application/json'
                }
            });
        window.location.href = "/";
    }


    public render() {
        return (
            <form>
                <h1>Add Goal</h1>
                <div className="form-group">
                    <label>Goal Name</label>
                    <input type="text" className="form-control" placeholder="Spend less on coffee" value={this.state.goalName}
                        onChange={this.onGoalNameChange} />
                    <small className="form-text text-muted">A distinctive name for your goal.</small>
                </div>
                <div className="form-group">
                    <label>Transaction types</label>
                    <input type="text" className="form-control" placeholder="Starbucks,200degrees" value={this.state.transactions}
                        onChange={this.onTransactionsChange}/>
                    <small className="form-text text-muted">A comma separated list of transaction sources.</small>
                </div>
                <div className="form-group">
                    <label>Goal type</label>
                    <select className="form-control" value={this.state.goalType} onChange={this.onGoalTypeChange}>
                        <option value="Under">Spend less than</option>
                        <option value="Over">Spend more than</option>
                    </select>
                    <small className="form-text text-muted">Whether the goal is to spend less than or more than.</small>
                </div>
                <div className="form-group">
                    <label>Amount limit</label>
                    <input type="number" className="form-control" placeholder="5" value={this.state.limit} onChange={this.onLimitChange} />
                    <small className="form-text text-muted">The amount you want to spend less or more than.</small>
                </div>
                <div className="form-group">
                    <label>Frequency</label>
                    <select className="form-control" value={this.state.frequency} onChange={this.onFrequencyChange}>
                        <option value="Daily">Daily</option>
                        <option value="Weekly">Weekly</option>
                    </select>
                    <small className="form-text text-muted">The frequency to match your goal.</small>
                </div>
                {this.renderButton()}

                <button className="btn" onClick={this.cancel}>Cancel</button>
            </form>
        );
    }
}