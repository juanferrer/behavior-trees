
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

class Editor {
	/**
	 * Construct editor and return the newly created object
	 * @param {string} elementSelector Selector for element to substitute
	 */
	constructor(elementSelector) {
		this.charWidth = 0;
		this.cursor = {
			linePos: 0,
			colPos: 0
		};

		this.element = $(elementSelector)[0];
		if (this.element.nodeName === "DIV") {
			// Remove all children from element
			while (this.element.firstChild) { this.element.removeChild(this.element.firstChild); }

			// Now store the width of the character
			// TODO: Performant enough to be done on click?
			let testElement = document.createElement("SPAN");
			const testStr = "Test";
			testElement.innerHTML = testStr;
			this.element.appendChild(testElement);
			this.charWidth = $(testElement).width() / testStr.length;
			this.element.removeChild(testElement);

			// Create a single div element from the editor-line class
			let newLine = document.createElement("DIV");
			newLine.classList.add(Editor.classes.line);
			newLine.addEventListener("click", Editor.eventHandlers.clickHandler);
			this.element.appendChild(newLine);
		} else {
			debug.error(`Unexpected tag for editor element. Expected "DIV", found ${this.element.nodeName}`);
			return;
		}

		return this;
	}
}

// #region Static properties
Editor.classes = {
	line: "editor-line",
	input: "editor-input",
};

// #endregion

// #region Static methods

/**
 * Get the text inside the line
 * @param {HTMLElement} e
 */
Editor.getLine = (e) => {
	return Array.from(e.childNodes).reduce((acc, val) => {
		acc += val;
	}, "");
};

// #endregion

// #region Event handlers
Editor.eventHandlers = {
	clickHandler: (e) => {
		let target = e.target;
		// Get coordinates of mouse click and element relative to screen
		let mouseX = e.clientX,
			elementX = window.scrollX + target.getBoundingClientRect().left;

		// Calculate which character in the line is closest to the click point
		let charNum = (mouseX - elementX) / window.editor.charWidth;
		let line = Editor.getLine(target);
		if (charNum > line.length) charNum = line.length - 1;

		debug.log(charNum);

		// TODO: Create a blinking cursor

		// TODO: Delete all other inputs
		$("." + Editor.classes.input).remove();

		// TODO: Create an invisible input
		let lineInput = document.createElement("INPUT");
		lineInput.classList.add(Editor.classes.input);
		lineInput.addEventListener("change", Editor.eventHandlers.inputHandler);
		target.appendChild(lineInput);
	},

	inputHandler: (e) => {
		console.log(e);
	},
};
// #endregion
