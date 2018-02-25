define(function (require) {
    'use strict'

    var $ = require('jquery');
    var elfinder = require('elfinder');
    
    var ViewModel = function () {
        var self = this;
        
        self.attached = function () {
            var myCommands = elfinder.prototype._options.commands;

            var disabled = ['extract', 'archive', 'resize', 'help', 'select']; // Not yet implemented commands in ElFinder.Net

            var idx = null;
            $.each(disabled, function (i, cmd) {
                (idx = $.inArray(cmd, myCommands)) !== -1 && myCommands.splice(idx, 1);
            });

            var selectedFile = null;

            var options = {
                url: '/admin/media/elfinder/connector',
                rememberLastDir: false, // Prevent elFinder saving in the Browser LocalStorage the last visited directory
                commands: myCommands,
                //lang: 'pt_BR', // elFinder supports UI and messages localization. Check the folder Content\elfinder\js\i18n for all available languages. Be sure to include the corresponding .js file(s) in the JavaScript bundle.
                uiOptions: { // UI buttons available to the user
                    toolbar: [
                        ['back', 'forward'],
                        ['reload'],
                        ['home', 'up'],
                        ['mkdir', 'mkfile', 'upload'],
                        ['open', 'download'],
                        ['info'],
                        ['quicklook'],
                        ['copy', 'cut', 'paste'],
                        ['rm'],
                        ['duplicate', 'rename', 'edit'],
                        ['view', 'sort']
                    ]
                },

                //handlers: {
                //    select: function (event, elfinderInstance) {
                //        if (event.data.selected.length == 1) {
                //            var item = $('#' + event.data.selected[0]);
                //            if (!item.hasClass('directory')) {
                //                selectedFile = event.data.selected[0];
                //                $('#elfinder-selectFile').show();
                //                return;
                //            }
                //        }
                //        $('#elfinder-selectFile').hide();
                //        selectedFile = null;
                //    }
                //}
            };
            $('#elfinder').elfinder(options).elfinder('instance');

            //$('.elfinder-toolbar:first').append('<div class="ui-widget-content ui-corner-all elfinder-buttonset" id="elfinder-selectFile" style="display:none; float:right;">' +
            //'<div class="ui-state-default elfinder-button" title="Select" style="width: 100px;"></div>');
            //$('#elfinder-selectFile').click(function ()
            //{
            //    if (selectedFile != null)
            //        $.post('file/select-file', { target: selectedFile }, function (response)
            //        {
            //            alert(response);
            //        });

            //});
        };
    };

    var viewModel = new ViewModel();
    return viewModel;
});