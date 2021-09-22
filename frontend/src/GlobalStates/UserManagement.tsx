import { observer } from "mobx-react";
import { makeAutoObservable } from "mobx";

class UserInformation {
	private username: string | null = null;
	public getUsername = () => {
		return this.username;
	};


	constructor() {
		makeAutoObservable(this);
	}
}