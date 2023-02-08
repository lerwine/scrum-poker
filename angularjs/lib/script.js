(function (angular) {
    "use strict";
    console.log(JSON.stringify(angular));
    var app = angular.module("scrumPokerApp", ["ngRoute"]);
    app.service("DeckTypesService", function($http) {
        var deckTypes = [];
        var promise = $http.get('assets/CardSequences.json').then(function(result) {
            deckTypes = result.data;
            console.log("got: " + JSON.stringify(deckTypes));
        });
        return {
            promise: promise,
            getAllDeckTypes: function() {
                var result = deckTypes.map(function(value, index) {
                    return { id: index, name: value.name, description: value.description, previewUrl: 'assets/' + value.previewImage.file, height: value.previewImage.height, width: value.previewImage.width }
                });
                console.log("returning: " + JSON.stringify(result));
                return result;
            },
            getDeck: function(id) {
                if (id > -1 && id < deckTypes.length) {
                    var deck = deckTypes[id];
                    return {
                        name: deck.name,
                        previewImage: { url: 'assets/' + deck.previewImage, height: value.previewImage.height, width: value.previewImage.width },
                        description: deck.description,
                        cards: deck.cards.map(function(c, i) {
                            return { id: i, value: c.value, symbol: c.symbol, type: c.type, order: c.order, baseName: c.baseName };
                        })
                    };
                }

            }
        };
    });
    app.component("deckTypeList", {
        templateUrl: "deckTypeList.htm",
        controller: function DeckTypeListController($scope, DeckTypesService) {
            $scope.deckTypes = DeckTypesService.getAllDeckTypes();
        }
    });
    // app.controller("MainController", [
    //     "$scope",
    //     "$route",
    //     "$routeParams",
    //     "$location",
    //     function ($scope, $route, $routeParams, $location) {
    //         $scope.$route = $route;
    //         $scope.$location = $location;
    //         $scope.$routeParams = $routeParams;
    //     },
    // ])
    app.config([
        "$routeProvider",
        "$locationProvider",
        function ($routeProvider, $locationProvider) {
            $routeProvider
                .when('/home', { templateUrl: "home.htm"/*, controller: "MainController"*/,
                    resolve: {
                        'DeckTypesService': function(DeckTypesService) {
                            return DeckTypesService.promise;
                        }
                    }
                })
                .when('/', { redirectTo: "/home" })
            // configure html5 to get links working on jsfiddle
            $locationProvider.html5Mode(true);
        },
    ]);
})(window.angular);
