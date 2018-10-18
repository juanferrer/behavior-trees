var cy = cytoscape({container: $("#cy")});

$(window).resize(() => {
	$("#text-editor")[0].style.width = "calc(100% - 8px)";
	$("#text-editor")[0].style.height = "calc(100% - 5px)"
});

$("#text-editor").change(e => {
	console.log("Text changed");
});