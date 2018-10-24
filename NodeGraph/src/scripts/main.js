/* globals require, process, utility */
const { app, Menu, BrowserWindow, ipcMain } = require("electron");
let win;

const template = [
	{
		label: "File",
		submenu: [
			{
				label: "New", accelerator: "CommandOrControl+N",
				click: function () {
					win.webContents.send("new");
				}
			},
			{
				label: "Open...", accelerator: "CommandOrControl+O",
				click: function () {
					win.webContents.send("open");
				}
			},
			{
				label: "Save", accelerator: "CommandOrControl+S",
				click: function () {
					win.webContents.send("save");
				}
			},
			{
				label: "Save As...",
				click: function saveAs() {
					win.webContents.send("saveAs");
				}
			},
			{ type: "separator" },
			{
				label: "Preferences",
				enabled: false,
				click: function () {

				},

			},
			{ type: "separator" },
			{ role: "quit" }
		]
	},
	{
		label: "Edit",
		submenu: [
			{ role: "undo" },
			{ role: "redo" },
			{ type: "separator" },
			{ role: "cut" },
			{ role: "copy" },
			{ role: "paste" }
		]
	},
	{
		label: "View",
		submenu: [
			{ role: "toggledevtools" },
			{
				label: "Zoom in",
				click: function () {
					win.webContents.send("zoom", "increase");
				}
			},
			{
				label: "Zoom out",
				click: function () {
					win.webContents.send("zoom", "decrease");
				}
			}
		]
	},
	{
		role: "help",
		submenu: [
			{
				label: "Report a bug",
				click() {
					require("electron").shell.openExternal(
						"https://github.com/juanferrer/behavior-trees/issues"
					);
				}
			}
		]
	}
];

const menu = Menu.buildFromTemplate(template);
Menu.setApplicationMenu(menu);

function createWindow() {
	win = new BrowserWindow({ width: 800, height: 600 });

	win.loadFile("index.html");

	win.on("closed", () => {
		win = null;
	});
}

app.on("ready", createWindow);

app.on("window-all-closed", () => {
	if (process.platform !== "darwin") {
		app.quit();
	}
});

app.on("activate", () => {
	if (win === null) {
		createWindow();
	}
});

// Respond to async event
ipcMain.on("async", (event, arg) => {
	utility.log(arg);
	event.sender.send("async-reply", 2);
});
