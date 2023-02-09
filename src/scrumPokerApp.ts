type CardType = "Ambiguous" | "Points" | "Unattainable" | "Abstain";
interface ICardDefinition {
    value: number;
    symbol: string;
    type: CardType;
    baseName: string;
}
interface ISheetDefinition {
    fileName: string;
    maxValue: number;
}
interface INameAndDescription {
    name: string;
    description: string;
}
interface IDeckTypeListItem extends INameAndDescription {
    id: number;
    previewUrl: string;
    width: string;
    height: string;
}
interface IPreviewImage {
    file: string;
    width: string;
    height: string;
}
interface IDeckTypeDefinition extends INameAndDescription {
    previewImage: IPreviewImage;
    cards: ICardDefinition[];
    sheets: ISheetDefinition[];
}
interface ICardItem extends ICardDefinition {
    id: number;
}
interface IDeckDetails {
    name: string;
    description: string;
    previewImage: Omit<IPreviewImage, "file"> & {
        url: string;
    };
    cards: ICardItem[];
}
type DeckColor = "Blue" | "Green" | "Red" | "Yellow";
interface IParticpantCard extends Omit<ICardItem, "baseName"> {
    url: string;
}
interface IDeckTypesServiceResult {
    promise: angular.IPromise<void>;
    getAllDeckTypes(): IDeckTypeListItem[];
    getDeck(id: number): IDeckDetails | undefined;
    getCards(deckId: number, color: DeckColor): IGetCardsResult | undefined;
}
interface IDeckTypeControllerScope extends ng.IScope {
    deckTypes: IDeckTypeListItem[];
}
interface IGetCardsResult {
    votingCardUrl: string;
    cards: IParticpantCard[];
}
interface INewSessionRouteParams extends ng.route.IRouteParamsService {
    deckId: string;
}
interface IUserStory {
    name: string;
}
interface IDeveloper {
    name: string;
}
interface INewSessionControllerScope extends ng.IScope {
    name: string;
    description: string;
    previewImageUrl: string;
    width: string;
    height: string;
    cards: ICardItem[];
    userStories: IUserStory[];
    developers: IDeveloper[];
    projectName: string;
    themeName: string;
    initiativeName: string;
    epicName: string;
    milestoneName: string;
    sprintName: string;
    details: string;
    zeroPointsCard: boolean;
    halfPointCard: boolean;
    infinityCard: boolean;
    needInfoCard: boolean;
}
(function (angular: ng.IAngularStatic) {
    var app: ng.IModule = angular.module("scrumPokerApp", ["ngRoute"]);
    app.service("DeckTypesService", function ($http: ng.IHttpService): IDeckTypesServiceResult {
        var deckTypes: IDeckTypeDefinition[] = [];
        var promise = $http.get<IDeckTypeDefinition[]>('assets/CardSequences.json').then(function (result: ng.IHttpResponse<IDeckTypeDefinition[]>): void {
            deckTypes = result.data;
        });
        return {
            promise: promise,
            getAllDeckTypes: function (): IDeckTypeListItem[] {
                return deckTypes.map<IDeckTypeListItem>(function (item: IDeckTypeDefinition, index: number): IDeckTypeListItem {
                    return {
                        id: index, name: item.name, description: item.description, previewUrl: 'assets/' + item.previewImage.file,
                        height: item.previewImage.height, width: item.previewImage.width
                    };
                });
            },
            getDeck: function (id: number): IDeckDetails | undefined {
                if (isNaN(id) || id < 0 || id >= deckTypes.length)
                    return;
                var deck: IDeckTypeDefinition = deckTypes[id];
                return {
                    name: deck.name,
                    previewImage: { url: 'assets/' + deck.previewImage, height: deck.previewImage.height, width: deck.previewImage.width },
                    description: deck.description,
                    cards: deck.cards.map(function (c: ICardDefinition, i: number) {
                        return { id: i, value: c.value, symbol: c.symbol, type: c.type, baseName: c.baseName };
                    })
                };
            },
            getCards: function (deckId: number, color: DeckColor): IGetCardsResult | undefined {
                if (isNaN(deckId) || deckId < 0 || deckId >= deckTypes.length)
                    return;
                return {
                    votingCardUrl: 'assets/Voting-' + color + ".svg",
                    cards: deckTypes[deckId].cards.map<IParticpantCard>(function (item: ICardDefinition, index: number): IParticpantCard {
                        return {
                            id: index,
                            value: item.value,
                            symbol: item.symbol,
                            type: item.type,
                            url: 'assets/' + item.baseName + "-" + color + ".svg"
                        }
                    })
                };
            }
        };
    });
    class DeckTypeController {
        constructor($scope: IDeckTypeControllerScope, DeckTypesService: IDeckTypesServiceResult) {
            $scope.deckTypes = DeckTypesService.getAllDeckTypes();
        }
    }
    class NewSessionController {
        deckId: number;
        allCards: ICardItem[] = [];
        hasErrors: boolean = true;

        constructor($scope: INewSessionControllerScope, $routeParams: INewSessionRouteParams, DeckTypesService: IDeckTypesServiceResult) {
            this.deckId = parseInt($routeParams.deckId);
            var deckDetails: IDeckDetails | undefined = DeckTypesService.getDeck(this.deckId);
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
                        'DeckTypesService': function (DeckTypesService: IDeckTypesServiceResult) {
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
