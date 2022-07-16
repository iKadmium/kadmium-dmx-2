import React from "react";
import { Container, Section } from "react-bulma-components";
import "./App.css";

export const App = () => {
	return (
		<Section>
			<Container>
				<h1 className="title">Hello World</h1>
				<p className="subtitle">Uhhh</p>
			</Container>
		</Section>
	);
}

export default App;