# Reindeer

Reindeer is an open-source, work in progress plugin for Grasshopper. It aims to simplify the process of designing, detailing and fabricating timber structures parametrically. The plugin includes tools for defining a timber element, search for details, analyze a structure according to Eurocode 5 and detail a structure using timber processing. 

Please download after carefully reading and accepting the Terms of Use and Disclaimer. 
Password to unzip the software is found at the bottom of the License page.

The reindeer is an animal that lives around the polar circle. Through harsh and ever-changing climates and landscape topologies, he searches for food by sniffing out lichen beneath the snow. Similarly, the reindeer toolkit searches for parametric details using geometric properties. Search, don’t sort: No matter if the topology and typology changes, the toolkit will still find valid details. 

Summary of features:

	●	Defines geometry as elements and nodes. Its relations is automatically generated as details.
	●	Multi-Criteria search of details based on geometric properties. Output is customized for detailing purposes. 
	●	Possible to define an element by multiple sub-elements
	●	Element alignment using various logics
	●	Multi-criteria optimization tools
	●	EuroCode 5 extension to Karamba 3D
	●	Timber processing tools: cut/drill/pocket/tenon/mortise.
	●	BTLx-export and NURBS output from timber detailing


## Development
The plugin is still a Work-In-Progress-version, and might contain bugs. Please use it as is. The development of the plugin is a collaboration between NTNU CSDG and Digital Design Lab, Nikken Sekkei Ltd.
Conceptual Structural Design Group (CSDG) is an interdisciplinary research group based at the Department of Architecture and Technology and the Department of Structural Engineering at the Norwegian University of Science and Technology. 
DDL is a department in Nikken Sekkei Ltd, performing computational design and research & development within the field of digital design in architecture and engineering.
The toolkit is open-source and future collaborations and contributions are welcome.


## version and status

- V.0.5.00 : Initial release. 

## when you encounter a buggy behaviour

Posting the contents of your problem to the Issue on github would be appreciated, although we may not be available to solve the problems.

# Licenses and Copyright

Reindeer is released under the MIT License. Please refer to the textfile named "LISENCE".

# External Libraries

Following Libraries are employed to develop Reindeer.

- JSON.NET https://github.com/JamesNK/Newtonsoft.Json 
- D3 https://github.com/d3/d3


Copyright 2010-2017 Mike Bostock
All rights reserved.

- Parallel Coordinates https://github.com/syntagmatic/parallel-coordinates

Copyright (c) 2012, Kai Chang
All rights reserved.

- BTLx https://design2machine.com/btlx/index.html

# Credit

- Karamba3D
- The Resources from design2machine/btlx has been essential to build the toolkit's components

## Core Features
Defining a timber element 
A timber element (the “blank”) is generated from a guide curve. Global alignment, local alignment, structural properties, cross-section, and a composite component enable flexibility in how a timber element is being defined. The composite component is useful to define an architectural element of multiple manufacturing element, which is often required when creating large glulam structures. 
 
I-beam and Circular cross-section built up by sub-elements

### Detail Search
The toolkit defines a detail as a node and its elements. The core components of the toolkit is the Detail search component and its associated search criteria components. The user defines a detail type and its solution space by inputting relevant criteria. Then the algorithm finds all valid details and outputs nodes and its connecting elements(members, beams, columns, bars) in a neatly organized data structure. The nodes and elements can further be deconstructed into geometric data and metadata. The result is that the algorithm is not dependent on the topology. If the topology changes, the algorithm will still find eventually valid details.

### Timber-processing Tools
The toolkit aims to naturally integrate fabrication data in the design phase. Thus, the toolkit is equipped with a series of timber-processing tools. The toolkit currently offers cutting, drilling, pocket, tenon/mortise. Reindeer outputs both BTLx, a processed NURBS component and the surfaces that have been processed. The result is that the components can be used while designing. When the design is ready, the structure can be immediately transferred to CAM. 

### Multi-Objective Optimization
Reindeer comes with the following optimisation components using meta-heuristic algorithms.
For example, it is possible to optimise the topology and cross section so that the amount of deformation and member weight are minimised.
Single-Objective Optimisation
●	Genetic Algorithm Optimisation : Algorithm that mimics the evolution of living things.
Multi-Objective Optimisation
●	Particle Swarm Optimisation(PSO) : An algorithm that mimics the process of finding food by the flock.
●	Parallel Coordinates Viewer : A viewer to narrow down the desired solution from the group of Pareto-Solutions derived by PSO.
 
