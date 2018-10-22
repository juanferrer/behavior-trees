/* globals $, cytoscape, require  */

// #region Cytoscape
//function main() {
var cy = cytoscape({
	container: document.getElementById("cy"), // container to render in

	elements: [
		// list of graph elements to start with
		{
			// Root node
			data: { id: "ROOT" }
		}
	],

	style: [
		// the stylesheet for the graph
		{
			selector: "node",
			style: {
				"background-color": "#666",
				label: "data(label)",
				"text-wrap": "wrap",
				"text-max-width": "25px"
			},
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
			selector: ".action",
			style: {
				"background-color": "#ff50f0",
				"border-color": "#00ff00"
			},
		},
		{
			selector: ".root",
			style: {
				"background-color": "#ff50f0",
				"border-color": "#00ff00"
			},
		}
	],

	// initial viewport state:
	zoom: 1,
	pan: { x: 0, y: 0 },

	// interaction options:
	minZoom: 0.5,
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

	// rendering options:
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

cy.autoungrabify();

function addNode(nodeId, parentId, nodeType, nodeName) {

	var classes = "";

	switch (nodeType) {
		case "":
			break;
	}

	var node = cy.add({
		group: "nodes",
		data: { id: nodeId, label: nodeName },
		classes: classes
	});

	if (parentId) {
		var edge = cy.add({
			group: "edges",
			data: {
				id: parentId + "to" + nodeId,
				source: parentId,
				target: nodeId
			}
		});
	}

	log(`Node ${node.data.id} added as a child of ${(edge ? (edge.data.parentId || "NULL") : "NULL")}`);

	var layout = cy.layout({ name: "breadthfirst", directed: true, roots: "#ROOT" });
	layout.run();
}

// #endregion

// #region Event handlers
$(window).resize(() => {
	$("#text-editor")[0].style.width = "calc(100% - 8px)";
	$("#text-editor")[0].style.height = "calc(100% - 5px)";
});

$("#text-editor").change(() => {
	parse();
});

/**
 * Run BTML parser
 */
$("#output").click(() => {
	// TODO
	const { app } = require("electron");
	const { fs } = require("fs");

	var tempfile = app.getPath("temp") + "\\" + (new Date()).getTime() + ".btml";

	fs.writeFile(tempfile, $("#text-editor")[0].value, "utf-8", () => {
		var child = require("child_process").execFile;
		var executablePath = ".\\BTMLPARSERCPP.exe";
		var parameters = [tempfile];

		child(executablePath, parameters, function (err, data) {
			if (err) {
				error(err);
				return;
			}

			log(data.toString());
		});
	});
});

/*$("text-editor").keydown(e => {
	// Tab pressed
	if (e.keyCode === 9) { // tab was pressed
		// get caret position/selection
		var start = this.selectionStart;
		var end = this.selectionEnd;

		var $this = $(this);
		var value = $this.val();

		// set textarea value to: text before caret + tab + text after caret
		$this.val(value.substring(0, start)
			+ "\t"
			+ value.substring(end));

		// put caret at right position again (add one for the tab)
		this.selectionStart = this.selectionEnd = start + 1;

		// prevent the focus lose
		e.preventDefault();
	}
});*/

// #endregion

// #region Parser functions
function countTabs(s) {
	var num = 0;
	while (s[0] === "\t" || (s.substr(0, 4) === "    ")) {
		s = s.substr(1);
		num++;
	}
	return num;
}

/**
 * Format the text and create a map according to it
 */
function parse() {
	cy.elements().remove();
	addNode("ROOT", "", "_", "Base");

	var tabNum = 0;
	var nodeType;
	var nodeName;
	var parents = [{ id: "ROOT", childNo: 0, type: "_" }];
	var parent;

	var lines = $("#text-editor")[0].value.split("\n");

	var nodeId;

	lines.forEach(line => {
		if (line) {

			// Make sure we replace spaces and tabs
			// Might add as an option
			line = line.replace(/ {4}/g, "\t");

			tabNum = countTabs(line);
			while (tabNum < parents.length - 1) {
				// We finished in this level, so go back to the previous parent
				parents.pop();
			}

			parent = parents[parents.length - 1];
			nodeId = parent.id === "ROOT" ? "0" : parent.id + "." + parent.childNo;

			// Adding a child to this parent
			parents[parents.length - 1].childNo++;

			// Get string until first space, that should be the node symbol
			nodeType = line.substring(0, line.indexOf(" ")).trim();
			nodeName = line.substring(line.indexOf(" "));

			// Add a node to the node graph
			addNode(nodeId, parent.id, nodeType, nodeName);

			// Make sure we push this parent into the list for next child
			if (["&", "|", "?", "!", "*", "^", "\""].includes(nodeType)) {
				parents.push({ id: nodeId, childNo: 0, type: nodeType });
			}
		}
	});
}

// #endregion

// #region Utility functions

function error(message) {
	console.error(message); // eslint-disable-line
}

function log(message) {
	console.log(message); // eslint-disable-line
}

// #endregion
/*}

main();*/
