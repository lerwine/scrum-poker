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

declare interface IHomeTeamListItem {
    teamId: string;
    teamName: string;
    teamDescription?: string;
    isFacilitator: boolean;
    facilitatorName?: string;
}
declare interface IHomeControllerScope extends ng.IScope {
    displayName?: string;
    userName: string;
    isAdmin: boolean;
    teams: webServices.ITeamListItem[];
}

/**
 * @todo Rename to ICardDeckControllerScope
 */
declare interface IDeckTypeControllerScope extends ng.IScope {
    deckTypes: deckDefinitions.IDeckTypeListItem[];
}

declare interface IDeckCardControllerScope extends ng.IScope {
    fillColor: string;
    strokeColor: string;
    textColor: string;
    cards: (deckDefinitions.ISimpleParticpantCard | deckDefinitions.IPathParticpantCard)[];
}

declare interface INewSessionRouteParams extends ng.route.IRouteParamsService {
    deckId: string;
    schemaId: string;
}

declare interface INewSessionControllerScope extends ng.IScope {
    name: string;
    description: string;
    previewImageUrl: string;
    width: number;
    height: number;
    cards: (deckDefinitions.ISimpleParticpantCard | deckDefinitions.IPathParticpantCard)[];
    userStories: scrumSession.IUserStory[];
    members: scrumSession.ITeamMember[];
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