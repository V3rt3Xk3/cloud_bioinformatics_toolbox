import { observer } from "mobx-react";
import { autorun, makeAutoObservable, observable } from "mobx";

class UserInformation {
	public username: string | null = null;
	public getUsername = () => {
		return this.username;
	};


	constructor() {
		makeAutoObservable(this, {
			username: observable
		});
	}

	public UpdateUserInformation(_jsonData: any) {


	}
}