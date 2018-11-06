window.onload = () => {
    let preferences = JSON.parse(localStorage.getItem("preferences"));
    $("#language-selector").val(preferences.languageOutput);
};

// Buttons event handlers
$("#saveButton").click(() => {
    var preferences = JSON.parse(localStorage.getItem("preferences")) || {};
    preferences.languageOutput = $("#language-selector").val();
    localStorage.setItem("preferences", JSON.stringify(preferences));
    close();
});

$("#cancelButton").click(() => {
    close();
});
