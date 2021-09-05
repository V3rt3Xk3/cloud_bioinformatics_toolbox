import React from "react";
import LoginModal from "./LoginModal";
import { render } from "@testing-library/react";

// FIXME: Remove repeats of code. I mean particularly the lambdaFunction.
describe("LoginModal tests", () => {
	test("Header renders correctly with correct content", () => {
		const lambdaFunction = () => { };

		const component = render(<LoginModal show={true} onClose={lambdaFunction} />);
		const loginHeader = component.getByTestId("loginModalHeader");

		expect(loginHeader.textContent).toBe("Login");
	});

	test("Username input has placeholder 'Username'", () => {
		const lambdaFunction = () => { };

		const component = render(<LoginModal show={true} onClose={lambdaFunction} />);
		const usernameInputElement = component.getByTestId("usernameInput");

		expect(usernameInputElement.getAttribute("placeholder")).toBe("Username");
	});

	test("Username input has initial value ''", () => {
		const lambdaFunction = () => { };

		const component = render(<LoginModal show={true} onClose={lambdaFunction} />);
		const usernameInputElement = component.getByTestId("usernameInput");

		expect(usernameInputElement.getAttribute("value")).toBe("");
	});

	test("Password input has placeholder 'Password'", () => {
		const lambdaFunction = () => { };

		const component = render(<LoginModal show={true} onClose={lambdaFunction} />);
		const passwordInputElement = component.getByTestId("passwordInput");

		expect(passwordInputElement.getAttribute("placeholder")).toBe("Password");
	});

	test("Password input has initial value ''", () => {
		const lambdaFunction = () => { };

		const component = render(<LoginModal show={true} onClose={lambdaFunction} />);
		const passwordInputElement = component.getByTestId("passwordInput");

		expect(passwordInputElement.getAttribute("value")).toBe("");
	});
});
