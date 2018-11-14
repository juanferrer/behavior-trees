
/* globals $ */
/**
 * This is the text editor and contains syntax highlighting and other goodies
 */

// TODO:
// When editor created, spawn divs with editor-line class inside div element
// Add events for editor-line class:
// 		- onClick: Show a cursor and create an invisible input right on the cursor. On
//		  input, update the underlying div to show the new character added. If character
// 		  is enter close this div and create a new one under it

window.editor = {
	classes: {
		lineClass: "editor-line",
	},

	/**
	 *
	 * @param {string} elementSelector Selector for element to substitute
	 */
	editor: function (elementSelector) {
		let element = $(elementSelector)[0];
		if (element.nodeName === "DIV") {
			// Remove all children from element
			while (element.firstChild) { element.removeChild(element.firstChild); }
			// Create a single div element from the editor-line class
			let newLine = document.createElement("DIV");
			newLine.classList.add(this.classes.lineClass);
			element.appendChild(newLine);
		} else {
			error(`Unexpected tag for editor element. Expected "DIV", found ${element.nodeName}`);
		}
	}
};

$(`.${window.editor.classes.lineClass}`).click(() => {

});
