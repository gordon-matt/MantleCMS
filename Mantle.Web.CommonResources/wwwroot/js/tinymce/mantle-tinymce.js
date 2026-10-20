function mantlePickstackFilePicker(callback, value, meta) {
    if (!window.Pickstack || !window.Pickstack.files) {
        return;
    }

    var rootField = document.getElementById('TinyMCE_RootFolder');
    window.Pickstack.files.open({
        type: meta && meta.filetype ? meta.filetype : 'file',
        root: rootField && rootField.value ? rootField.value : '',
        callback: function (file) {
            callback(file.url, { alt: file.name || '', title: file.name || '' });
        }
    });
}

const mantleTinyMceFilePicker = {
    file_picker_types: 'file image media',
    file_picker_callback: mantlePickstackFilePicker,
    relative_urls: false,
    remove_script_host: true,
    convert_urls: true
};

const mantleDefaultTinyMCEConfig = Object.assign({
    theme: "silver",
    plugins: [
        "advlist autolink lists link image charmap print preview hr anchor pagebreak",
        "searchreplace wordcount visualblocks visualchars code fullscreen",
        "insertdatetime media nonbreaking save table directionality",
        "template paste"
    ],
    toolbar1: "insertfile undo redo | styleselect | ltr rtl | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
    toolbar2: "print preview media | forecolor backcolor",
    image_advtab: true,
    image_dimensions: false,
    height: 400,
    allow_script_urls: true,
    allow_events: true,
    valid_elements: '+*[*]',
    extended_valid_elements: '+*[*]',
    valid_children: "+body[style]"
}, mantleTinyMceFilePicker);

const mantleAdvancedTinyMCEConfig = Object.assign({
    theme: "silver",
    plugins: [
        "advlist autolink lists link image charmap print preview hr anchor pagebreak",
        "searchreplace wordcount visualblocks visualchars code fullscreen",
        "insertdatetime media nonbreaking save table directionality",
        "template paste",
        "mantle_contentzone"
    ],
    toolbar1: "insertfile undo redo | styleselect | ltr rtl | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
    toolbar2: "print preview media | forecolor backcolor",
    toolbar3: "contentzone",
    image_advtab: true,
    image_dimensions: false,
    height: 400,
    allow_script_urls: true,
    allow_events: true,
    valid_elements: '+*[*]',
    extended_valid_elements: '+*[*]',
    valid_children: "+body[style]"
}, mantleTinyMceFilePicker);
