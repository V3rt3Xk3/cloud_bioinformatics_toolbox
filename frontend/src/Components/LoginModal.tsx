import React from "react";

import "./../Style/Components/LoginModal.scss";

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
					<h4>Login</h4>
					<form>
						<input type="text" name="userName" className="username form-control" placeholder="Username" value={this.state.userName} onChange={this.handleChange} />
						<input type="password" name="password" className="password form-control" placeholder="password" value={this.state.password} onChange={this.handleChange} />
						<button id="login-form-close-trigger" type="submit">Login</button>
						<button type="button" onClick={() => { this.onClose(); }}>Close</button>
					</form>
				</div >
			);
		}
	}
}