/* globals $, cytoscape, require, editor */
//function main() {
// #region Cytoscape
let wipCy = [];
let cy = cytoscape({
	container: document.getElementById("cy"), // Container to render in

	elements: [
		{}
	],

	layout: {
		name: "dagre"
	},

	style: [
		// Stylesheet for the graph
		{
			selector: "node",
			style: {
				"background-color": "#888",
				label: "data(label)",
				"text-wrap": "wrap",
				"text-max-width": "80px",
				width: "100px",
				height: "100px",
				"min-zoomed-font-size": "8px",
				"background-width": "70%",
				"background-height": "70%"
			}
		},
		{
			selector: "edge",
			style: {
				width: 3,
				"line-color": "#ccc",
				"target-arrow-color": "#ccc",
				"target-arrow-shape": "triangle"
			}
		},
		{
			selector: ".root",
			style: {
				"background-color": "#4dd926"
			}
		},
		{
			selector: ".leaf",
			style: {
				"background-color": "#d93e26"
			}
		},
		{
			selector: ".composite",
			style: {
				//"background-color": "#d6d926",
			}
		},
		{
			selector: ".tree",
			style: {
				"background-color": "#4dd926"
			}
		},
		{
			selector: ".decorator",
			style: {
				//"background-color": "#d98026",
			}
		},
		{
			selector: ".action",
			style: {
				"background-image": "./img/action.svg"
			}
		},
		{
			selector: ".condition",
			style: {
				"background-image": "./img/condition.svg"
			}
		},
		{
			selector: ".sequence",
			style: {
				"background-image": "./img/sequence.svg"
			}
		},
		{
			selector: ".selector",
			style: {
				"background-image": "./img/selector.svg"
			}
		},
		{
			selector: ".negator",
			style: {
				"background-image": "./img/negator.svg"
			}
		},
		{
			selector: ".repeater",
			style: {
				"background-image": "./img/repeater.svg"
			}
		},
		{
			selector: ".repeatUntilFail",
			style: {
				"background-image": "./img/repeatUntilFail.svg"
			}
		},
		{
			selector: ".succeeder",
			style: {
				"background-image": "./img/succeeder.svg"
			}
		},
		{
			selector: ".timer",
			style: {
				"background-image": "./img/timer.svg"
			}
		}
	],

	// Options
	zoom: 1,
	pan: { x: 0, y: 0 },

	minZoom: 0.1,
	maxZoom: 5,
	zoomingEnabled: true,
	userZoomingEnabled: true,
	panningEnabled: true,
	userPanningEnabled: true,
	boxSelectionEnabled: false,
	selectionType: "single",
	touchTapThreshold: 8,
	desktopTapThreshold: 4,
	autolock: false,
	autoungrabify: true,
	autounselectify: false,

	headless: false,
	styleEnabled: true,
	hideEdgesOnViewport: false,
	hideLabelsOnViewport: false,
	textureOnViewport: false,
	motionBlur: false,
	motionBlurOpacity: 0.2,
	wheelSensitivity: 1,
	pixelRatio: "auto"
});

// Disable node grabbing
cy.autoungrabify();

/**
 * Add a node of the specified type to the specified parent (or headless if undefined)
 * @param {string} nodeId
 * @param {string} parentId
 * @param {string} nodeType
 * @param {string} nodeName
 */
function addNode(nodeId, parentId, nodeType, nodeName) {
	if (nodeType) {
		let classes = "";

		switch (nodeType) {
			case "_":
				classes = "root";
				break;
			case "#":
				classes = "tree";
				break;
			case "!":
				classes = "leaf action";
				break;
			case "?":
				classes = "leaf condition";
				break;
			case "&":
				classes = "composite sequence";
				break;
			case "|":
				classes = "composite selector";
				break;
			case "¬":
				classes = "decorator negator";
				break;
			case "n":
				classes = "decorator repeater";
				break;
			case "*":
				classes = "decorator repeatUntilFail";
				break;
			case "^":
				classes = "decorator succeeder";
				break;
			case '"':
				classes = "decorator timer";
				break;
		}

		wipCy[nodeId] = {
			group: "nodes",
			data: { id: nodeId, label: nodeName },
			classes: classes
		};

		if (parentId) {
			let edgeId = parentId + "to" + nodeId;
			wipCy[edgeId] = {
				group: "edges",
				data: {
					id: edgeId,
					source: parentId,
					target: nodeId
				}
			};
		}

		debug.log(
			`Node ${nodeId} added as a child of ${parentId || "NULL"}`
		);
	}
}

