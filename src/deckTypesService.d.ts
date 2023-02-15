declare namespace deckTypesService {
    export interface IDeckTypeListItem extends INameAndDescription {
        id: number;
        previewUrl: string;
        width: number;
        height: number;
    }
    
    export interface ICardItem extends dataEntities.ICardEntity {
        id: number;
    }

    export interface IDeckDetails {
        name: string;
        description: string;
        previewImage: ISize & {
            url: string;
        };
        cards: ICardItem[];
    }

    export interface IDeckTypesServiceResult {
        promise: angular.IPromise<void>;
        getAllDeckTypes(): IDeckTypeListItem[];
        getDeck(id: number): IDeckDetails | undefined;
        getCards(deckId: number, color: string): IGetCardsResult | undefined;
        getDeckColors(): string[];
    }

    export interface IParticpantCard extends Omit<ICardItem, "baseName"> {
        url: string;
    }

    export interface IGetCardsResult {
        votingCardUrl: string;
        cards: IParticpantCard[];
    }
}