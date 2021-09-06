import React from "react";
import { getAccessTokenJWT } from "src/Authentication/AccessToken";
import SequencesEntity from "src/Models/SequencesEntity";

import "./SideBarSequenceAccessors.scss";


interface ISideBarSequenceAccessorsProps {

}

interface SideBarSequenceAccessorsStates {
	isLoading: boolean;
	sequences: SequencesEntity[] | null;
}

class SideBarSequenceAccessors extends React.Component<ISideBarSequenceAccessorsProps, SideBarSequenceAccessorsStates> {

	constructor(props: any) {
		super(props);
		this.state = {
			isLoading: true,
			sequences: []
		};
	}

	suequenceFetcher = () => {
		let resultSequenceList: SequencesEntity[] | null = null;
		const requestOptions = {
			method: "GET",
			headers: {
				'Content-Type': 'application/json',
				"Authorization": getAccessTokenJWT()
			}
		};
		fetch("https://localhost:5001/api/naturaldna", requestOptions)
			.then((_response) => _response.json())
			.then((_data) => {
				console.log(_data);
				let resultSequenceList: SequencesEntity[] = [];
				for (let i: number = 0; i < _data.length; i++) {
					resultSequenceList.push(_data[i]);
				}
				console.log(resultSequenceList);

				this.setState({
					sequences: resultSequenceList
				});

				console.log(this.state.sequences);
			});

	};


	componentDidMount() {
		this.suequenceFetcher();

		this.setState({ isLoading: false });
	}

	render() {
		if (this.state.isLoading) {
			return (
				<div className="Sequences side-bar">
					<h3>Loading...</h3>
				</div>
			);
		}
		else {
			return (
				<aside>
					<h2>Sequences</h2>
					<ul>
						{this.state.sequences?.map((_sequenceEntity: SequencesEntity) => {
							return (
								<li key={_sequenceEntity.Id}>
									<h5>{_sequenceEntity.sequenceName}</h5>
									{/* Here we could have some basic details abbout each entry. */}
								</li>);
						})}
					</ul>
				</aside>
			);
		}
	}
}

export default SideBarSequenceAccessors;