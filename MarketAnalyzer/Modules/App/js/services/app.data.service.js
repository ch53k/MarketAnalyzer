angular.module('marketAnalyzer').factory('appDataService', appDataService);

function appDataService($http) {
    var service = {
        getStocks: getStocks,
        loadStock: loadStock
    }
    return service;

    function getStocks(webinarId) {
        return events[webinarId];
    }

    function loadStock(ticker) {
        return $http.post('/api/stocks/load?ticker=' + ticker);
    }
}