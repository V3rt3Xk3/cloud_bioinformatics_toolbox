import React from "react";
import { Route } from "react-router";
import { setAccessTokenJWT } from "src/Authentication/AccessToken";

import "./LoginModal.scss";

interface ILoginModalProperties {
	show: boolean;
	onClose: () => void;
}

interface ILoginModalState {
	isLoading: boolean;
	email: string;
	password: string;
}

export default class LoginModal extends React.Component<ILoginModalProperties, ILoginModalState> {

	constructor(props: any) {
		super(props);
		this.state = {
			isLoading: true,
			email: "",
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

	formSubmit = (_event: any) => {
		// NOTE: the next line prevent page refresh.
		_event.preventDefault();
		const requestOptions = {
			method: "POST",
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({
				Email: this.state.email,
				Password: this.state.password
			})
		};

		this.onClose();

		fetch("https://localhost:5001/api/users/authenticate", requestOptions)
			.then((_response) => _response.json())
			.then((_data) => {
				setAccessTokenJWT(_data.AccessToken);
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
					<h4 data-testid="loginModal.header">Login</h4>
					<form>
						<input type="text" name="email" className="form-control" placeholder="Email" value={this.state.email} onChange={this.handleChange} data-testid="loginModal.emailInput" />
						<input type="password" name="password" className="form-control" placeholder="Password" value={this.state.password} onChange={this.handleChange} data-testid="loginModal.passwordInput" />
						<div>
							<button id="login-form-close-trigger" name="submit" type="submit" onClick={(_event) => { this.formSubmit(_event); }} data-testid="loginModal.submit">Login</button>
						</div>
						<div>
							<button type="button" onClick={() => { this.onClose(); }}>Close</button>
						</div>
					</form >
				</div >
			);
		}
	}
}