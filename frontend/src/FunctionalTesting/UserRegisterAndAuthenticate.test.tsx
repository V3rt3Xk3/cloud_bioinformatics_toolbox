import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import App from "../App";
import LoginModal from "../Components/LoginModal";

const signUpText = "Register";
const singInText = "Sign in";


// Actual tests
describe("User registration and authentication", () => {
	test('Registering in as "vertex"', () => {
		const component = render(<App />);
		const registerButton = component.getByText(signUpText);
	});
	test('Logging in as "vertex"', () => {
		const component = render(<App />);
		const loginButton = component.getByText(singInText);

		fireEvent.click(loginButton);

		const usernameInput = component.getByTestId("loginModal.usernameInput");
		fireEvent.change(usernameInput, {
			target: {
				value: "vertex"
			}
		});
		const passwordInput = component.getByTestId("loginModal.passwordInput");
		fireEvent.change(passwordInput, {
			target: {
				value: "#33FalleN666#"
			}
		});
		const submitButton = component.getByTestId("loginModal.submit");


	});
});