declare type CardType = "Ambiguous" | "Points" | "Unattainable" | "Abstain";

declare interface ICardDefinition {
    value: number;
    symbol: string;
    type: CardType;
    baseName: string;
}

declare interface ISheetDefinition {
    fileName: string;
    maxValue: number;
}

declare interface INameAndDescription {
    name: string;
    description: string;
}

declare interface IDeckTypeListItem extends INameAndDescription {
    id: number;
    previewUrl: string;
    width: string;
    height: string;
}

declare interface IPreviewImage {
    file: string;
    width: string;
    height: string;
}

declare interface IDeckTypeDefinition extends INameAndDescription {
    previewImage: IPreviewImage;
    cards: ICardDefinition[];
    sheets: ISheetDefinition[];
}

declare interface ICardItem extends ICardDefinition {
    id: number;
}

declare interface IDeckDetails {
    name: string;
    description: string;
    previewImage: Omit<IPreviewImage, "file"> & {
        url: string;
    };
    cards: ICardItem[];
}

declare type DeckColor = "Blue" | "Green" | "Red" | "Yellow";

declare interface IParticpantCard extends Omit<ICardItem, "baseName"> {
    url: string;
}

declare interface IDeckTypesServiceResult {
    promise: angular.IPromise<void>;
    getAllDeckTypes(): IDeckTypeListItem[];
    getDeck(id: number): IDeckDetails | undefined;
    getCards(deckId: number, color: DeckColor): IGetCardsResult | undefined;
}

declare interface IDeckTypeControllerScope extends ng.IScope {
    deckTypes: IDeckTypeListItem[];
}

declare interface IGetCardsResult {
    votingCardUrl: string;
    cards: IParticpantCard[];
}

declare interface INewSessionRouteParams extends ng.route.IRouteParamsService {
    deckId: string;
}

declare interface IUserStory {
    name: string;
}

declare interface IDeveloper {
    name: string;
}

declare interface INewSessionControllerScope extends ng.IScope {
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