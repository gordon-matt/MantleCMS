var mantleDefaultTinyMCEConfig = {
    theme: "modern",
    plugins: [
        "advlist autolink lists link image charmap print preview hr anchor pagebreak",
        "searchreplace wordcount visualblocks visualchars code fullscreen",
        "insertdatetime media nonbreaking save table contextmenu directionality",
        "emoticons template paste textcolor",
        "responsivefilemanager"
    ],
    toolbar1: "insertfile undo redo | styleselect | ltr rtl | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
    toolbar2: "print preview media | forecolor backcolor emoticons",
    toolbar3: "responsivefilemanager",
    image_advtab: true,
    image_dimensions: false,
    //templates: [
    //    { title: 'Test template 1', content: 'Test 1' },
    //    { title: 'Test template 2', content: 'Test 2' }
    //],
    //force_br_newlines: false,
    //force_p_newlines: false,
    //forced_root_block: '',
    height: 400,
    allow_script_urls: true,
    allow_events: true,
    external_filemanager_path: "/filemanager/",
    external_plugins: { "filemanager": "/filemanager/plugin.js" },
    valid_elements: '+*[*]',
    extended_valid_elements: '+*[*]',
    valid_children: "+body[style]"
};

var mantleAdvancedTinyMCEConfig = {
    theme: "modern",
    plugins: [
        "advlist autolink lists link image charmap print preview hr anchor pagebreak",
        "searchreplace wordcount visualblocks visualchars code fullscreen",
        "insertdatetime media nonbreaking save table contextmenu directionality",
        "emoticons template paste textcolor",
        "mantle_contentzone responsivefilemanager"
    ],
    toolbar1: "insertfile undo redo | styleselect | ltr rtl | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
    toolbar2: "print preview media | forecolor backcolor emoticons",
    toolbar3: "responsivefilemanager | contentzone",
    image_advtab: true,
    image_dimensions: false,
    //templates: [
    //    { title: 'Test template 1', content: 'Test 1' },
    //    { title: 'Test template 2', content: 'Test 2' }
    //],
    //force_br_newlines: false,
    //force_p_newlines: false,
    //forced_root_block: '',
    height: 400,
    allow_script_urls: true,
    allow_events: true,
    external_filemanager_path: "/filemanager/",
    external_plugins: { "filemanager": "/filemanager/plugin.js" },
    valid_elements: '+*[*]',
    extended_valid_elements: '+*[*]',
    valid_children: "+body[style]"
};

$(document).ready(function () {
    if (typeof tinyMCEContentCss !== 'undefined') {
        mantleDefaultTinyMCEConfig.content_css = tinyMCEContentCss;
        mantleAdvancedTinyMCEConfig.content_css = tinyMCEContentCss;
    }
});