export interface ISequencesEntity {
	Id: string;
	sequenceName: string;
	sequence: string;
}


class SequencesEntity implements ISequencesEntity {
	public Id: string;
	public sequenceName: string;
	public sequence: string;

	constructor(_jsonData: any) {
		this.Id = _jsonData["Id"];
		this.sequenceName = _jsonData["sequenceName"];
		this.sequence = _jsonData["sequence"];
	}
}

export default SequencesEntity;