(function (angular: ng.IAngularStatic) {
    var app: ng.IModule = angular.module("scrumPokerApp", ["ngRoute"]);
    app.service("DeckTypesService", function ($http: ng.IHttpService): deckTypesService.IDeckTypesServiceResult {
        var deckDefinitions: dataEntities.IDeckDefinitions = {
            deckColors: [],
            deckTypes: []
        };
        var promise = $http.get<dataEntities.IDeckDefinitions>('assets/deck-definitions.json').then(function (result: ng.IHttpResponse<dataEntities.IDeckDefinitions>): void {
            deckDefinitions = result.data;
        });
        return {
            promise: promise,
            getAllDeckTypes: function (): deckTypesService.IDeckTypeListItem[] {
                return deckDefinitions.deckTypes.map<deckTypesService.IDeckTypeListItem>(function (item: dataEntities.IDeckTypeEntity, index: number): deckTypesService.IDeckTypeListItem {
                    return {
                        id: index, name: item.name, description: item.description, previewUrl: 'assets/' + item.previewImage.fileName,
                        height: item.previewImage.height, width: item.previewImage.width
                    };
                });
            },
            getDeck: function (id: number): deckTypesService.IDeckDetails | undefined {
                if (isNaN(id) || id < 0 || id >= deckDefinitions.deckTypes.length)
                    return;
                var deck: dataEntities.IDeckTypeEntity = deckDefinitions.deckTypes[id];
                return {
                    name: deck.name,
                    previewImage: { url: 'assets/' + deck.previewImage, height: deck.previewImage.height, width: deck.previewImage.width },
                    description: deck.description,
                    cards: deck.cards.map(function (c: dataEntities.ICardEntity, i: number) {
                        return { id: i, value: c.value, symbol: c.symbol, type: c.type, baseName: c.baseName };
                    })
                };
            },
            getCards: function (deckId: number, color: string): deckTypesService.IGetCardsResult | undefined {
                if (isNaN(deckId) || deckId < 0 || deckId >= deckDefinitions.deckTypes.length)
                    return;
                return {
                    votingCardUrl: 'assets/Voting-' + color + ".svg",
                    cards: deckDefinitions.deckTypes[deckId].cards.map<deckTypesService.IParticpantCard>(function (item: dataEntities.ICardEntity, index: number): deckTypesService.IParticpantCard {
                        return {
                            id: index,
                            value: item.value,
                            symbol: item.symbol,
                            type: item.type,
                            url: 'assets/' + item.baseName + "-" + color + ".svg"
                        }
                    })
                };
            },
            getDeckColors: function(): string[] { return deckDefinitions.deckColors; }
        };
    });

    class DeckTypeController {
        constructor($scope: IDeckTypeControllerScope, DeckTypesService: deckTypesService.IDeckTypesServiceResult) {
            $scope.deckTypes = DeckTypesService.getAllDeckTypes();
        }
    }

    class NewSessionController {
        deckId: number;
        allCards: deckTypesService.ICardItem[] = [];
        hasErrors: boolean = true;

        constructor($scope: INewSessionControllerScope, $routeParams: INewSessionRouteParams, DeckTypesService: deckTypesService.IDeckTypesServiceResult) {
            this.deckId = parseInt($routeParams.deckId);
            var deckDetails: deckTypesService.IDeckDetails | undefined = DeckTypesService.getDeck(this.deckId);
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
            var controller: NewSessionController = this;
            $scope.$watch('sprintName', function (newValue: string, oldValue: string) {
                if (newValue.trim().length == 0)
                    controller.hasErrors = true;
                else
                    controller.hasErrors = $scope.userStories.length == 0 || $scope.developers.length == 0;
            });
            $scope.$watchCollection('userStories', function (newValue: IUserStory[], oldValue: IUserStory[]) {
                if (newValue.length == 0)
                    controller.hasErrors = true;
                else
                    controller.hasErrors = $scope.sprintName.trim().length == 0 || $scope.developers.length == 0;
            });
            $scope.$watchCollection('developers', function (newValue: IDeveloper[], oldValue: IDeveloper[]) {
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
        function ($routeProvider: ng.route.IRouteProvider, $locationProvider: ng.ILocationProvider) {
            $routeProvider
                .when('/newSession/:deckId', {
                    templateUrl: "newSession.htm",
                    controller: NewSessionController,
                    controllerAs: "controller"
                })
                .when('/home', {
                    templateUrl: "home.htm"/*, controller: "MainController"*/,
                    resolve: {
                        'DeckTypesService': function (DeckTypesService: deckTypesService.IDeckTypesServiceResult) {
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
