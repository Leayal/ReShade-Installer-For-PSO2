function GenerateImages() {
    var result = $("<div>").addClass("twentytwenty-container");
    result.append($("<img>").attr("src", "original.png"));
    result.append($("<img>").attr("src", "postprocessed.png"));
    return result;
}

function ScaleDown() {
    window.fullsize = false;
    $("#scale").text("Click to view FullSize");
    var target = $("#panel");
    target.empty();
    target.css("width", "50%");
    target.append(GenerateImages());
    InitTwentyTwenty();
}

function ScaleFullsize() {
    window.fullsize = true;
    $("#scale").text("Click to view scaled");
    var target = $("#panel");
    target.empty();
    target.css("width", "auto");
    target.append(GenerateImages());
    InitTwentyTwenty();
}

function InitTwentyTwenty() {
    $(".twentytwenty-container").twentytwenty({
        default_offset_pct: 0.24,
        orientation: 'vertical',
        before_label: 'Original',
        after_label: 'ReShade Effects'
    });
}

$(function() {
    ScaleDown();
    $("#scale").click(function(e) {
        e.preventDefault();
        if (!window.fullsize) {
            ScaleFullsize();
        } else {
            ScaleDown();
        }
    });
});