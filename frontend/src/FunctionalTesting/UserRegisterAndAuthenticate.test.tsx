import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import App from "../App";

const signUpText = "Register";
const singInText = "Sign in";


// Actual tests
describe("User registration and authentication", () => {
	test('Registering in as "vertex"', () => {
		const component = render(<App />);
		const registerButton = component.getByText(signUpText);

		fireEvent.click(registerButton);

		const usernameInput = component.getByPlaceholderText("Username");
		fireEvent.change(usernameInput, {
			target: {
				value: "vertex"
			}
		});
		const passwordInput = component.getByPlaceholderText("Password");
		fireEvent.change(passwordInput, {
			target: {
				value: "#33FalleN666#"
			}
		});
		const rePasswordInput = component.getByPlaceholderText("Repeat password");
		fireEvent.change(rePasswordInput, {
			target: {
				value: "#33FalleN666#"
			}
		});

		const submitButton = component.getByTestId("registerModal.submit");
	});
	test('Logging in as "vertex"', () => {
		const component = render(<App />);
		const loginButton = component.getByText(singInText);

		fireEvent.click(loginButton);

		const usernameInput = component.getByPlaceholderText("Username");
		fireEvent.change(usernameInput, {
			target: {
				value: "vertex"
			}
		});
		const passwordInput = component.getByPlaceholderText("Password");
		fireEvent.change(passwordInput, {
			target: {
				value: "#33FalleN666#"
			}
		});
		const submitButton = component.getByTestId("loginModal.submit");


	});
});