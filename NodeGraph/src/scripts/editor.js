
/* globals $, debug, nodeTypes */
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
			let newLine = Editor.getNewLine();
			this.element.appendChild(newLine);
		} else {
			debug.error(`Unexpected tag for editor element. Expected "DIV", found ${this.element.nodeName}`);
			return;
		}

		return this;
	}
}

// #region Static properties
Editor.data = {
	lineClass: "editor-line",
	inputClass: "editor-input",

	dataTextAttribute: "data-plain-text",
};

Editor.htmlClasses = {
	nodeType: "<html>$1</html>$2",
};

// #endregion

// #region Static methods

/**
 * Get the text inside the line
 * @param {HTMLElement} element
 * @returns {string}
 */
Editor.getLine = (element) => {
	if (!element.classList.contains(Editor.data.lineClass)) debug.error("Passed element is not an editor line");
	return element.getAttribute(Editor.data.dataTextAttribute) || "";
};

/**
 * Get line number
 * @param {HTMLElement} element
 * @returns {Number}
 */
Editor.getLineNumber = (element) => {
	let i = 0;
	while ((element = element.previousSibling) != null)++i;
	return i;
};

/**
 * Create a new editor line
 * @param {HTMLElement}
 */
Editor.getNewLine = () => {
	let newLine = document.createElement("DIV");
	newLine.classList.add(Editor.data.lineClass);
	newLine.addEventListener("click", Editor.eventHandlers.clickHandler);
	return newLine;
};

/**
 * Create a new invisible input in element
 * @param {HTMLElement} element
 */
Editor.addInputToElement = (element) => {
	let lineInput = document.createElement("INPUT");
	lineInput.setAttribute("type", "text");
	lineInput.setAttribute("id", "editor-cursor");
	lineInput.classList.add(Editor.data.inputClass);
	lineInput.addEventListener("keydown", Editor.eventHandlers.inputHandler);
	element.appendChild(lineInput);
	lineInput.focus();
};

/** Remove every editor input element in the editor */
Editor.removeInputElements = () => {
	$("." + Editor.data.inputClass).remove();
};

/**
 * Insert a character in the desired position in the passed element
 * @param {string} char
 * @param {Number} position
 * @param {HTMLElement} element
 */
Editor.insertCharacterInPosition = (char, position, element) => {
	let line = Editor.getLine(element);
	element.setAttribute(Editor.data.dataTextAttribute, [line.slice(0, position), char, line.slice(position)].join(""));
	++window.editor.cursor.colPos;
};

/**
 * Recalculate the element and trigger a redraw
 * @param {HTMLElement} element
 */
Editor.redraw = (element) => {
	let text = element.getAttribute(Editor.data.dataTextAttribute);
	let regex = new RegExp(nodeTypes.join("") + " \\w+", "g");
	let html = text.replace(regex, Editor.htmlClasses.nodeType);
	element.innerHTML = html;
};

// #endregion

// #region Event handlers
Editor.eventHandlers = {
	clickHandler: (event) => {
		let target = event.target;
		// Get coordinates of mouse click and element relative to screen
		let mouseX = event.clientX,
			elementX = window.scrollX + target.getBoundingClientRect().left;

		// Calculate which character in the line is closest to the click point
		let charNum = (mouseX - elementX) / window.editor.charWidth;
		let line = Editor.getLine(target);
		if (charNum > line.length) charNum = Math.max(line.length - 1, 0);

		window.editor.cursor.linePos = Editor.getLineNumber(target);
		window.editor.cursor.colPos = charNum;

		debug.log(charNum);

		// TODO: Create a blinking cursor

		// Delete all other inputs
		Editor.removeInputElements();

		// Create an invisible input
		Editor.addInputToElement(target);
	},

	inputHandler: (event) => {
		// Get reference to parent element
		let inputElement = $("." + Editor.data.inputClass).parent()[0];
		let element = inputElement.parentNode;
		let key = event.key;

		// Go through the possible types of key press
		switch (key) {

			case "Enter":
				// Add a new line after the current node
				Editor.removeInputElements();
				element.parentNode.insertBefore(Editor.getNewLine(), element.nextSibling);
				break;

			default:
				// Any other key pressed, inser the data into the cursor position
				Editor.insertCharacterInPosition(key, window.editor.cursor.colPos, element);
				Editor.redraw(element);
				Editor.removeInputElements();
				Editor.addInputToElement(element);
				break;
		}

		console.log(event);
	},
};
// #endregion
