/*****************************************/
// Name: Javascript Textarea BBCode Markup Editor
// Version: 1.3
// Author: Balakrishnan
// Last Modified Date: 25/jan/2009
// License: Free
// URL: http://www.corpocrat.com
/******************************************/

// Modified for Mantle
// Last Modified: 2016.05.25

var textarea;
var content;

function edToolbar(elementId, webRoot, allowUploads) {
    document.write("<div class=\"toolbar\">");
    document.write("<img class=\"btn btn-light markup-button\" src=\"" + webRoot + "BBEditor/images/bold.gif\" title=\"Bold\" name=\"btnBold\" onClick=\"doAddTags('[b]','[/b]','" + elementId + "')\">");
    document.write("<img class=\"btn btn-light markup-button\" src=\"" + webRoot + "BBEditor/images/italic.gif\" title=\"Italic\" name=\"btnItalic\" onClick=\"doAddTags('[i]','[/i]','" + elementId + "')\">");
    document.write("<img class=\"btn btn-light markup-button\" src=\"" + webRoot + "BBEditor/images/underline.gif\" title=\"Underline\" name=\"btnUnderline\" onClick=\"doAddTags('[u]','[/u]','" + elementId + "')\">");
    document.write("<img class=\"btn btn-light markup-button\" src=\"" + webRoot + "BBEditor/images/link.gif\" title=\"Link\" name=\"btnLink\" onClick=\"doURL('" + elementId + "')\">");
    document.write("<img class=\"btn btn-light markup-button\" src=\"" + webRoot + "BBEditor/images/picture.gif\" title=\"Picture\" name=\"btnLink\" onClick=\"doImg('" + elementId + "')\">");
    document.write("<img class=\"btn btn-light markup-button\" src=\"" + webRoot + "BBEditor/images/code.gif\" title=\"Code\" name=\"btnCode\" onClick=\"doAddTags('[code]','[/code]','" + elementId + "')\">");

    if (allowUploads) {
        document.write("<img class=\"btn btn-light markup-button\" src=\"" + webRoot + "BBEditor/images/upload-img.gif\" title=\"Upload Image\" name=\"btnUploadImg\" onClick=\"uploadFile('" + elementId + "','img')\">");
        document.write("<img class=\"btn btn-light markup-button\" src=\"" + webRoot + "BBEditor/images/upload-file.gif\" title=\"Upload File\" name=\"btnUploadFile\" onClick=\"uploadFile('" + elementId + "','file')\">");
    }

    document.write("</div>");
}

function doURL(elementId, url) {
    if (!url) {
        url = prompt('Enter the URL:', 'http://');
    }

    textarea = document.getElementById(elementId);
    var scrollTop = textarea.scrollTop;
    var scrollLeft = textarea.scrollLeft;
    if (url != '' && url != null) {
        if (document.selection) {
            textarea.focus();
            var sel = document.selection.createRange();
            if (sel.text == "") {
                sel.text = '[url]' + url + '[/url]';
            }
            else {
                sel.text = '[url=' + url + ']' + sel.text + '[/url]';
            }
        }
        else {
            var len = textarea.value.length;
            var start = textarea.selectionStart;
            var end = textarea.selectionEnd;

            var sel = textarea.value.substring(start, end);

            if (sel == "") {
                var rep = '[url]' + url + '[/url]';
            }
            else {
                var rep = '[url=' + url + ']' + sel + '[/url]';
            }

            textarea.value = textarea.value.substring(0, start) + rep + textarea.value.substring(end, len);
            textarea.scrollTop = scrollTop;
            textarea.scrollLeft = scrollLeft;
        }
    }
}

function doImg(elementId, url) {
    if (!url) {
        url = prompt('Enter the picture URL:', 'http://');
    }

    textarea = document.getElementById(elementId);
    var scrollTop = textarea.scrollTop;
    var scrollLeft = textarea.scrollLeft;
    if (url != '' && url != null) {
        if (document.selection) {
            textarea.focus();
            var sel = document.selection.createRange();
            sel.text = '[img]' + url + '[/img]';
        }
        else {
            var len = textarea.value.length;
            var start = textarea.selectionStart;
            var end = textarea.selectionEnd;

            var rep = '[img]' + url + '[/img]';

            textarea.value = textarea.value.substring(0, start) + rep + textarea.value.substring(end, len);
            textarea.scrollTop = scrollTop;
            textarea.scrollLeft = scrollLeft;
        }
    }
}

function doAddTags(tag1, tag2, elementId) {
    textarea = document.getElementById(elementId);
    // Code for IE
    if (document.selection) {
        textarea.focus();
        var sel = document.selection.createRange();
        sel.text = tag1 + sel.text + tag2;
    }
    else {  // Code for Mozilla Firefox
        var len = textarea.value.length;
        var start = textarea.selectionStart;
        var end = textarea.selectionEnd;
        var scrollTop = textarea.scrollTop;
        var scrollLeft = textarea.scrollLeft;
        var sel = textarea.value.substring(start, end);
        var rep = tag1 + sel + tag2;
        textarea.value = textarea.value.substring(0, start) + rep + textarea.value.substring(end, len);
        textarea.scrollTop = scrollTop;
        textarea.scrollLeft = scrollLeft;
    }
}



var ed_UploadFileType = '';
var ed_TextAreaElementId = '';

function uploadFile(elementId, fileType) {
    $("#UploadModal").modal("show");
    ed_TextAreaElementId = elementId;
    ed_UploadFileType = fileType;
}

function uploadFileDone(url, fileName) {
    $("#UploadModal").modal("hide");
    switch (ed_UploadFileType) {
        case 'img': doImg(ed_TextAreaElementId, url); break;
            //case 'file': doURL(ed_TextAreaElementId, url); break;
        case 'file':

            textarea = document.getElementById(ed_TextAreaElementId);
            var scrollTop = textarea.scrollTop;
            var scrollLeft = textarea.scrollLeft;
            if (url != '' && url != null) {
                var len = textarea.value.length;
                var start = textarea.selectionStart;
                var end = textarea.selectionEnd;

                var sel = textarea.value.substring(start, end);

                var rep = '[url=' + url + ']' + fileName + '[/url]';

                textarea.value = textarea.value.substring(0, start) + rep + textarea.value.substring(end, len);
                textarea.scrollTop = scrollTop;
                textarea.scrollLeft = scrollLeft;
            }

            break;
        default: break;
    }
}