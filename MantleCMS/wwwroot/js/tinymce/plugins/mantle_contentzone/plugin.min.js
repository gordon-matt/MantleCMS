tinymce.PluginManager.add('kore_contentzone', function (editor, url) {
    // Add a button that opens a window
    editor.addButton('contentzone', {
        title: 'Content Zone',
        image: url + '/img/kore_contentzone.png',
        onclick: function () {
            // Open window
            editor.windowManager.open({
                title: 'Kore Content Zone Plugin',
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