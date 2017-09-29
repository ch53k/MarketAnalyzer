angular.module('marketAnalyzer', ['ngRoute', 'ngSanitize', 'ui.bootstrap', 'dx']);

/* @ngInject */
angular.module('marketAnalyzer').config(['$routeProvider', function ($routeProvider) {

    $routeProvider.when('/', {
        template: '<stock></stock>'
    });

    $routeProvider.when('/register/:webinarId', {
        template: '<eahcs-register></eahcs-register>',
        reloadOnSearch: false
    });

    $routeProvider.when('/register/:webinarId/success', {
        template: '<eahcs-register-success></eahcs-register-success>'
    });

    $routeProvider.otherwise({ redirectTo: '/' });
}]);

/* @ngInject */
angular.module('marketAnalyzer').run(['$rootScope', '$route', function run($rootScope) {
    $rootScope.title = 'Market Analyzer';
}]);