import React from 'react';
import {
	BrowserRouter as Router,
	Switch,
	Route,
	Link
} from "react-router-dom";

import Home from "./Pages/Home";
import UserProfile from "./Pages/UserProfile";
import UserRegistration from "./Pages/UserRegistration";

// Misc
import logo from './logo.svg';
import './App.scss';
import "./SharedStyle/TopNavBar.scss";
import "./SharedStyle/AppLoading.scss";
// Login Modal
import LoginModal from "./Components/LoginModal";

interface IAppProperties {

}

interface IAppState {
	loading: boolean,
	showLoginModal: boolean;
}

class App extends React.Component<IAppProperties, IAppState> {

	constructor(properties: any) {
		super(properties);
		this.state = {
			loading: true,
			showLoginModal: false
		};
	}

	setShowLoginModal = () => {
		this.setState({
			showLoginModal: !this.state.showLoginModal
		});
	};

	componentDidMount() {
		this.setState({ loading: false });
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
							<ul className="navbar-nav">
								<li>
									<Link to="/">Home</Link>
								</li>
								<li>
									<Link to="/user/profile/">User profile</Link>
								</li>
								<li>
									<Link to="/user/registration/">User registration</Link>
								</li>
								<li>
									<a id="login-modal-trigger" href="#" onClick={_event => { this.setShowLoginModal(); }}>Login</a>
								</li>
							</ul>
						</div>
					</nav>
					<LoginModal show={this.state.showLoginModal} onClose={this.setShowLoginModal} />
					<Switch>
						<Route path="/user/profile/">
							<UserProfile />
						</Route>
						<Route path="/user/registration/">
							<UserRegistration />
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
