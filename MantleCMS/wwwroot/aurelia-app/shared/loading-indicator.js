import * as nprogress from 'nprogress';
import { bindable, noView } from 'aurelia-framework';

@noView(['/jspm_packages/github/rstacruz/nprogress@0.2.0/nprogress.css'])
export class LoadingIndicator {
    @bindable loading = false;

    loadingChanged(newValue) {
        if (newValue) {
            nprogress.start();
        }
        else {
            nprogress.done();
        }
    }
}