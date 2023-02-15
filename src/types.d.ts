declare interface INameAndDescription {
    name: string;
    description: string;
}

/**
 * Defines graphical image dimensions.
 */
declare interface ISize {
    /**
     * The width of the graphic, in pixels.
     */
    width: number;

    /**
     * The height of the graphic, in pixels.
     */
    height: number;
}

declare interface IDeckTypeControllerScope extends ng.IScope {
    deckTypes: deckTypesService.IDeckTypeListItem[];
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
    width: number;
    height: number;
    cards: deckTypesService.ICardItem[];
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