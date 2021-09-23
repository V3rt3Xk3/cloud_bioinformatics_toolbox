export interface IUserInfromationResponse {
	username: string | null;
}


class UserInfromation implements IUserInfromationResponse {
	public username: string | null;

	constructor() {
		this.username = null;
	}

	public UserInfoFiller(_jsonData: any) {
		this.username = _jsonData["Username"];
	}
}

export default UserInfromation;