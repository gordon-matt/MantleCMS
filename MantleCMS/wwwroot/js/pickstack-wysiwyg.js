(function ($, ko) {
    var binding = {
        'after': ['attr', 'value'],
        'defaults': {},
        'extensions': {},

        'init': function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            if (!ko.isWriteableObservable(valueAccessor())) {
                throw 'valueAccessor must be writeable and observable';
            }

            var options = allBindings.has('wysiwygConfig') ? allBindings.get('wysiwygConfig') : null,
                ext = allBindings.has('wysiwygExtensions') ? allBindings.get('wysiwygExtensions') : [],
                settings = configure(binding['defaults'], ext, options, arguments),
                disposed = false,
                attempts = 0;

            $(element).text(valueAccessor()());

            var timer = setInterval(function () {
                if (disposed) {
                    clearInterval(timer);
                    return;
                }

                if (!window.Pickstack || !Pickstack.editors || typeof Pickstack.editors.init !== 'function') {
                    if (++attempts > 200) {
                        clearInterval(timer);
                    }
                    return;
                }

                var rootField = document.getElementById('TinyMCE_RootFolder');
                if (Pickstack.editors.init(element, {
                    fileRoot: rootField && rootField.value ? rootField.value : '',
                    init: settings
                }) === false) {
                    if (++attempts > 200) {
                        clearInterval(timer);
                    }
                    return;
                }

                clearInterval(timer);
            }, 30);

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                disposed = true;
                clearInterval(timer);
                var editor = element._pickstackEditor;
                if (editor && typeof editor.remove === 'function' && !editor.removed) {
                    editor.remove();
                }
                element._pickstackEditor = null;
            });

            return { controlsDescendantBindings: true };
        },

        'update': function (element, valueAccessor) {
            var editor = element._pickstackEditor,
                value = valueAccessor()() || '';

            if (editor && !editor.removed && typeof editor.getContent === 'function') {
                if (editor.getContent() !== value) {
                    editor.setContent(value);
                }
                return;
            }

            if (element.value !== value) {
                element.value = value;
            }
        }
    };

    var configure = function (defaults, extensions, options, args) {
        var config = $.extend(true, {}, defaults);

        if (options) {
            ko.utils.objectForEach(options, function (property) {
                if (Object.prototype.toString.call(options[property]) === '[object Array]') {
                    if (!config[property]) {
                        config[property] = [];
                    }
                    options[property] = ko.utils.arrayGetDistinctValues(config[property].concat(options[property]));
                }
            });

            $.extend(true, config, options);
        }

        if (!config['plugins']) {
            config['plugins'] = ['paste'];
        } else if ($.inArray('paste', config['plugins']) === -1) {
            config['plugins'].push('paste');
        }

        var applyChange = function (editor) {
            args[0]._pickstackEditor = editor;
            editor.on('change keyup nodechange', function (e) {
                args[1]()(editor.getContent());

                for (var name in extensions) {
                    if (extensions.hasOwnProperty(name)) {
                        binding['extensions'][extensions[name]](editor, e, args[2], args[4]);
                    }
                }
            });
        };

        if (typeof (config['setup']) === 'function') {
            var setup = config['setup'];

            config['setup'] = function (editor) {
                setup(editor);
                applyChange(editor);
            };
        } else {
            config['setup'] = applyChange;
        }

        return config;
    };

    ko.bindingHandlers['wysiwyg'] = binding;
})(jQuery, ko);