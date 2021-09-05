import React from "react";

import "./LoginModal.scss";

interface ILoginModalProperties {
	show: boolean;
	onClose: () => void;
}

interface ILoginModalState {
	isLoading: boolean;
	userName: string;
	password: string;
}

export default class LoginModal extends React.Component<ILoginModalProperties, ILoginModalState> {

	constructor(props: any) {
		super(props);
		this.state = {
			isLoading: true,
			userName: "",
			password: ""
		};
	}

	componentDidMount() {
		this.setState({ isLoading: false });
	}

	onClose = () => {
		this.props.onClose();
	};

	handleChange = (_event: any) => {
		const eventTarget = _event.target;
		const value = eventTarget.type === "checkbox" ? eventTarget.checked : eventTarget.value;
		const name: string = eventTarget.name;

		const newState = { [name]: value } as Pick<ILoginModalState, keyof ILoginModalState>;

		this.setState(newState);
	};

	formSubmit = () => {
		const requestOptions = {
			method: "POST",
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({
				username: this.state.userName,
				password: this.state.password
			})
		};
		fetch("https://localhost:5001/api/users/authenticate", requestOptions)
			.then((_response) => _response.json())
			.then((_data) => console.log(_data));

		this.props.onClose();
	};

	render() {
		if (!this.props.show) {
			return null;
		}
		else if (this.state.isLoading) {
			return (
				<div className="modal-body">
					<h3>Loading</h3>
				</div>
			);
		} else {
			return (
				<div className="modal-body">
					<h4 data-testid="loginModalHeader">Login</h4>
					<form onSubmit={this.formSubmit}>
						<input type="text" name="userName" className="username form-control" placeholder="Username" value={this.state.userName} onChange={this.handleChange} data-testid="usernameInput" />
						<input type="password" name="password" className="password form-control" placeholder="Password" value={this.state.password} onChange={this.handleChange} data-testid="passwordInput" />
						<div>
							<button id="login-form-close-trigger" type="submit">Login</button>
						</div>
						<div>
							<button type="button" onClick={() => { this.onClose(); }}>Close</button>
						</div>
					</form>
				</div >
			);
		}
	}
}