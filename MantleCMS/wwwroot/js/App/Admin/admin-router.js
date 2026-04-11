class AdminRouter {
    #routes = [];
    #pageHost = null;
    #currentViewModel = null;
    #isNavigating = false;

    async init() {
        const response = await fetch('/admin/get-spa-routes');
        this.#routes = await response.json();

        this.#pageHost = document.getElementById('page-host');

        window.addEventListener('hashchange', () => this.#navigate());

        await this.#navigate();
    }

    #findRoute(hash) {
        for (const route of this.#routes) {
            if (route.route === hash) return { route, params: [] };
        }

        for (const route of this.#routes) {
            const pattern = route.route;
            if (!pattern.includes(':')) continue;

            let regexStr = '^' + pattern
                .replace(/\(\/:[^)]+\)/g, '(?:/([^/]+))?')
                .replace(/:([^/]+)/g, '([^/]+)') + '$';

            const match = hash.match(new RegExp(regexStr));
            if (match) {
                return { route, params: match.slice(1).filter(p => p !== undefined) };
            }
        }

        return null;
    }

    async #navigate() {
        if (this.#isNavigating) return;
        this.#isNavigating = true;

        try {
            const hash = window.location.hash.substring(1) || '';
            const result = this.#findRoute(hash);

            if (!result) {
                if (hash) console.warn(`No route found for: ${hash}`);
                this.#isNavigating = false;
                return;
            }

            const { route, params } = result;

            if (this.#pageHost.children.length > 0) {
                ko.cleanNode(this.#pageHost);
            }

            const viewUrl = '/' + route.moduleId.replace(/^viewmodels\//, '');

            const response = await fetch(viewUrl);
            if (!response.ok) {
                this.#pageHost.innerHTML = '<div class="alert alert-danger mt-3">Failed to load page.</div>';
                this.#isNavigating = false;
                return;
            }

            const html = await response.text();
            this.#pageHost.innerHTML = html;

            const jsUrl = (route.jsPath.endsWith('.js') ? route.jsPath : route.jsPath + '.js')
                + '?v=' + (window.__appVersion || '1');
            const module = await import(jsUrl);
            const viewModel = module.default;

            if (viewModel && typeof viewModel.activate === 'function') {
                await viewModel.activate(...params);
            }

            if (viewModel) {
                ko.applyBindings(viewModel, this.#pageHost);
                this.#currentViewModel = viewModel;
            }

            if (viewModel && typeof viewModel.attached === 'function') {
                await viewModel.attached();
            }
        } catch (error) {
            console.error('Navigation error:', error);
            this.#pageHost.innerHTML = '<div class="alert alert-danger mt-3">An error occurred loading the page.</div>';
        } finally {
            this.#isNavigating = false;
        }
    }
}

const router = new AdminRouter();
router.init();
