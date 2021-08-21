import React from "react";

import "./../Style/Components/LoginModal.scss";

interface ILoginModalProperties {
	show: boolean
	onClose: () => void;
}

interface ILoginModalState {

}

export default class LoginModal extends React.Component<ILoginModalProperties, ILoginModalState> {

	onClose = () => {
		this.props.onClose();
	};

	render() {
		if (!this.props.show) {
			return null;
		}
		return (
			<div className="modal-body">
				<h4>Login</h4>
				<form>
					<input type="text" name="username" className="username form-control" placeholder="Username" />
					<input type="password" name="password" className="password form-control" placeholder="password" />
					<button id="login-form-close-trigger" type="submit">Login</button>
					<button type="button" onClick={() => { this.onClose(); }}>Close</button>
				</form>

			</div >
		)
	}
}