// #endregion

// #region globals
const { ipcRenderer } = require("electron");
const { dialog, app, BrowserWindow } = require("electron").remote;
const fs = require("fs");

const parentNodeTypes = ["&", "|", "¬", "n", "*", "^", '"', "#"];
let pathToFileBeingEdited;

/** Start editing a new file */
function newFile() {
	$("#text-editor")[0].value = "";
	pathToFileBeingEdited = undefined;
	parse();
}

/** Open a file from a specified location */
function open() {
	dialog.showOpenDialog(
		require("electron").remote.getCurrentWindow(),
		{
			filters: [
				{ name: "BTML files", extensions: ["btml"] },
				{ name: "All Files", extensions: ["*"] }
			]
		},
		filenames => {
			if (!filenames) {
				debug.log("No file selected");
				return;
			}
			let filename = filenames[0];

			fs.readFile(filename, "utf-8", (err, data) => {
				if (err) {
					debug.error(
						"An error ocurred reading the file: " + err.message
					);
					return;
				}
				pathToFileBeingEdited = filename;
				// Do stuff with open file
				$("#text-editor")[0].value = data;
				$("#text-editor").change();
			});
		}
	);
}

/** Save file to the path it was loaded from */
function save() {
	if (!pathToFileBeingEdited) {
		saveAs();
	} else {
		let content = $("#text-editor")[0].value;

		fs.writeFile(pathToFileBeingEdited, content, err => {
			if (err) {
				debug.error(
					"An error ocurred creating the file " + err.message
				);
			}

			debug.log("The file has been succesfully saved");
		});
	}
}

/** Save file to a new path */
function saveAs() {
	let content = $("#text-editor")[0].value;

	dialog.showSaveDialog(
		require("electron").remote.getCurrentWindow(),
		{
			filters: [
				{ name: "BTML files", extensions: ["btml"] },
				{ name: "All Files", extensions: ["*"] }
			]
		},
		filename => {
			if (filename === undefined) {
				debug.log("No file selected");
				return;
			}

			fs.writeFile(filename, content, err => {
				if (err) {
					debug.error(
						"An error ocurred creating the file " + err.message
					);
				}
				pathToFileBeingEdited = filename;
				debug.log("The file has been succesfully saved");
			});
		}
	);
}

/** Open the preferences window */
function openPreferences() {
	// TODO: Load and populate user preferences from file
	let preferencesWindow = new BrowserWindow({
		width: 300, height: 400,
		resizable: false
	});

	preferencesWindow.loadFile("preferences.html");

	preferencesWindow.on("closed", () => {
		// Update renderer if needed
	});

	/*preferencesWindow.once("ready-to-show", () => {
        preferencesWindow.show();
    });*/
}

/** Modify graph zoom */
function zoom(type) {
	// TODO
	switch (type) {
		case "increase":
			// TODO
			cy.zoom(cy.zoom() + 1);
			break;

		case "decrease":
			// TODO
			cy.zoom(cy.zoom() - 1);
			break;
	}
	cy.center();
}

// #endregion

// #region Event handlers

/** Update when resizing */
$(window).resize(() => {
	$("#text-editor")[0].style.width = "calc(100% - 8px)";
	$("#text-editor")[0].style.height = "calc(100% - 5px)";
});

/** Update render when text editor loses focus */
$("#text-editor").change(() => {
	parse();
});

/** Update render when text editor receives user input */
$("#text-editor").on("input", () => {
	parse(true);
});

/** Run BTML parser */
$("#output").click(() => {
	let language = JSON.parse(localStorage.getItem("preferences")).languageOutput || "C++";

	let filepath;

	filepath = app.getPath("temp") + "\\" + new Date().getTime() + ".btml";
	fs.writeFile(filepath, $("#text-editor")[0].value, "utf-8", () => {
		outputToFile(language, filepath);
	});

	function outputToFile(l, p) {
		// Run a local copy of the parser
		let executablePath = ".\\BTMLPARSERCPP.exe";
		let parameters = [l, p];
		require("child_process").execFile(
			executablePath,
			parameters,
			(err, stdout) => {
				if (err) {
					debug.error(err);
					return;
				}

				// When the output comes back, if no errors, save that as the dialog
				debug.log(stdout);
				dialog.showSaveDialog(
					{
						filters: [
							{ name: "Text files", extensions: ["txt"] },
							{ name: "All Files", extensions: ["*"] }
						]
					},
					filename => {
						if (filename === undefined) {
							debug.log("No file selected");
							return;
						}

						fs.writeFile(filename, stdout, err => {
							if (err) {
								debug.error(
									"An error ocurred creating the file " +
									err.message
								);
							}
							pathToFileBeingEdited = filename;
							debug.log("The file has been succesfully saved");
						});
					}
				);
			}
		);
	}
});

