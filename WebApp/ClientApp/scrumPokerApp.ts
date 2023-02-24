(function (angular: ng.IAngularStatic) {
    var app: ng.IModule = angular.module("scrumPokerApp", ["ngRoute"]);
    
    app.service("userAppStateService", webServices.UserService);

    app.service("DeckTypesService", deckDefinitions.DeckTypesService);

    class HomeController {
        constructor($scope: IHomeControllerScope, userAppStateService: webServices.UserService) {
            $scope.userName = userAppStateService.userName;
            $scope.displayName = userAppStateService.displayName;
            $scope.isAdmin = userAppStateService.isAdmin;
            $scope.teams = userAppStateService.getTeams();
        }
    }

    class DeckTypeController {
        constructor($scope: IDeckTypeControllerScope, deckTypesService: deckDefinitions.DeckTypesService) {
            $scope.deckTypes = deckTypesService.getAllDeckTypes();
        }
    }

    class DeckCardController {
        constructor($scope: IDeckCardControllerScope, deckTypesService: deckDefinitions.DeckTypesService) {
            $scope.fillColor = deckTypesService.fillColor;
            $scope.strokeColor = deckTypesService.strokeColor;
            $scope.textColor = deckTypesService.textColor;
            $scope.cards = deckTypesService.getCards();
        }
    }

    class VotingTypeController {
        constructor($scope: IDeckTypeControllerScope, deckTypesService: deckDefinitions.DeckTypesService) {
            $scope.deckTypes = deckTypesService.getAllDeckTypes();
        }
    }

    class NewSessionController {
        deckId: number;
        schemaId: number;
        allCards: deckDefinitions.ICardItem[] = [];
        hasErrors: boolean = true;

        constructor($scope: INewSessionControllerScope, $routeParams: INewSessionRouteParams, deckTypesService: deckDefinitions.DeckTypesService) {
            this.deckId = parseInt($routeParams.deckId);
            this.schemaId = parseInt($routeParams.schemaId);
            deckTypesService.selectDeck(this.deckId);
            var currentDeck: dataEntities.IDeckTypeEntity | undefined = deckTypesService.currentDeck;
            if (typeof currentDeck === 'undefined')
                return;
            deckTypesService.selectColor(this.schemaId, 0);
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
            $scope.members = [];
            var controller: NewSessionController = this;
            $scope.$watch('sprintName', function (newValue: string, oldValue: string) {
                if (newValue.trim().length == 0)
                    controller.hasErrors = true;
                else
                    controller.hasErrors = $scope.userStories.length == 0 || $scope.members.length == 0;
            });
            $scope.$watchCollection('userStories', function (newValue: scrumSession.IUserStory[], oldValue: scrumSession.IUserStory[]) {
                if (newValue.length == 0)
                    controller.hasErrors = true;
                else
                    controller.hasErrors = $scope.sprintName.trim().length == 0 || $scope.members.length == 0;
            });
            $scope.$watchCollection('members', function (newValue: scrumSession.ITeamMember[], oldValue: scrumSession.ITeamMember[]) {
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
        function ($routeProvider: ng.route.IRouteProvider, $locationProvider: ng.ILocationProvider) {
            $routeProvider
                .when('/newSession/:deckId', {
                    templateUrl: "newSession.htm",
                    controller: NewSessionController,
                    controllerAs: "controller"
                })
                .when('/home', {
                    templateUrl: "home.htm",
                    controller: HomeController,
                    controllerAs: "controller",
                    resolve: {
                        'userAppStateService': function (userAppStateService: webServices.UserService) {
                            return userAppStateService.promise;
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
