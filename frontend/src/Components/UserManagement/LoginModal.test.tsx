import React from "react";
import LoginModal from "./LoginModal";
import { fireEvent, render } from "@testing-library/react";

// Shared variables:
const emptyFunction = () => { };

// Actual tests
describe("LoginModal Unit tests", () => {
	test("Header renders correctly with correct content", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const loginHeader = component.getByTestId("loginModal.header");

		expect(loginHeader.textContent).toBe("Login");
	});

	test("Email input has placeholder 'Email'", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const emailInputElement = component.getByTestId("loginModal.emailInput");

		expect(emailInputElement.getAttribute("placeholder")).toBe("Email");
	});

	test("Email input has initial value ''", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const emailInputElement = component.getByTestId("loginModal.emailInput");

		expect(emailInputElement.getAttribute("value")).toBe("");
	});

	test("Email input element can change its value!", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const emailInputElement = component.getByTestId("loginModal.emailInput");

		expect(emailInputElement.getAttribute("value")).toBe("");

		fireEvent.change(emailInputElement, {
			target: {
				value: "vertex@vertex.hu"
			}
		});

		expect(emailInputElement.getAttribute("value")).toBe("vertex@vertex.hu");
	});



	test("Password input has placeholder 'Password'", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("loginModal.passwordInput");

		expect(passwordInputElement.getAttribute("placeholder")).toBe("Password");
	});

	test("Password input has initial value ''", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("loginModal.passwordInput");

		expect(passwordInputElement.getAttribute("value")).toBe("");
	});

	test("Password input element can change its value!", () => {
		const component = render(<LoginModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("loginModal.passwordInput");

		expect(passwordInputElement.getAttribute("value")).toBe("");

		fireEvent.change(passwordInputElement, {
			target: {
				value: "TestPassword"
			}
		});

		expect(passwordInputElement.getAttribute("value")).toBe("TestPassword");
	});
});
