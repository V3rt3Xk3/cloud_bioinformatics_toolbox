let accessTokenJWT: string;

export const setAccessTokenJWT = (value: string) => {
	accessTokenJWT = value;
};

export const getAccessTokenJWT = () => {
	return accessTokenJWT;
};