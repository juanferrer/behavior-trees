// #region Cytoscape
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
                label: "data(id)"
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
			}
		}
    ],

    layout: {
        name: "grid",
        rows: 1
    },

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

function addNode(nodeId, parentId, nodeType) {
	
	var classes = "";

	switch (nodeType){

	}

	var node = cy.add({
		group: "nodes",
		data: {id: nodeId, parent: parentId},
		classes: classes
	});
}

// #endregion

// #region Event handlers
$(window).resize(() => {
    $("#text-editor")[0].style.width = "calc(100% - 8px)";
    $("#text-editor")[0].style.height = "calc(100% - 5px)";
});

$("#text-editor").change(e => {
    console.log("Text changed");
});

// #endregion

// #region Parser functions
function countTabs(s) {
    var num = 0;
    while (s[0] == "\t") {
        s = s.substr(1);
        num++;
    }
    return num;
}

function parse() {
    var separator = " ";
    var line = "";

    var currentLevel = 0;
    var levelsOpen = 0;
    var tabNum = 0;
    var type = "";
    var parts = [];

    while (getline(ifile, line)) {
        tabNum = countTabs(line);
        if (0 < tabNum && tabNum <= currentLevel) {
            output += "\n.End()";
            levelsOpen--;
        }

        if (type == "#") {
            output += '\n.Do("' + parts[1] + '", ' + getLambda("action") + ")";
        } else if (type == "&") {
            output += '\n.Sequence("' + parts[1] + '")';
            currentLevel = tabNum;
            levelsOpen++;
        } else if (type == "|") {
            output += '\n.Selector("' + parts[1] + '")';
            currentLevel = tabNum;
            levelsOpen++;
        } else if (type == "?") {
            output +=
                '\n.If("' + parts[1] + '",' + getLambda("condition") + ")";
        } else if (type == "!") {
            output += '\n.Not("' + parts[1] + '")';
        } else if (isNumber(type)) {
            var result = stoi(type);
            output +=
                '\n.Repeat("' +
                parts[1] +
                (result > 0 ? '", ' + type : "") +
                ")";
        } else if (type == "*") {
            output += '\n.RepeatUntilFail("' + parts[1] + '")';
        } else if (type == "^") {
            output += '\n.Ignore("' + parts[1] + '")';
        } else if (type == '"') {
            output += '\n.Wait("' + parts[1] + '", 0)';
        } else {
            // Do nothing
        }
    }
    ifile.close();
    while (levelsOpen > 0) {
        output += "\n.End()";
        levelsOpen--;
    }
}

// #endregion
