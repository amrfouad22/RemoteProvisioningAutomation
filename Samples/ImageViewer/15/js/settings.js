
$(document).ready(function () {
    SP.SOD.executeFunc('sp.js', 'SP.ClientContext', readApplicationSettings);
});

function readApplicationSettings() {
    fixUrls();
    var context = new SP.ClientContext.get_current();
    var web = context.get_web();
    this.props = web.get_allProperties();
    context.load(this.props);
    context.executeQueryAsync(Function.createDelegate(this, gotProperty), Function.createDelegate(this, failedGettingProperty));
}

function gotProperty() {
    var count = this.props.get_item("ITEM_COUNT");
    if (count == '')
        count = '6';
    $("#txtCount").val(count);

}

function failedGettingProperty() {
    alert("Can't get the web properties");
}

function saveSettings() {
    var context = new SP.ClientContext.get_current();
    var web = context.get_web();
    this.props = web.get_allProperties();
    this.props.set_item("ITEM_COUNT", $("#txtCount").val());
    web.update();
    context.executeQueryAsync(Function.createDelegate(this, onSettingsSaveSuccess), Function.createDelegate(this, onSettingsSaveFailure));

}

function onSettingsSaveSuccess() {
    alert("Application Setting Saved");
}

function onSettingsSaveFailure() {
    alert("Error Saving the settings..!");
}
function fixUrls() {
    $("#img").attr("src", _spPageContextInfo.siteAbsoluteUrl + "/PhotoGalleryApp/Images/example.png");
    $("#lstImages").attr("href", _spPageContextInfo.siteAbsoluteUrl + "/PhotoGalleryApp/Lists/PhotoGallery");
}
