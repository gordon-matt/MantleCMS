tinymce.PluginManager.add('mantle_contentzone', function (editor, url) {
    // Add a button that opens a window
    editor.addButton('contentzone', {
        title: 'Content Zone',
        image: '/Mantle.Web.CommonResources.Scripts.tinymce.plugins.mantle_contentzone.img.mantle_contentzone.png',
        onclick: function () {
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