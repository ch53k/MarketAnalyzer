angular.module('marketAnalyzer').factory('appUiService', appUiService);

function appUiService() {
    var service = {
        showLoadingPanel: showLoadingPanel,
        hideLoadingPanel:  hideLoadingPanel
    }

    return service;

    function getLoadingPanel() {
        return $('#appLoadingPanel').dxLoadPanel('instance');
    }

    function showLoadingPanel(message) {
        var loadingPanel = getLoadingPanel();
        if (message) {
            loadingPanel.option('message', message);
        }

        loadingPanel.option('visible', true);
    }

    function hideLoadingPanel() {
        var loadingPanel = getLoadingPanel();
        loadingPanel.option('message', 'Loading...');
        loadingPanel.option('visible', false);
    }
}