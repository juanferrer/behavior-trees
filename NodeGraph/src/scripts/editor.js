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
	 * @param {string} editorSelector Selector for element to substitute
	 */
	constructor(editorSelector, numberLinesSelector) {
		this.cursor = {
			linePos: 0,
			colPos: 0
		};

		this.element = $(editorSelector)[0];
		this.lineSidebar = $(numberLinesSelector)[0];
		if (this.element.nodeName === "DIV") {
			// Remove all children from element
			while (this.element.firstChild) {
				this.element.removeChild(this.element.firstChild);
			}

			// Now store the width of the character
			// TODO: Performant enough to be done on click?
			let testElement = document.createElement("SPAN");
			const testStr = "Test";
			testElement.innerHTML = testStr;
			testElement.classList.add(Editor.data.lineClass);
			this.element.appendChild(testElement);
			this.charWidth = $(testElement).width() / testStr.length;
			this.element.removeChild(testElement);

			// Create a single div element from the editor-line class
			let newLine = this.getNewLine();
			this.element.appendChild(newLine);
			this.addNewNumberLine();
		} else {
			debug.error(
				`Unexpected tag for editor element. Expected "DIV", found ${this.element.nodeName}`
			);
			return;
		}

		return this;
	}

	/**
	 * Move selection according to pressed key
	 * @param {string} direction
	 * @param {Number} amount
	 */
	moveCursor(direction, amount = 1) {
		// Remove cursor
		$("." + Editor.data.cursorClass).remove();

		let line = this.getLineFromNumber(this.cursor.linePos);

		switch (direction.toLowerCase()) {
			case "left":
				this.cursor.colPos -= amount;
				break;
			case "right":
				this.cursor.colPos += amount;
				break;
			case "up":
				if (this.cursor.linePos > 0) this.cursor.linePos -= amount;
				line = this.getLineFromNumber(this.cursor.linePos);
				break;
			case "down":
				this.cursor.linePos += amount;
				line = this.getLineFromNumber(this.cursor.linePos);
				if (!line) this.cursor.linePos -= amount;
				break;
			case "none":
				break;
		}

		// Fix colPos if out of boundaries
		let lineText = this.getLine(line);
		if (this.cursor.colPos > lineText.length) {
			// After end of line
			if (direction === "right") {
				// Is there a line after this one?
				line = this.getLineFromNumber(++this.cursor.linePos);
				if (line) {
					this.cursor.colPos = 0;
				} else {
					this.cursor.linePos--;
					this.cursor.colPos = lineText.length;
					// Get the previous line again
					line = this.getLineFromNumber(this.cursor.linePos);
				}
			} else {
				// Actually, just jump to the last character in the line
				this.cursor.colPos = lineText.length;
			}
		} else if (this.cursor.colPos < 0) {
			// Before start of line
			if (this.cursor.linePos > 0) {
				// Not in first line
				// Go up one line
				line = this.getLineFromNumber(--this.cursor.linePos);
				lineText = this.getLine(line);
				// Go to last character of line
				this.cursor.colPos = lineText.length;
			} else {
				// This is the first line, just stay in position 0
				this.cursor.colPos = 0;
			}
		}

		// Add cursor to line
		this.redraw(line, true);

		this.addInputToElement(line);
	}

	/**
	 * Insert a character in the specified position in the passed element
	 * @param {string} char
	 * @param {Number} position
	 * @param {HTMLElement} element
	 */
	insertCharacterInPosition(char, position, element) {
		let line = this.getLine(element);
		element.setAttribute(
			Editor.data.dataTextAttribute,
			[line.slice(0, position), char, line.slice(position)].join("")
		);
		this.moveCursor("right", char.length);
	}

	/**
	 * Remove the character that is in the specified position in the passed element
	 * @param {Number} position
	 * @param {HTMLElement} element
	 */
	removeCharacterInPosition(position, element) {
		let line = this.getLine(element);
		element.setAttribute(Editor.data.dataTextAttribute, [line.slice(0, position - 1), line.slice(position)].join(""));
	}

	/**
	 * Get the text inside the line
	 * @param {HTMLElement} element
	 * @returns {string}
	 */
	getLine(element) {
		if (!element || !element.classList.contains(Editor.data.lineClass))
			debug.error("Passed element is not an editor line");
		return element.getAttribute(Editor.data.dataTextAttribute) || "";
	}

	/**
	 * Get line number
	 * @param {HTMLElement} element
	 * @returns {Number}
	 */
	getLineNumber(element) {
		let i = 0;
		let elements = $("." + Editor.data.lineClass);
		while (elements[i] != element) {
			++i;
		}

		return i;
	}

	/**
	 * Get the element that corresponds to a given line number
	 * @param {Number} number
	 * @returns {HTMLElement}
	 */
	getLineFromNumber(number) {
		return $("." + Editor.data.lineClass)[number];
	}

	/**
	 * Create a new editor line
	 * @returns {HTMLElement}
	 */
	getNewLine() {
		let newLine = document.createElement("DIV");
		newLine.classList.add(Editor.data.lineClass);
		newLine.addEventListener("click", e => {
			Editor.eventHandlers.clickHandler(e, this);
		});
		return newLine;
	}

	/** Append a new line number to the line sidebar */
	addNewNumberLine() {
		if (this.lineSidebar) {

			let newLineNumber = document.createElement("DIV");
			newLineNumber.classList.add(Editor.data.lineNumberClass);
			newLineNumber.innerHTML = $("." + Editor.data.lineClass).length;
			this.lineSidebar.appendChild(newLineNumber);
		}
	}

	/** Remove the last line number from the line sidebar */
	removeNumberLine() {
		if (this.lineSidebar && this.lineSidebar.children.length > 1) {
			this.lineSidebar.removeChild(this.lineSidebar.lastChild);
		}
	}

	/**
	 * Create a new invisible input in element
	 * @param {HTMLElement} element
	 */
	addInputToElement(element) {
		if (element) {
			let lineInput = document.createElement("INPUT");
			lineInput.setAttribute("type", "text");
			lineInput.classList.add(Editor.data.inputClass);
			lineInput.addEventListener("keydown", e => {
				Editor.eventHandlers.inputHandler(e, this);
			});
			element.appendChild(lineInput);
			lineInput.focus();
		}
	}

	/** Remove every editor input element in the editor */
	removeInputElements() {
		$("." + Editor.data.inputClass).remove();
	}

	/**
	 * Recalculate the element and trigger a line redraw
	 * @param {HTMLElement} element
	 */
	redraw(element, addCursor) {
		if (element) {
			let text = element.getAttribute(Editor.data.dataTextAttribute) || "";
			// Put a marker where the cursor is going to end up
			let cursorText = [text.slice(0, this.cursor.colPos), Editor.data.cursorMarker, text.slice(this.cursor.colPos)].join("");

			let html = text;
			html = html.replace(/\t/g, "    ");
			// html = html.replace(/(\s+)?([!?&|¬n*"^#])(&nbsp;\w+)/g, Editor.htmlClasses.nodeType);
			html = html.replace(/^(\s+)?([!?&|¬\d*"^#])( \w+)/gm, Editor.htmlClasses.nodeType);

			cursorText = cursorText.replace(/\t/g, "    ");
			// Now replace the cursor marker with the correct element
			cursorText = cursorText.replace(Editor.data.cursorMarker, `<span class="${Editor.data.cursorClass}">|</span>`);

			if (addCursor) {
				// Remove all invisible lines
				$("." + Editor.data.invisibleLineClass).remove();
				$("." + Editor.data.invisibleLineContainerClass).remove();
				// Copy the text into an invisible line that will be on top of the other line
				let invisibleLineContainer = document.createElement("DIV");
				invisibleLineContainer.classList.add(Editor.data.invisibleLineContainerClass);
				let invisibleLine = this.getNewLine();
				invisibleLine.classList.remove(Editor.data.lineClass);
				invisibleLine.classList.add(Editor.data.invisibleLineClass);
				invisibleLine.innerHTML = cursorText;
				invisibleLineContainer.appendChild(invisibleLine);
				element.parentNode.insertBefore(invisibleLineContainer, element);
			}
			element.innerHTML = html;
		}
	}

	/**
	 * Get an array with all lines
	 * @returns {string}
	 */
	getText() {
		let lineElements = $("." + Editor.data.lineClass);
		let lines = "";

		Array.from(lineElements).forEach((l) => {
			lines += this.getLine(l) + "\n";
		});
		return lines;
	}

	/**
	 * Create an element for each line in the passed text
	 * @param {string} text
	 */
	setText(text) {
		$("." + Editor.data.lineClass).remove();
		$("." + Editor.data.inputClass).remove();
		$("." + Editor.data.invisibleLineClass).remove();
		$("." + Editor.data.invisibleLineContainerClass).remove();
		$("." + Editor.data.lineNumberClass).remove();
		let newLine;
		let lines = text.split("\n");

		lines.forEach((l) => {
			newLine = this.getNewLine();
			newLine.setAttribute(Editor.data.dataTextAttribute, this.formatTextForLine(l));
			this.element.appendChild(newLine);
			this.addNewNumberLine();
			this.removeNumberLine();
			this.redraw(newLine, false);
		});

	}

	/**
	 * Prepare the string to be inserted in a line
	 * @param {string} text
	 * @returns {string}
	 */
	formatTextForLine(text) {
		let formattedLine = text.replace(/\t/g, "    ");
		return formattedLine;
	}

	/** Empty every line in editor */
	reset() {
		this.cursor = {
			linePos: 0,
			colPos: 0
		};
		while (this.element.firstChild) {
			this.element.removeChild(this.element.firstChild);
		}

		while (this.lineSidebar && this.lineSidebar.firstChild) {
			this.lineSidebar.removeChild(this.lineSidebar.firstChild);
		}
		// Create a single div element from the editor-line class
		let newLine = this.getNewLine();
		this.element.appendChild(newLine);
		this.addNewNumberLine();
	}
}

// #region Static properties
Editor.data = {
	lineClass: "editor-line",
	inputClass: "editor-input",
	cursorClass: "editor-cursor",
	invisibleLineClass: "editor-invisible-line",
	invisibleLineContainerClass: "editor-invisible-line-container",
	lineNumberClass: "editor-line-number",

	lineSidebar: "line-sidebar",
	dataTextAttribute: "data-plain-text",
	cursorMarker: "--CURSOR_MARKER--",
};

Editor.htmlClasses = {
	nodeType: "$1<span class='editor-node'>$2</span>$3"
};

// #endregion

// #region Static methods

// #endregion

// #region Event handlers
Editor.eventHandlers = {
	/**
	 * @param {Event} event
	 * @param {Editor} editor
	 */
	clickHandler: (event, editor) => {
		let target = event.target;

		// Use Chrome's behavior to find clicked character
		//var selection = window.getSelection();
		//let charNum = selection.focusOffset;

		// Calculate which character in the line is closest to the click point
		let charNum = Math.round(((event.clientX - target.getBoundingClientRect().left) / editor.charWidth));

		let line = editor.getLine(target);
		if (charNum > line.length) charNum = Math.max(line.length, 0);

		editor.cursor.linePos = editor.getLineNumber(target);
		editor.cursor.colPos = charNum;

		debug.log(charNum);

		// Create a blinking cursor
		editor.redraw(target, true);

		// Delete all other inputs
		editor.removeInputElements();

		// Create an invisible input
		editor.addInputToElement(target);
	},

	/**
	 * @param {Event} event
	 * @param {Editor} editor
	 */
	inputHandler: (event, editor) => {
		// Get reference to parent element
		let inputElement = event.target;
		let element = inputElement.parentNode;
		let key = event.key;

		let text, line1, line2, newLine;

		// Go through the possible types of key press
		switch (key) {
			case "Enter":
				// Add a new line after the current node
				editor.removeInputElements();
				text = editor.getLine(element);
				line1 = text.slice(0, editor.cursor.colPos);
				line2 = text.slice(editor.cursor.colPos);

				newLine = editor.getNewLine();
				element.setAttribute(Editor.data.dataTextAttribute, line1);
				newLine.setAttribute(Editor.data.dataTextAttribute, line2);


				element.parentNode.insertBefore(
					newLine,
					element.nextSibling
				);
				editor.addNewNumberLine();

				editor.redraw(element, true);
				editor.cursor.colPos = 0;

				//editor.addInputToElement(element.nextSibling);
				editor.moveCursor("down");
				break;

			case "Backspace":
				// Check if we're at the start of a line and not on the first line
				if (editor.cursor.colPos === 0 && editor.cursor.linePos > 0) {
					// Ok, so we need to get combine the current line with the previous line
					editor.moveCursor("left");
					element = editor.getLineFromNumber(editor.cursor.linePos);
					line1 = element.getAttribute(Editor.data.dataTextAttribute);
					line1 += editor.getLineFromNumber(editor.cursor.linePos + 1).getAttribute(Editor.data.dataTextAttribute);

					$(editor.getLineFromNumber(editor.cursor.linePos + 1)).remove();

					element.setAttribute(Editor.data.dataTextAttribute, line1);
					editor.redraw(element);
					editor.removeInputElements();
					editor.moveCursor("none");
				} else {
					editor.removeCharacterInPosition(editor.cursor.colPos, element);
					editor.removeInputElements();
					editor.moveCursor("left");
				}
				break;

			case "Delete":
				// Check if we're at the end of a line and not on the last line
				if (editor.cursor.colPos === editor.getLine(element).length && editor.cursor.linePos < $("." + Editor.data.lineClass).length - 1) {
					element = editor.getLineFromNumber(editor.cursor.linePos);
					line1 = element.getAttribute(Editor.data.dataTextAttribute);
					line1 += editor.getLineFromNumber(editor.cursor.linePos + 1).getAttribute(Editor.data.dataTextAttribute);

					$(editor.getLineFromNumber(editor.cursor.linePos + 1)).remove();

					element.setAttribute(Editor.data.dataTextAttribute, line1);
					editor.redraw(element);
					editor.removeInputElements();
					editor.moveCursor("none");
				} else {
					editor.removeCharacterInPosition(editor.cursor.colPos + 1, element);
					editor.removeInputElements();
					editor.moveCursor("none");
				}
				break;

			case "Tab":
				editor.insertCharacterInPosition("    ", editor.cursor.colPos, element);
				editor.removeInputElements();
				editor.addInputToElement(element);
				// Make sure you don't deselect the editor
				event.preventDefault();
				break;

			case "Shift":
			case "CapsLock":
			case "Control":
			case "Alt":
			case "Escape":
			case "Windows":
			case "ScrollLock":
			case "NumLock":
			case "Insert":
			case "PageUp":
			case "PageDown":
			case "F1":
			case "F2":
			case "F3":
			case "F4":
			case "F5":
			case "F6":
			case "F7":
			case "F8":
			case "F9":
			case "F10":
			case "F11":
			case "F12":
				// Do nothing
				break;

			case "Home":
				// TODO: Move to start of the line
				break;
			case "End":
				// TODO: Move to end of the line
				break;

			case "ArrowLeft":
			case "ArrowRight":
			case "ArrowUp":
			case "ArrowDown":
				editor.removeInputElements();
				editor.moveCursor(key.replace("Arrow", "").toLowerCase());
				//editor.addInputToElement(element);
				break;

			default:
				// Any other key pressed, inser the data into the cursor position
				editor.insertCharacterInPosition(key, editor.cursor.colPos, element);
				editor.removeInputElements();
				editor.addInputToElement(element);
				break;
		}

		debug.log(event);
	}
};
// #endregion
