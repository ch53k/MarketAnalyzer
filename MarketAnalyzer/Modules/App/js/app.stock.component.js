angular.module('marketAnalyzer').component('stock', {
    templateUrl: 'modules/app/partials/stock.partial.html',
    controller: StockController,
    bindings: {
    }
});

/* @ngInject */
function StockController($rootScope, $q, $http, $uibModal, appDataService, appUiService) {

    var $ctrl = this;

    $ctrl.$onInit = function() {
    }

    $ctrl.errageMessage = '';

    var activeTicker = 'MSFT';
    $ctrl.activeStock = {};

    //#region Load Available Tickers
    $ctrl.availableTickers = [];
    loadAvailableTickers();

    function loadAvailableTickers() {
        $http.get('api/stocks/tickers').then(function(response) {
                $ctrl.availableTickers.length = 0;
                _.forEach(response.data,
                    function(value) {
                        $ctrl.availableTickers.push(value);
                    });
            },
            function (resonse) {
                $ctrl.errorMessage = response.data;
            });
    }
    //#endregion

    //#region Ticker Search
    $ctrl.tickerSelectBoxValue = '';

    $ctrl.tickerSelectBox = {
        bindingOptions: { value: '$ctrl.tickerSelectBoxValue', items: '$ctrl.availableTickers' },
        placeholder: 'Type a Ticker Symbol (MSFT)...',
        searchMode: 'startswith',
        searchEnabled: true,
        acceptCustomValue: true,
        onEnterKey: function() {
            $ctrl.searchTicker();
        }
    };

    $ctrl.searchTicker = function() {
        activeTicker = $ctrl.tickerSelectBoxValue;
        if ($ctrl.availableTickers.indexOf(activeTicker.toUpperCase()) > -1) {
            loadTicker();
            return;
        }

        if (!activeTicker) {
            return;
        }

        var modal = $uibModal.open({
            size: 'md',
            component: 'appMessageBox',
            windowClass: 'modal-zindex',
            keyboard: false,
            backdrop: 'static',
            resolve: {
                title: function() {
                    return 'Load Stock Data';
                },
                message: function() {
                    return 'This stock hasn\'t been loaded, would you like to load it?';
                },
                okButton: function() {
                    return 'Yes';
                },
                showCancelButton: function() {
                    return true;
                }
            }

        });

        modal.result.then(function() {
                appUiService.showLoadingPanel();
                appDataService.loadStock(activeTicker).then(function() {
                        appUiService.hideLoadingPanel();
                        loadAvailableTickers();
                        loadTicker();
                    },
                    function(response) {
                        $ctrl.errageMessage = response.data.message;
                        appUiService.hideLoadingPanel();
                    });
            },
            function() {});
    }

    function loadTicker() {
        if (activeTicker.length > 0) {
            refreshChart();
        }

        $ctrl.tickerSelectBoxValue = '';
    }
    //#endregion

    //#region Chart
    $ctrl.chartTitle = '';
    var chartInstance;

    var preSetStartDate;
    var preSetEndDate;

    var chartFilters = {
        threeDay: {
            display: '3 Day',
            startDate: moment().startOf('day').subtract(3, 'd'),
            key: '3d',
            chartType: 'candlestick'
        },
        fiveDay: {
            display: '5 Day',
            startDate: moment().startOf('day').subtract(5, 'd'),
            key: '5d',
            chartType: 'candlestick'
        },
        month: {
            display: '1 Month',
            startDate: moment().startOf('day').subtract(1, 'M'),
            key: '1m',
            chartType: 'candlestick'
        },
        sixmonth: {
            display: '6 Month',
            startDate: moment().startOf('day').subtract(6, 'M'),
            key: '6m',
            chartType: 'spline'
        },
        ytd: { display: 'Year To Date', startDate: moment().startOf('year'), key: 'ytd', chartType: 'spline' },
        oneYear: {
            display: '1 Year',
            startDate: moment().startOf('day').subtract(1, 'y'),
            key: '1y',
            chartType: 'spline'
        },
        fiveYear: {
            display: '5 Year',
            startDate: moment().startOf('day').subtract(5, 'y'),
            key: '5y',
            chartType: 'spline'
        },
        max: { display: 'Max', startDate: moment('1900-01-01'), key: 'max', chartType: 'spline' },
        custom: { display: 'Custom', startDate: undefined, key: 'custom', chartType: 'spline' },
        findFilter: function(key) {
            return _.find(chartFilters,
                function(o) {
                    return o.key == key;
                });
        }
    };

    $ctrl.chartFilter = {
        preDefined: chartFilters.threeDay.key,
        startDate: moment().startOf('day').subtract(3, 'd').toDate(),
        endDate: moment().startOf('day').toDate()
    };


    function refreshChart() {
        var filter = chartFilters.findFilter($ctrl.chartFilter.preDefined);
        if (!filter) {
            return;
        }

        chartInstance.beginUpdate();
        if (filter.chartType == 'candlestick' ||
            moment($ctrl.chartFilter.endDate).diff($ctrl.chartFilter.startDate, 'days') <= 30) {
            chartInstance.option('commonSeriesSettings',
                {
                    argumentField: 'date',
                    type: 'candlestick'
                });
            chartInstance.option('series',
                [
                    {
                        name: activeTicker,
                        openValueField: 'open',
                        highValueField: 'high',
                        lowValueField: 'low',
                        closeValueField: 'close',
                        reduction: {
                            color: 'red'
                        }
                    }
                ]);
            chartInstance.option('tooltip',
                {
                    enabled: true,
                    location: 'edge',
                    customizeTooltip: function(arg) {
                        return {
                            text: 'Date: ' +
                                arg.argumentText +
                                '<br/>' +
                                'Open: $' +
                                arg.openValue +
                                '<br/>' +
                                'Close: $' +
                                arg.closeValue +
                                '<br/>' +
                                'High: $' +
                                arg.highValue +
                                '<br/>' +
                                'Low: $' +
                                arg.lowValue +
                                '<br/>'
                        };
                    }
                });
        } else if (filter.chartType == 'spline') {
            chartInstance.option('commonSeriesSettings',
                {
                    argumentField: 'date',
                    type: 'spline',
                    point: { visible: false }
                });
            chartInstance.option('series',
                [
                    { valueField: 'close', name: activeTicker }
                ]);
            chartInstance.option('tooltip',
                {
                    enabled: true,
                    location: 'edge',
                    customizeTooltip: function(arg) {
                        return {
                            text: 'Date: ' +
                                arg.argumentText +
                                '<br/>' +
                                'Close: $' +
                                arg.valueText
                        };
                    }
                });
        }
        var ds = getNewChartDataSource();
        chartInstance.showLoadingIndicator();
        chartInstance.option('dataSource', ds);
        chartInstance.endUpdate();
    }

    function getNewChartDataSource() {
        return new DevExpress.data.DataSource({
            load: function() {
                var d = new $q.defer();
                var url = 'api/stocks/chart?ticker=' + activeTicker;
                if ($ctrl.chartFilter.preDefined === chartFilters.threeDay.key) {
                    url = url + '&take=' + 3;
                } else if ($ctrl.chartFilter.preDefined === chartFilters.fiveDay.key) {
                    url = url + '&take=' + 5;
                } else {
                    url = url +
                        '&start=' +
                        moment($ctrl.chartFilter.startDate).format('YYYYMMDD') +
                        '&end=' +
                        moment($ctrl.chartFilter.endDate).format('YYYYMMDD');
                }

                $http.get(url).then(function(response) {
                        if ($ctrl.chartFilter.preDefined == chartFilters.threeDay.key ||
                            $ctrl.chartFilter.preDefined == chartFilters.fiveDay.key ||
                            $ctrl.chartFilter.preDefined == chartFilters.max.key) {
                            var minObject = _.minBy(response.data.chartQuotes,
                                function(o) {
                                    return o.date;
                                });
                            preSetStartDate = new Date(minObject.date);
                            $ctrl.chartFilter.startDate = preSetStartDate;
                            chartInstance.hideLoadingIndicator();
                        }
                        angular.copy(response.data, $ctrl.activeStock);
                        return d.resolve(response.data.chartQuotes);
                    },
                    function (response) {
                        $ctrl.errageMessage = response.data;
                        return $q.reject(response);
                    });
                return d.promise;
            }
        });
    }


    $ctrl.chartOptions = {
        bindingOptions: {
            'title': '$ctrl.chartTitle'
        },
        onInitialized: function(e) {
            chartInstance = e.component;
        },
        palette: 'Harmony Light',
        legend: {
            itemTextPosition: 'left'
        },
        commonSeriesSettings: {
            argumentField: 'date',
            type: 'spline',
            point: { visible: false }
        },
        argumentAxis: {
            valueMarginsEnabled: false
        },
        margin: {
            bottom: 20
        },
        series: [
            { valueField: 'close', name: activeTicker }
        ],
        valueAxis: {
            tickInterval: 1,
            title: {
                text: 'US dollars'
            },
            label: {
                format: {
                    type: 'currency',
                    precision: 0
                }
            }
        },
        "export": {
            enabled: true
        },
        tooltip: {
            enabled: true
        }
    };
    //#endregion

    //#region Chart Filter Form
    var processedChartFilterPreDefined;
    var isNotCustomerEntry = false;
    $ctrl.chartFilterForm = {
        bindingOptions: {
            formData: '$ctrl.chartFilter'
        },
        labelLocation: 'top',
        colCount: 3,
        onFieldDataChanged: function(e) {
            if (e.dataField == 'startDate') {
                var endDateEditor = $('#chartfilterForm').dxForm('instance').getEditor('endDate');
                endDateEditor.option('min', e.value);
                //if (isNotCustomerEntry){
                //    isNotCustomerEntry = false;
                //    return;
                //}
                if (preSetStartDate && preSetStartDate != e.value) {
                    $ctrl.chartFilter.preDefined = 'custom';
                    loadTicker();
                }
                return;
            }
            if (e.dataField == 'endDate') {
                //if (isNotCustomerEntry){
                //    return;
                //}
                if (preSetEndDate && preSetEndDate != e.value) {
                    $ctrl.chartFilter.preDefined = 'custom';
                    loadTicker();
                }
                return;
            }
            if (e.dataField == 'preDefined' && e.value && processedChartFilterPreDefined != e.value) {
                if (e.value != 'custom') {
                    var filter = chartFilters.findFilter(e.value);
                    preSetStartDate = filter.startDate.toDate();
                    $ctrl.chartFilter.startDate = preSetStartDate;
                    preSetEndDate = moment().startOf('day').toDate();
                    $ctrl.chartFilter.endDate = preSetEndDate;
                    processedChartFilterPreDefined = e.value;
                    isNotCustomerEntry = true;
                    loadTicker();
                }
            }
        },
        items: [
            {
                dataField: 'preDefined',
                label: {
                    text: 'Filter'
                },
                editorType: 'dxSelectBox',
                editorOptions: {
                    items: [
                        chartFilters.threeDay, chartFilters.fiveDay, chartFilters.month, chartFilters.sixmonth,
                        chartFilters.ytd, chartFilters.oneYear, chartFilters.fiveYear, chartFilters.max,
                        chartFilters.custom
                    ],
                    displayExpr: 'display',
                    valueExpr: 'key'
                }
            },
            {
                dataField: 'startDate',
                editorType: 'dxDateBox',
                editorOptions: {
                    width: '100%'
                }
            },
            {
                dataField: 'endDate',
                editorType: 'dxDateBox',
                editorOptions: {
                    width: '100%',
                    max: new Date()
                }
            }
        ]
    }
    //#endregion
}