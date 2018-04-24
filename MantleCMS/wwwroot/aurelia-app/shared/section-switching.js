import $ from 'jquery';

export class SectionSwitcher {
    currentSection = null;

    constructor(id) {
        this.currentSection = $("#" + id);
    }

    swap(id) {
        this.currentSection.hide("fast");
        this.currentSection = $("#" + id);
        this.currentSection.show("fast");
    }
}