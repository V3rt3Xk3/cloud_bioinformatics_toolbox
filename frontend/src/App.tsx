import React from 'react';
import {
	BrowserRouter as Router,
	Switch,
	Route,
	Link
} from "react-router-dom";

import { setAccessTokenJWT, getAccessTokenJWT } from "src/Authentication/AccessToken";
import Home from "./Pages/Home";
import UserProfile from "./Pages/UserProfile";
import Sequences from "./Pages/Sequences";

// Misc
import logo from './logo.svg';
import './App.scss';
import "./SharedStyle/TopNavBar.scss";
import "./SharedStyle/AppLoading.scss";
// Login Modal
import LoginModal from "./Components/UserManagement/LoginModal";
import RegisterModal from './Components/UserManagement/RegisterModal';
import { stringify } from 'querystring';
import { observer } from 'mobx-react';
import { UserManagement, userInformation } from "src/GlobalStates/UserManagement";

interface IAppProperties {
	userInfo: UserManagement;
}

interface IAppState {
	loading: boolean,
	user: string,
	showLoginModal: boolean;
	showRegisterModal: boolean;
}

@observer
class App extends React.Component<IAppProperties, IAppState> {

	constructor(properties: any) {
		super(properties);
		this.state = {
			loading: true,
			user: "User",
			showLoginModal: false,
			showRegisterModal: false
		};
	}

	setShowLoginModal = () => {
		this.setState({
			showLoginModal: !this.state.showLoginModal
		});
		if (this.state.showRegisterModal) this.setShowRegisterModal();
	};
	setShowRegisterModal = () => {
		this.setState({
			showRegisterModal: !this.state.showRegisterModal
		});
		if (this.state.showLoginModal) this.setShowLoginModal();
	};

	componentDidMount() {
		this.setState({ loading: false });

		if (getAccessTokenJWT() == "") {
			fetch("https://localhost:5001/api/users/refresh-token", { method: "POST", credentials: "include" })
				.then((_response) => _response.json())
				.then((_data) => {
					setAccessTokenJWT(_data.AccessToken);
					userInformation.UpdateUserInformation(_data);
				});
		}
	}

	render() {
		if (this.state.loading) {
			return (
				<div className="AppLoading" >
					<header className="App-header">
						<img src={logo} className="App-logo" alt="logo" />
						<p>
							Edit <code>src/App.tsx</code> and save to reload.
						</p>
						<a
							className="App-link"
							href="https://reactjs.org"
							target="_blank"
							rel="noopener noreferrer"
						>
							Learn React
						</a>
					</header>
				</div>
			);
		} else {
			return (
				<Router>
					<nav className="navbar">
						<div className="navbar-menu">
							<ul className="navbar-userinfo">
								<li id="usernameField" data-testid="navBar.username">
									{this.props.userInfo.username}
								</li>
							</ul>
							<ul className="navbar-nav">
								<li>
									<Link to="/">Home</Link>
								</li>
								<li>
									<Link to="/user/profile/">User profile</Link>
								</li>
								<li>
									<Link to="/sequences/">Sequences</Link>
								</li>
								<li>
									<button id="registerModalTrigger" onClick={_event => { this.setShowRegisterModal(); }}>Register</button>
								</li>
								<li>
									<button id="loginModalTrigger" onClick={_event => { this.setShowLoginModal(); }}>Sign in</button>
								</li>
							</ul>
						</div>
					</nav>
					<RegisterModal show={this.state.showRegisterModal} onClose={this.setShowRegisterModal} />
					<LoginModal show={this.state.showLoginModal} onClose={this.setShowLoginModal} />
					<Switch>
						<Route path="/user/profile/">
							<UserProfile />
						</Route>
						<Route path="/sequences/">
							<Sequences />
						</Route>
						<Route path="/">
							<Home />
						</Route>
					</Switch>
				</Router>
			);
		}

	}
}

export default App;
