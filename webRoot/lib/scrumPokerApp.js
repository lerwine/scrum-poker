"use strict";
(function (angular) {
    var app = angular.module("scrumPokerApp", ["ngRoute"]);
    app.service("DeckTypesService", function ($http) {
        var deckTypes = [];
        var promise = $http.get('assets/CardSequences.json').then(function (result) {
            deckTypes = result.data;
        });
        return {
            promise: promise,
            getAllDeckTypes: function () {
                return deckTypes.map(function (item, index) {
                    return {
                        id: index, name: item.name, description: item.description, previewUrl: 'assets/' + item.previewImage.file,
                        height: item.previewImage.height, width: item.previewImage.width
                    };
                });
            },
            getDeck: function (id) {
                if (isNaN(id) || id < 0 || id >= deckTypes.length)
                    return;
                var deck = deckTypes[id];
                return {
                    name: deck.name,
                    previewImage: { url: 'assets/' + deck.previewImage, height: deck.previewImage.height, width: deck.previewImage.width },
                    description: deck.description,
                    cards: deck.cards.map(function (c, i) {
                        return { id: i, value: c.value, symbol: c.symbol, type: c.type, baseName: c.baseName };
                    })
                };
            },
            getCards: function (deckId, color) {
                if (isNaN(deckId) || deckId < 0 || deckId >= deckTypes.length)
                    return;
                return {
                    votingCardUrl: 'assets/Voting-' + color + ".svg",
                    cards: deckTypes[deckId].cards.map(function (item, index) {
                        return {
                            id: index,
                            value: item.value,
                            symbol: item.symbol,
                            type: item.type,
                            url: 'assets/' + item.baseName + "-" + color + ".svg"
                        };
                    })
                };
            }
        };
    });
    class DeckTypeController {
        constructor($scope, DeckTypesService) {
            $scope.deckTypes = DeckTypesService.getAllDeckTypes();
        }
    }
    class NewSessionController {
        constructor($scope, $routeParams, DeckTypesService) {
            this.allCards = [];
            this.hasErrors = true;
            this.deckId = parseInt($routeParams.deckId);
            var deckDetails = DeckTypesService.getDeck(this.deckId);
            if (typeof deckDetails === 'undefined')
                return;
            this.allCards = deckDetails.cards;
            $scope.name = deckDetails.name;
            $scope.description = deckDetails.description;
            $scope.previewImageUrl = deckDetails.previewImage.url;
            $scope.width = deckDetails.previewImage.width;
            $scope.height = deckDetails.previewImage.height;
            $scope.projectName = '';
            $scope.themeName = '';
            $scope.initiativeName = '';
            $scope.epicName = '';
            $scope.milestoneName = '';
            $scope.sprintName = '';
            $scope.details = '';
            $scope.zeroPointsCard = true;
            $scope.halfPointCard = true;
            $scope.infinityCard = true;
            $scope.needInfoCard = true;
            $scope.cards = [];
            $scope.userStories = [];
            $scope.developers = [];
            var controller = this;
            $scope.$watch('sprintName', function (newValue, oldValue) {
                if (newValue.trim().length == 0)
                    controller.hasErrors = true;
                else
                    controller.hasErrors = $scope.userStories.length == 0 || $scope.developers.length == 0;
            });
            $scope.$watchCollection('userStories', function (newValue, oldValue) {
                if (newValue.length == 0)
                    controller.hasErrors = true;
                else
                    controller.hasErrors = $scope.sprintName.trim().length == 0 || $scope.developers.length == 0;
            });
            $scope.$watchCollection('developers', function (newValue, oldValue) {
                if (newValue.length == 0)
                    controller.hasErrors = true;
                else
                    controller.hasErrors = $scope.sprintName.trim().length == 0 || $scope.userStories.length == 0;
            });
        }
    }
    app.component("deckTypeList", {
        templateUrl: "deckTypeList.htm",
        controller: DeckTypeController
    });
    app.config([
        "$routeProvider",
        "$locationProvider",
        function ($routeProvider, $locationProvider) {
            $routeProvider
                .when('/newSession/:deckId', {
                    templateUrl: "newSession.htm",
                    controller: NewSessionController,
                    controllerAs: "controller"
                })
                .when('/home', {
                    templateUrl: "home.htm" /*, controller: "MainController"*/,
                    resolve: {
                        'DeckTypesService': function (DeckTypesService) {
                            return DeckTypesService.promise;
                        }
                    }
                })
                .when('/', {
                    redirectTo: "/home"
                });
            // configure html5 to get links working on jsfiddle
            $locationProvider.html5Mode(true);
        },
    ]);
})(window.angular);
//# sourceMappingURL=scrumPokerApp.js.map