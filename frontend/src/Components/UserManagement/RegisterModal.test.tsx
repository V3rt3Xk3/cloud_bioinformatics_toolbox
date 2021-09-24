import React from "react";
import RegisterModal from "./RegisterModal";
import { fireEvent, render } from "@testing-library/react";

// Shared variables:
const emptyFunction = () => { };

// Actual tests
describe("RegisterModal Unit tests", () => {
	test("Header renders correctly with correct content", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const registerHeader = component.getByTestId("registerModal.header");

		expect(registerHeader.textContent).toBe("Login");
	});

	test("Email input has placeholder 'Email'", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const emailInputElement = component.getByTestId("registerModal.emailInput");

		expect(emailInputElement.getAttribute("placeholder")).toBe("Email");
	});

	test("Email input has initial value ''", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const emailInputElement = component.getByTestId("registerModal.emailInput");

		expect(emailInputElement.getAttribute("value")).toBe("");
	});

	test("Email input element can change its value!", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const emailInputElement = component.getByTestId("registerModal.emailInput");

		expect(emailInputElement.getAttribute("value")).toBe("");

		fireEvent.change(emailInputElement, {
			target: {
				value: "vertex@vertex.hu"
			}
		});

		expect(emailInputElement.getAttribute("value")).toBe("vertex@vertex.hu");
	});



	test("Password input has placeholder 'Password'", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("registerModal.passwordInput");

		expect(passwordInputElement.getAttribute("placeholder")).toBe("Password");
	});

	test("Password input has initial value ''", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("registerModal.passwordInput");

		expect(passwordInputElement.getAttribute("value")).toBe("");
	});

	test("Password input element can change its value!", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("registerModal.passwordInput");

		expect(passwordInputElement.getAttribute("value")).toBe("");

		fireEvent.change(passwordInputElement, {
			target: {
				value: "TestPassword"
			}
		});

		expect(passwordInputElement.getAttribute("value")).toBe("TestPassword");
	});

	test("Repeat password input has placeholder 'Repeat password'", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("registerModal.rePasswordInput");

		expect(passwordInputElement.getAttribute("placeholder")).toBe("Repeat password");
	});

	test("Repeat password input has initial value ''", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("registerModal.rePasswordInput");

		expect(passwordInputElement.getAttribute("value")).toBe("");
	});

	test("Repeat password input element can change its value!", () => {
		const component = render(<RegisterModal show={true} onClose={emptyFunction} />);
		const passwordInputElement = component.getByTestId("registerModal.rePasswordInput");

		expect(passwordInputElement.getAttribute("value")).toBe("");

		fireEvent.change(passwordInputElement, {
			target: {
				value: "TestPassword"
			}
		});

		expect(passwordInputElement.getAttribute("value")).toBe("TestPassword");
	});
});
