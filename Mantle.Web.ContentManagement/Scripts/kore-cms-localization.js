function T(key) {
    $.ajax({
        url: "/admin/localization/localizable-strings/translate/" + encodeURIComponent(key),
        type: "GET",
        dataType: "json",
        async: false
    })
    .done(function (json) {
        return json.Translation;
    });
}