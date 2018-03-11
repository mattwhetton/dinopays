import * as React from 'react';
import * as moment from 'moment';
import { RouteComponentProps } from 'react-router';

interface IEditCategoriesState {
    categories: ICategoryStatus[]
}

interface ICategoryStatus {
    name: string,
    status: string
}

export class EditCategories extends React.Component<RouteComponentProps<{}>, IEditCategoriesState> {
    constructor() {
        super();

        this.state = {
            categories: [
                {
                    name: "NONE",
                    status: "Neutral"
                }, {
                    name: "BILLS_AND_SERVICES",
                    status: "Neutral"
                }, {
                    name: "EATING_OUT",
                    status: "Neutral"
                }, {
                    name: "ENTERTAINMENT",
                    status: "Neutral"
                }, {
                    name: "EXPENSES",
                    status: "Neutral"
                }, {
                    name: "GENERAL",
                    status: "Neutral"
                }, {
                    name: "GROCERIES",
                    status: "Neutral"
                }, {
                    name: "SHOPPING",
                    status: "Neutral"
                }, {
                    name: "HOLIDAYS",
                    status: "Neutral"
                }, {
                    name: "PAYMENTS",
                    status: "Neutral"
                }, {
                    name: "TRANSPORT",
                    status: "Neutral"
                }, {
                    name: "LIFESTYLE",
                    status: "Neutral"
                }
            ]
        };

        this.renderItems = this.renderItems.bind(this);
        this.renderItem = this.renderItem.bind(this);
        this.save = this.save.bind(this);
    }

    async componentDidMount() {
        let response = await fetch('/api/dave/categories');
        let data = await response.json() as ICategoryStatus[];
        data.forEach(item => {
            var item1 = this.state.categories.filter(i => i.name === item.name);
            item1[0].status = item.status;
        });
        this.setState({
            categories: this.state.categories
        });
    }


    renderButton() {
        return (
            <button className="btn btn-primary" onClick={this.save}>Save Categories</button>
        );

    }

    cancel(event: any) {
        event.preventDefault();
        window.location.href = "/";
    }

    itemChanged(itemName: string, event: any) {
        var item = this.state.categories.filter(i => i.name === itemName);
        item[0].status = event.currentTarget.value;
        this.setState({
            categories: this.state.categories
        });
    }

    renderItem(item: ICategoryStatus) {

        return (
            <div className="form-group" key={item.name}>
                <label className="fixed-label">{item.name}</label>
                <select className="bigger" value={item.status} onChange={this.itemChanged.bind(this, item.name)}>
                    <option value="Positive">Good thing</option>
                    <option value="Neutral">Neutral thing</option>
                    <option value="Negative">Bad thing</option>
                </select>
            </div>
        );
    }

    renderItems() {
        return this.state.categories.map(this.renderItem);
    }

    async save(event: any) {
        event.preventDefault();
        let data = this.state.categories;

        let result = await fetch('/api/dave/categories',
            {
                body: JSON.stringify(data),
                method: 'PUT',
                headers: {
                    'content-type': 'application/json'
                }
            });
        window.location.href = "/";
    }


    public render() {
        return (
            <form>
                <h1>Edit Categories</h1>
                {this.renderItems()}

                {this.renderButton()}

                <button className="btn" onClick={this.cancel}>Cancel</button>
            </form>
        );
    }
}