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
	email: string;
	password: string;
	rePassword: string;
}

export default class RegisterModal extends React.Component<IRegisterModalProperties, IRegisterModalState> {

	constructor(props: any) {
		super(props);
		this.state = {
			isLoading: true,
			email: "",
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
				Email: this.state.email,
				Password: this.state.password,
				RePassword: this.state.rePassword
			})
		};

		this.onClose();

		fetch("https://localhost:5001/api/users/register", requestOptions)
			.then((_response) => _response.json())
			.then((_data) => {
				if (_data.message) console.log("Succesful registration!");
			});
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
					<form onSubmit={this.formSubmit} data-testid="registerModal.form">
						<input type="text" name="email" className="form-control" placeholder="Email" value={this.state.email} onChange={this.handleChange} data-testid="registerModal.emailInput" />
						<input type="password" name="password" className="form-control" placeholder="Password" value={this.state.password} onChange={this.handleChange} data-testid="registerModal.passwordInput" />
						<input type="password" name="rePassword" className="form-control" placeholder="Repeat password" value={this.state.rePassword} onChange={this.handleChange} data-testid="registerModal.rePasswordInput" />
						<div>
							<button id="register-form-close-trigger" name="submit" type="submit" data-testid="registerModal.submit">Register</button>
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