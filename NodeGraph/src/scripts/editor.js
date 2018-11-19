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
            while (this.element.firstChild) {
                this.element.removeChild(this.element.firstChild);
            }

            // Now store the width of the character
            // TODO: Performant enough to be done on click?
            let testElement = document.createElement("SPAN");
            const testStr = "Test";
            testElement.innerHTML = testStr;
            this.element.appendChild(testElement);
            this.charWidth = $(testElement).width() / testStr.length;
            this.element.removeChild(testElement);

            // Create a single div element from the editor-line class
            let newLine = this.getNewLine();
            this.element.appendChild(newLine);
        } else {
            debug.error(
                `Unexpected tag for editor element. Expected "DIV", found ${
                    this.element.nodeName
                }`
            );
            return;
        }

        return this;
    }

    /**
     * Move selection according to pressed key
     * @param {string} direction
     */
    moveCursor(direction) {
        let line = this.getLineFromNumber(this.cursor.linePos);

        switch (direction.toLowerCase()) {
            case "left":
                this.cursor.colPos--;
                break;
            case "right":
                this.cursor.colPos++;
                break;
            case "up":
                if (this.cursor.linePos > 0) this.cursor.linePos--;
                break;
            case "down":
                this.cursor.linePos++;
                line = this.getLineFromNumber(this.cursor.linePos);
                if (!line) this.cursor.linePos--;
                break;
        }

        // Fix colPos if out of boundaries
        let lineText = this.getLine(line);
        if (this.cursor.colPos > lineText.length) {
            // After end of line
            // Is there a line after this one?
            line = this.getLineFromNumber(++this.cursor.linePos);
            if (line) {
                this.cursor.colPos = 0;
            } else {
                this.cursor.linePos--;
            }

            this.cursor.colPos = lineText.length;
        } else if (this.cursor.colPos < 0 && this.cursor.linePos > 0) {
            // Before start of line and not in first line
            // Go up one line
            line = this.getLineFromNumber(--this.cursor.linePos);
            lineText = this.getLine(line);
            // Go to last character of line
            this.cursor.colPos = lineText.length;
        }
    }

    /**
     * Insert a character in the desired position in the passed element
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
        this.moveCursor("right");
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
        while ((element = element.previousSibling) != null) ++i;
        return i;
    }

    /**
     * Get the element that corresponds to a given line number
     * @param {Number} number
     * @returns {HTMLElement}
     */
    getLineFromNumber(number) {
        let element = $("." + Editor.data.lineClass)[0],
            i = 0;
        while (element && i < number) {
            ++i;
            element = element.nextSibling;
        }

        return element || element.previousSibling;
    }

    /**
     * Create a new editor line
     * @param {HTMLElement}
     */
    getNewLine() {
        let newLine = document.createElement("DIV");
        newLine.classList.add(Editor.data.lineClass);
        newLine.addEventListener("click", e => {
            Editor.eventHandlers.clickHandler(e, this);
        });
        return newLine;
    }

    /**
     * Create a new invisible input in element
     * @param {HTMLElement} element
     */
    addInputToElement(element) {
        let lineInput = document.createElement("INPUT");
        lineInput.setAttribute("type", "text");
        lineInput.setAttribute("id", "editor-cursor");
        lineInput.classList.add(Editor.data.inputClass);
        lineInput.addEventListener("keydown", e => {
            Editor.eventHandlers.inputHandler(e, this);
        });
        element.appendChild(lineInput);
        lineInput.focus();
    }

    /** Remove every editor input element in the editor */
    removeInputElements() {
        $("." + Editor.data.inputClass).remove();
    }

    /**
     * Recalculate the element and trigger a line redraw
     * @param {HTMLElement} element
     */
    redraw(element) {
        let text = element.getAttribute(Editor.data.dataTextAttribute);
        let regex = new RegExp(nodeTypes.join("") + " \\w+", "g");
        let html = text.replace(regex, Editor.htmlClasses.nodeType);
        element.innerHTML = html;
    }
}

// #region Static properties
Editor.data = {
    lineClass: "editor-line",
    inputClass: "editor-input",

    dataTextAttribute: "data-plain-text"
};

Editor.htmlClasses = {
    nodeType: "<div class='editor-node'>$1</div>$2"
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
        // Get coordinates of mouse click and element relative to screen
        let mouseX = event.clientX,
            elementX = window.scrollX + target.getBoundingClientRect().left;

        // Calculate which character in the line is closest to the click point
        let charNum = (mouseX - elementX) / editor.charWidth;
        let line = editor.getLine(target);
        if (charNum > line.length) charNum = Math.max(line.length - 1, 0);

        editor.cursor.linePos = editor.getLineNumber(target);
        editor.cursor.colPos = charNum;

        debug.log(charNum);

        // TODO: Create a blinking cursor

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

        // Go through the possible types of key press
        switch (key) {
            case "Enter":
                // Add a new line after the current node
                editor.removeInputElements();
                element.parentNode.insertBefore(
                    editor.getNewLine(),
                    element.nextSibling
                );
                editor.moveCursor("down");
                break;

            case "Backspace":
                break;

            case "Delete":
                break;

            case "Shift":
                break;

            case "ArrowLeft":
            case "ArrowRight":
            case "ArrowUp":
            case "ArrowDown":
                editor.moveCursor(key.replace("Arrow", "").toLowerCase());
                break;

            default:
                // Any other key pressed, inser the data into the cursor position
                editor.insertCharacterInPosition(
                    key,
                    editor.cursor.colPos,
                    element
                );
                editor.redraw(element);
                editor.removeInputElements();
                editor.addInputToElement(element);
                break;
        }

        console.log(event);
    }
};
// #endregion
