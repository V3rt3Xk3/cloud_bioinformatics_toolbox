import { observer } from "mobx-react";
import { autorun, makeAutoObservable, observable } from "mobx";

export class UserManagement {
	public username: string | null = null;
	public getUsername = () => {
		return this.username;
	};


	constructor() {
		makeAutoObservable(this, {
			username: observable
		});
	}

	public UpdateUserInformation(_jsonResponse: any) {
		if (_jsonResponse["Username"] != null) this.username = _jsonResponse["Username"];
		else this.username = _jsonResponse["Email"];
	}
}

export const userInformation = new UserManagement();