angular.module('marketAnalyzer').component('marketAnalyzerApp', {
    templateUrl: 'modules/app/partials/app.partial.html',
    controller: MarketAnalyzerController,
    bindings: {
    }
});

/* @ngInject */
function MarketAnalyzerController($rootScope, $routeParams, appDataService, appUiService) {
    
    var $ctrl = this;
    
}