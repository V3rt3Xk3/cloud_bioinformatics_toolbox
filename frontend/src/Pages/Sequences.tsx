import React from "react";
import { getAccessTokenJWT } from "src/Authentication/AccessToken";
import SideBarSequenceAccessors from "src/Components/SequencesComponents/SideBarSequenceAccessors";
import SequencesEntity from "src/Models/SequencesEntity";


interface ISequencesProps {

}

interface ISequencesStates {
	isLoading: boolean;
	sequences: SequencesEntity[] | null;
}

class Sequences extends React.Component<ISequencesProps, ISequencesStates> {

	constructor(props: any) {
		super(props);
		this.state = {
			isLoading: true,
			sequences: []
		};
	}



	componentDidMount() {
		this.setState({ isLoading: false });
	}

	render() {
		if (this.state.isLoading) {
			return (
				<div className="Sequences">
					<h3>Loading...</h3>
				</div>
			);
		}
		else {
			return (
				<div className="Sequences main-container" >

					<SideBarSequenceAccessors />

					<main>

					</main>
				</div >
			);
		}
	}
}

export default Sequences;