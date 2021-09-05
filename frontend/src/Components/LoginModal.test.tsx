import React from "react";
import LoginModal from "./LoginModal";
import { fireEvent, render } from "@testing-library/react";

// Shared variables:
const emptyFunction = () => { };

// Actual tests
describe("LoginModal Unit tests", () => {
	test("Header renders correctly with correct content", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const loginHeader = component.getByTestId("loginModalHeader");

		expect(loginHeader.textContent).toBe("Login");
	});

	test("Username input has placeholder 'Username'", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const usernameInputElement = component.getByTestId("usernameInput");

		expect(usernameInputElement.getAttribute("placeholder")).toBe("Username");
	});

	test("Username input has initial value ''", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const usernameInputElement = component.getByTestId("usernameInput");

		expect(usernameInputElement.getAttribute("value")).toBe("");
	});

	test("Username input element can change its value!", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const usernameInputElement = component.getByTestId("usernameInput");

		expect(usernameInputElement.getAttribute("value")).toBe("");

		fireEvent.change(usernameInputElement, {
			target: {
				value: "TestUsername"
			}
		});

		expect(usernameInputElement.getAttribute("value")).toBe("TestUsername");
	});



	test("Password input has placeholder 'Password'", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("passwordInput");

		expect(passwordInputElement.getAttribute("placeholder")).toBe("Password");
	});

	test("Password input has initial value ''", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("passwordInput");

		expect(passwordInputElement.getAttribute("value")).toBe("");
	});

	test("Password input element can change its value!", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("passwordInput");

		expect(passwordInputElement.getAttribute("value")).toBe("");

		fireEvent.change(passwordInputElement, {
			target: {
				value: "TestPassword"
			}
		});

		expect(passwordInputElement.getAttribute("value")).toBe("TestPassword");
	});
});
