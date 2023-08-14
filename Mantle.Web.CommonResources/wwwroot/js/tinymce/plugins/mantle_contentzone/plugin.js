tinymce.PluginManager.add('mantle_contentzone', function (editor, url) {
    // Add a button that opens a window
    editor.ui.registry.addButton('contentzone', {
        title: 'Content Zone',
        image: '/_content/Mantle.Web.CommonResources/js/tinymce/plugins/mantle_contentzone/img/mantle_contentzone.png',
        onAction: function () {
            // Open window
            editor.windowManager.open({
                title: 'Mantle Content Zone Plugin',
                body: [
                    { type: 'textbox', name: 'zone', label: 'Zone Name' }
                ],
                onsubmit: function (e) {
                    // Insert content when the window form is submitted
                    editor.insertContent('[[ContentZone:' + e.data.zone + ']]');
                }
            });
        }
    });
});