// Handle events triggered in main window
ipcRenderer.on("new", () => {
	newFile();
});

ipcRenderer.on("open", () => {
	open();
});

ipcRenderer.on("save", () => {
	save();
});

ipcRenderer.on("saveAs", () => {
	saveAs();
});

ipcRenderer.on("preferences", () => {
	openPreferences();
});

ipcRenderer.on("zoom", (e, type) => {
	zoom(type);
});

// #endregion

// #region Parser functions
/**
 * Count the amount of tabs (or group of 4 spaces) in a given string
 * @param {string} s
 */
function countTabs(s) {
	let num = 0;
	while (s[0] === "\t" || s.substr(0, 4) === "    ") {
		s = s.substr(1);
		num++;
	}
	return num;
}

/**
 * Find nodes in content and add them to parent. If a subtree node
 * is found, it will try to load that file and append the nodes to
 * the tree
 * @param {string} content
 * @param {string} parentId
 * @param {string} parentType
 */
function addNodesToParent(content, parentId, parentType, isRealTimeParsing) {
	let tabNum = 0;
	let nodeType;
	let nodeName;
	let parents = [{ id: parentId, childNo: 0, type: parentType }];
	let parent;

	let lines = content.split("\n");

	let nodeId;

	lines.forEach(line => {
		if (line.trim()) {
			// Make sure we replace spaces and tabs
			// Might add as an option
			line = line.replace(/ {4}/g, "\t");

			tabNum = countTabs(line);
			while (tabNum < parents.length - 1) {
				// We finished in this level, so go back to the previous parent
				let lastParent = parents.pop();
				// Check if the last node type (since we haven't updated it yet) was expecting a child
				if (parentNodeTypes.includes(nodeType) && !isRealTimeParsing) {
					debug.error(`Parent ${lastParent.id} with type ${lastParent.type} has no child. Tree might end up malformed`);
					return;
				}
			}

			parent = parents[parents.length - 1];
			nodeId =
				parent.id === "ROOT" ? "0" : parent.id + "." + parent.childNo;

			// Adding a child to this parent
			parents[parents.length - 1].childNo++;

			// Get string until first space, that should be the node symbol
			nodeType = line.substring(0, line.indexOf(" ")).trim();

			if ($.isNumeric(nodeType)) nodeType = "n";

			nodeName = line.substring(line.indexOf(" ") + 1);

			// Add a node to the node graph
			addNode(nodeId, parent.id, nodeType, nodeName);

			// Make sure we push this parent into the list for next child
			if (parentNodeTypes.includes(nodeType)) {
				parents.push({ id: nodeId, childNo: 0, type: nodeType });
			}

			if (nodeType === "#" && pathToFileBeingEdited) {
				let subtreeFilename =
					pathToFileBeingEdited.substr(
						0,
						pathToFileBeingEdited.lastIndexOf("\\") + 1
					) + nodeName;
				if (fs.existsSync(subtreeFilename)) {
					let content = fs.readFileSync(subtreeFilename, "utf-8");
					addNodesToParent(content, nodeId, nodeType, isRealTimeParsing);
				} else {
					//
				}
			}
		}
	});

	// Add all noded and edges as a batch
	cy.add(Object.values(wipCy));

	// Clear memory
	wipCy = {};

	// Update layout
	cy.layout({ name: "dagre", directed: true }).run();
}

/**
 * Format the text and create a map according to it
 * @param {boolean} isRealTimeParsing
 */
function parse(isRealTimeParsing = false) {
	cy.elements().remove();
	addNode("ROOT", undefined, "_", "ROOT");
	addNodesToParent($("#text-editor")[0].value, "ROOT", "_", isRealTimeParsing);
}

// #endregion

// #region Debug functions
let debug = {
	isDebugging: true,
	error: message => {
		if (this.isDebugging)
			console.error(message); // eslint-disable-line
	},

	log: message => {
		console.log(message); // eslint-disable-line
	}
};

// #endregion

newFile();
//}

//main();
