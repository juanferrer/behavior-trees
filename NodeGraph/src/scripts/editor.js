
/* globals $, debug */
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
	 * Construct editor and return the newly created object
	 * @param {string} elementSelector Selector for element to substitute
	 */
	editor: function (elementSelector) {
		this.element = $(elementSelector)[0];
		if (this.element.nodeName === "DIV") {
			// Remove all children from element
			while (this.element.firstChild) { element.removeChild(this.element.firstChild); }
			// Create a single div element from the editor-line class
			let newLine = document.createElement("DIV");
			newLine.classList.add(this.classes.lineClass);
			this.element.appendChild(newLine);
		} else {
			debug.error(`Unexpected tag for editor element. Expected "DIV", found ${this.element.nodeName}`);
			return;
		}
		return this;
	}

	/**
	 * Get the text inside the line
	 * @param {HTMLElement} element
	 */
	getLine: function (element) {
		let str = "";
		//
		Array.from(element.children()).reduce(() => {
			// str +=
		});
	}
};

/**  */
$(`.${window.editor.classes.lineClass}`).click((e) => {
	let target = e.target;
	// Get coordinates of mouse click and element relative to sreen
	let mouseX = e.clientX,
		elementX = window.scrollX + target.getBoundingClientRect().left;

	let charNum = (mouseX - elementX) / 10;

	getLine(target);
});
