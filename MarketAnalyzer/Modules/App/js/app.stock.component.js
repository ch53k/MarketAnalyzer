angular.module('marketAnalyzer').component('stock', {
    templateUrl: 'modules/app/partials/stock.partial.html',
    controller: StockController,
    bindings: {
    }
});

/* @ngInject */
function StockController($rootScope, $q, $http, $uibModal, appDataService, appUiService) {
    
    var $ctrl = this;

    //#region Ticker Search
    var tickerSelected = '';
    var availableTickers = [];
    $ctrl.tickerSearch = {
        dataSource: new DevExpress.data.DataSource({
            load: function (loadOptions) {
                var d = new $q.defer();
                $http.get('api/stocks/tickers').then(function (response) {
                        availableTickers = response.data;
                        return d.resolve(response);
                    },
                    function (response) {
                        return $q.reject(response);
                    });
                return d.promise;
            }
        }),
        placeholder: 'Type A Ticker (MSFT)...',
        onSelectionChanged: function (data) {
            tickerSelected = data.selectedItem;
        }
    }

    $ctrl.searchTicker = function() {
        if (availableTickers.indexOf(tickerSelected.toUpperCase()) > -1) {
            loadTicker();
            return;
        }

        if (!tickerSelected) {
            return;
        }

        var modal = $uibModal.open({
            size: 'md',
            component: 'appMessageBox',
            keyboard: false,
            backdrop: 'static',
            resolve: {
                title: function () {
                    return 'Load Stock Data';
                },
                message: function () {
                    return 'This stock hasn\'t been loaded, would you like to load it?';
                },
                okButton: function () {
                    return 'Yes';
                },
                showCancelButton: function () {
                    return true;
                }
            }

        });
        
        modal.result.then(function () {
            appUiService.showLoadingPanel();
            appDataService.loadStock(tickerSelected).then(function () {
                appUiService.hideLoadingPanel();
                loadTicker();
            }, function (response) {
                //TODO: Handel error.
                appUiService.hideLoadingPanel();
            });
        }, function () { });
    }

    function loadTicker() {
        if (tickerSelected.length > 0) {
            refreshChart();
        }
    }
    //#endregion

    //#region Chart
    $ctrl.chartTitle = '';
    var chartInstance;

    function refreshChart() {
        var ds = new DevExpress.data.DataSource({
            load: function(loadOptions) {
                var d = new $q.defer();
                $http.get('api/stocks/chart?ticker=' +
                    tickerSelected +
                    '&start=' +
                    moment().subtract(7, 'd').format('YYYYMMDD')+
                    '&end=' +
                    moment().format('YYYYMMDD')).then(function(response) {
                        return d.resolve(response);
                    },
                    function(response) {
                        return $q.reject(response);
                    });
                return d.promise;
            }
        });
        chartInstance.option('dataSource', ds);
    }

    $ctrl.chartOptions = {
        bindingOptions: {
          'title': '$ctrl.chartTitle'  
        },
        onInitialized: function (e) {
            chartInstance = e.component;
        },
        palette: 'Harmony Light',
        commonSeriesSettings: {
            argumentField: 'date',
            type: 'spline'
        },
        argumentAxis: {
            valueMarginsEnabled: false
        },
        margin: {
            bottom: 20
        },
        series: [
            { valueField: 'close', name: 'Close' }
        ],
        "export": {
            enabled: true
        },
        tooltip:{
            enabled: true
        },
        legend: {
            verticalAlignment: 'bottom',
            horizontalAlignment: 'center'
        }
    };
    //#endregion

    //#region Chart Filter
    var chartFilters= {
        day: '1 Day', 
    }

    $ctrl.chartFilter = {
        dataSource =
    }
    //#endregion

    $ctrl.tickerLoad = '';
    $ctrl.tickerEntry = {
        bindingOptions:{'value': '$ctrl.tickerLoad'}
    }
}