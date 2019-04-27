import 'tinymce/plugins/advlist/plugin';
import 'tinymce/plugins/anchor/plugin';
import 'tinymce/plugins/autolink/plugin';
import 'tinymce/plugins/charmap/plugin';
import 'tinymce/plugins/code/plugin';
import 'tinymce/plugins/contextmenu/plugin';
import 'tinymce/plugins/directionality/plugin';
//import 'tinymce/plugins/emoticons/plugin';
import 'tinymce/plugins/fullscreen/plugin';
import 'tinymce/plugins/hr/plugin';
import 'tinymce/plugins/image/plugin';
import 'tinymce/plugins/insertdatetime/plugin';
import 'tinymce/plugins/link/plugin';
import 'tinymce/plugins/lists/plugin';
import 'tinymce/plugins/media/plugin';
import 'tinymce/plugins/nonbreaking/plugin';
import 'tinymce/plugins/pagebreak/plugin';
import 'tinymce/plugins/paste/plugin';
import 'tinymce/plugins/preview/plugin';
import 'tinymce/plugins/print/plugin';
import 'tinymce/plugins/save/plugin';
import 'tinymce/plugins/searchreplace/plugin';
import 'tinymce/plugins/table/plugin';
import 'tinymce/plugins/template/plugin';
import 'tinymce/plugins/textcolor/plugin';
import 'tinymce/plugins/visualblocks/plugin';
import 'tinymce/plugins/visualchars/plugin';
import 'tinymce/plugins/wordcount/plugin';

import '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.tinymce.plugins.mantle_contentzone.plugin';
import '/aurelia-app/embedded/Mantle.Web.CommonResources.Scripts.tinymce.plugins.responsivefilemanager.plugin';

export class MantleTinyMCEOptions {
    constructor() {
        this.defaultConfig = {
            theme: "silver",
            plugins: [
                "advlist autolink lists link image charmap print preview hr anchor pagebreak",
                "searchreplace wordcount visualblocks visualchars code fullscreen",
                "insertdatetime media nonbreaking save table directionality",
                "template paste",
                "responsivefilemanager"
            ],
            toolbar1: "insertfile undo redo | styleselect | ltr rtl | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
            toolbar2: "print preview media | forecolor backcolor",
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
            external_plugins: { "filemanager": "/filemanager/plugin.min.js" },
            valid_elements: '+*[*]',
            extended_valid_elements: '+*[*]',
            valid_children: "+body[style]"
        };

        this.advancedConfig = {
            theme: "silver",
            plugins: [
                "advlist autolink lists link image charmap print preview hr anchor pagebreak",
                "searchreplace wordcount visualblocks visualchars code fullscreen",
                "insertdatetime media nonbreaking save table directionality",
                "template paste",
                "mantle_contentzone responsivefilemanager"
            ],
            toolbar1: "insertfile undo redo | styleselect | ltr rtl | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
            toolbar2: "print preview media | forecolor backcolor",
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
            external_plugins: { "filemanager": "/filemanager/plugin.min.js" },
            valid_elements: '+*[*]',
            extended_valid_elements: '+*[*]',
            valid_children: "+body[style]"
        };
    }

    setContentCss(tinyMCEContentCss) {
        this.defaultConfig.content_css = tinyMCEContentCss;
        this.advancedConfig.content_css = tinyMCEContentCss;
    }
}