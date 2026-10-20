tinymce.PluginManager.add('mantle_contentzone', function (editor) {
    editor.ui.registry.addIcon('contentzone',
        '<svg width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M3 4h18v16H3V4zm2 2v12h14V6H5zm2 2h6v4H7V8z"/></svg>');

    editor.ui.registry.addButton('contentzone', {
        icon: 'contentzone',
        tooltip: 'Content Zone',
        onAction: function () {
            editor.windowManager.open({
                title: 'Mantle Content Zone Plugin',
                body: {
                    type: 'panel',
                    items: [
                        { type: 'input', name: 'zone', label: 'Zone Name' }
                    ]
                },
                buttons: [
                    { type: 'cancel', text: 'Cancel' },
                    { type: 'submit', text: 'Insert', primary: true }
                ],
                onSubmit: function (api) {
                    var data = api.getData();
                    editor.insertContent('[[ContentZone:' + (data.zone || '') + ']]');
                    api.close();
                }
            });
        }
    });
});
