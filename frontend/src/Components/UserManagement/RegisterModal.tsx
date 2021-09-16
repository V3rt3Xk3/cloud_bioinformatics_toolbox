import React from "react";
import { Route } from "react-router";
import { setAccessTokenJWT } from "src/Authentication/AccessToken";

import "./RegisterModal.scss";

interface IRegisterModalProperties {
	show: boolean;
	onClose: () => void;
}

interface IRegisterModalState {
	isLoading: boolean;
	userName: string;
	password: string;
	rePassword: string;
}

export default class RegisterModal extends React.Component<IRegisterModalProperties, IRegisterModalState> {

	constructor(props: any) {
		super(props);
		this.state = {
			isLoading: true,
			userName: "",
			password: "",
			rePassword: ""
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

		const newState = { [name]: value } as Pick<IRegisterModalState, keyof IRegisterModalState>;

		this.setState(newState);
	};

	formSubmit = (_event: any) => {
		// NOTE: the next line prevent page refresh.
		_event.preventDefault();
		const requestOptions = {
			method: "POST",
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({
				username: this.state.userName,
				password: this.state.password
			})
		};

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
					<h4 data-testid="registerModal.header">Login</h4>
					<form onSubmit={this.formSubmit}>
						<input type="text" name="userName" className="username form-control" placeholder="Username" value={this.state.userName} onChange={this.handleChange} data-testid="registerModal.usernameInput" />
						<input type="password" name="password" className="password form-control" placeholder="Password" value={this.state.password} onChange={this.handleChange} data-testid="registerModal.passwordInput" />
						<input type="password" name="rePassword" className="rePassword form-control" placeholder="Repeat password" value={this.state.rePassword} onChange={this.handleChange} data-testid="registerModal.rePasswordInput" />
						<div>
							<button id="login-form-close-trigger" type="submit" data-testid="registerModal.submit">Register</button>
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