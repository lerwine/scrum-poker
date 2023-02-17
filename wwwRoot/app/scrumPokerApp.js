"use strict";
(function (angular) {
    var app = angular.module("scrumPokerApp", ["ngRoute"]);
    var s = "{\"title\":\"My Sprint Session\",\"adminUser\":{\"assignedPoints\":0,\"displayName\":\"Admin\",\"userName\":\"admin\"}," +
        "\"currentScopePoints\":0,\"deckId\":1,\"developers\":[{\"assignedPoints\":0,\"displayName\":\"Paul\",\"userName\":\"pmc\"}," +
        "{\"assignedPoints\":0,\"displayName\":\"John\",\"userName\":\"walrus\"}],\"projects\":[],\"stories\":[{\"identifier\":\"SPNT0010002\"," +
        "\"title\":\"Procurement Form\",\"_points\":null,\"created\":\"\\/Date(1676583255847-0500)\\/\",\"order\":0,\"preRequisiteIds\":[],\"state\":0}," +
        "{\"identifier\":\"SPNT0010002\",\"title\":\"Procurement Form\",\"_points\":null,\"created\":\"\\/Date(1676583255857-0500)\\/\"," +
        "\"order\":0,\"preRequisiteIds\":[0],\"state\":0}],\"themes\":[]}";
    app.service("DeckTypesService", deckDefinitions.DeckTypesService);
    // app.service("DeckTypesService", function ($http: ng.IHttpService): deckTypesService.IDeckTypesServiceResult {
    //     var deckDefinitions: dataEntities.IDeckDefinitions = {
    //         votingCard: {
    //             fill: "",
    //             stroke: "",
    //             text: ""
    //         },
    //         deckColors: [],
    //         deckTypes: []
    //     };
    //     var promise = $http.get<dataEntities.IDeckDefinitions>('assets/deck-definitions.json').then(function (result: ng.IHttpResponse<dataEntities.IDeckDefinitions>): void {
    //         deckDefinitions.votingCard = result.data.votingCard;
    //         deckDefinitions.deckColors = result.data.deckColors;
    //         deckDefinitions.deckTypes = result.data.deckTypes;
    //     });
    //     return new DeckTypesService(deckDefinitions, promise);
    // });
    class DeckTypeController {
        constructor($scope, deckTypesService) {
            $scope.deckTypes = deckTypesService.getAllDeckTypes();
        }
    }
    class DeckCardController {
        constructor($scope, deckTypesService) {
            $scope.fillColor = deckTypesService.fillColor;
            $scope.strokeColor = deckTypesService.strokeColor;
            $scope.textColor = deckTypesService.textColor;
            $scope.cards = deckTypesService.getCards();
        }
    }
    class VotingTypeController {
        constructor($scope, deckTypesService) {
            $scope.deckTypes = deckTypesService.getAllDeckTypes();
        }
    }
    class NewSessionController {
        constructor($scope, $routeParams, deckTypesService) {
            this.allCards = [];
            this.hasErrors = true;
            this.deckId = parseInt($routeParams.deckId);
            this.colorId = parseInt($routeParams.colorId);
            deckTypesService.selectDeck(this.deckId);
            var currentDeck = deckTypesService.currentDeck;
            if (typeof currentDeck === 'undefined')
                return;
            deckTypesService.selectColor(0);
            this.allCards = deckTypesService.getCards();
            $scope.name = currentDeck.name;
            $scope.description = currentDeck.description;
            $scope.previewImageUrl = "assets/" + currentDeck.previewImage.fileName;
            $scope.width = currentDeck.previewImage.width;
            $scope.height = currentDeck.previewImage.height;
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
    app.component("deckCard", {
        templateUrl: "deckCard.htm",
        controller: DeckCardController
    });
    app.component("votingCard", {
        templateUrl: "votingCard.htm",
        controller: VotingTypeController
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
                    'deckTypesService': function (deckTypesService) {
                        return deckTypesService.promise;
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