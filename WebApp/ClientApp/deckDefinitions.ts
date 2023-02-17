namespace deckDefinitions {
    export interface IDeckTypeListItem extends INameAndDescription {
        id: number;
        previewUrl: string;
        width: number;
        height: number;
    }
    
    export interface ICardItem {
        /**
         * The unique identifier of the card within the parent deck.
         */
        id: number;

        /**
         * The value of the card in points.
         */
        value: number;

        /**
         * The character symbol of the card or the string representation of its numeric value.
         */
        symbolText: string;

        /**
         * The generalized card type.
         */
        type: dataEntities.CardType;

        /**
         * Title text for the card.
         * @TODO: Add to definition
         */
        title: string;

        /**
         * Shortened title text for the card.
         * @TODO: Add to definition
         */
        shortTitle?: string;

        /**
         * The description for the card.
         */
        description?: string;

        /**
         * The truncated description for the text.
         */
        truncatedDescription?: string;

        /**
         * The brief details in italics which follow the description text without a line break.
         */
        briefDetails?: string;

        /**
         * THe full details in italics which appear under the description text.
         */
        fullDetails?: string;
    }

    export interface ISimpleCardItem extends ICardItem {
        /**
         * The symbol font name.
         */
        symbolFont: string;

        /**
         * The y-coordinate for the middle symbol.
         */
        middleSymbolTop: number;

        /**
         * The font size in pixels for the top and bottom symbol text.
         */
        smallSymbolFontSize: number;

        /**
         * The font size in pixels for the large center symbol text.
         */
        largeSymbolFontSize: number;
    }

    export interface IPathCardItem extends ICardItem {
        /**
         * The graphics path data for the upper symbol.
         */
        upperSymbolPath: string;
        
        /**
         * The graphics path data for the center symbol.
         */
        middleSymbolPath: string;
        
        /**
         * The graphics path data for the lower symbol.
         */
        lowerSymbolPath: string;
    }
    
    export interface IDeckDetails {
        name: string;
        description: string;
        previewImage: ISize & {
            url: string;
        };
        cards: (ISimpleCardItem | IPathCardItem)[];
    }

    export interface IColorItem {
        id: number;
        name: string;
    }

    function isPathCardEntity(card: dataEntities.ISimpleCardEntity | dataEntities.IPathCardEntity): card is dataEntities.IPathCardEntity {
        return typeof (<dataEntities.IPathCardEntity>card).middleSymbolPath === 'string';
    }

    function toDeckTypeListItem(item: dataEntities.IDeckTypeEntity, id: number): IDeckTypeListItem {
        return {
            id: id, name: item.name, description: item.description, previewUrl: 'assets/' + item.previewImage.fileName,
            height: item.previewImage.height, width: item.previewImage.width
        };
    }

    function toColorItem(d: dataEntities.IDeckColor, id: number): IColorItem {
        return { id: id, name: d.name };
    }

    function toParticpantCard(this: dataEntities.IDeckColor, card: dataEntities.ISimpleCardEntity | dataEntities.IPathCardEntity, index: number): ISimpleParticpantCard | IPathParticpantCard {
        var result: ISimpleParticpantCard | IPathParticpantCard;
        if (isPathCardEntity(card)) {
            result = <IPathParticpantCard> {
                id: index,
                fillColor: this.fill,
                lowerSymbolPath: card.lowerSymbolPath,
                middleSymbolPath: card.middleSymbolPath,
                strokeColor: this.stroke,
                symbolText: (typeof card.symbol === 'string') ? card.symbol : card.symbol.text,
                textColor: this.text,
                title: (typeof card.title === 'string') ? card.title : card.title.text,
                type: card.type,
                upperSymbolPath: card.upperSymbolPath,
                value: 0
            };
        } else {
            result = <ISimpleParticpantCard> {
                id: index,
                fillColor: this.fill,
                strokeColor: this.stroke,
                symbolText: (typeof card.symbol === 'string') ? card.symbol : card.symbol.text,
                textColor: this.text,
                title: (typeof card.title === 'string') ? card.title : card.title.text,
                type: card.type,
                value: 0,
                symbolFont: (typeof card.symbol === 'string') ? 'Helvetica' : card.symbol.font,
                middleSymbolTop: (typeof card.middleSymbolTop === 'number') ? typeof card.middleSymbolTop : 115,
                smallSymbolFontSize: (typeof card.smallSymbolFontSize === 'number') ? card.smallSymbolFontSize : 20,
                largeSymbolFontSize: (typeof card.largeSymbolFontSize === 'number') ? card.largeSymbolFontSize : 96,
            };
        }
        if (typeof card.title !== 'string')
            result.shortTitle = card.title.truncated;
        if (typeof card.description !== 'undefined')
        {
            result.description = card.description.text;
            result.truncatedDescription = card.description.truncatedText;
            if (typeof card.description.briefDetails === 'string')
                result.briefDetails = card.description.briefDetails;
            if (typeof card.description.fullDetails === 'string')
                result.briefDetails = card.description.fullDetails;
        }
        return result;
    }

    export class DeckTypesService {
        private _promise: angular.IPromise<void>;
        private _deckDefinitions?: dataEntities.IDeckDefinitions;
        private _selectedDeck?: dataEntities.IDeckTypeEntity;
        private _selectedColor?: dataEntities.IDeckColor;
        
        get promise(): angular.IPromise<void> {
            return this._promise;
        }
        get currentDeck(): dataEntities.IDeckTypeEntity | undefined {
            return this._selectedDeck;
        }
        get fillColor(): string {
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.fill : this._selectedColor.fill;
        }
        get strokeColor(): string {
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.stroke : this._selectedColor.stroke;
        }
        get textColor(): string {
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.text : this._selectedColor.text;
        }

        constructor($http: ng.IHttpService) {
            var svc = this;
            this._promise = $http.get<dataEntities.IDeckDefinitions>('assets/deck-definitions.json').then(function (result: ng.IHttpResponse<dataEntities.IDeckDefinitions>): void {
                svc._deckDefinitions = result.data;
            });
        }
        
        private getDeckEntity(id: number): dataEntities.IDeckTypeEntity | undefined {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(id) || id < 0 || id >= this._deckDefinitions.deckTypes.length)
                return;
            return this._deckDefinitions.deckTypes[id];
        }

        private getDeckColorEntity(colorId: number): dataEntities.IDeckColor | undefined {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(colorId) || colorId < 0 || colorId >= this._deckDefinitions.deckColors.length)
                return;
            return this._deckDefinitions.deckColors[colorId];
        }

        getAllDeckTypes(): IDeckTypeListItem[] {
            return (typeof this._deckDefinitions === 'undefined') ? [] : this._deckDefinitions.deckTypes.map(toDeckTypeListItem);
        }

        getDeckColors(): IColorItem[] {
            return (typeof this._deckDefinitions === 'undefined') ? [] : this._deckDefinitions.deckColors.map(toColorItem);
        }

        selectDeck(deckId?: number): void {
            if (typeof deckId !== 'number')
                this._selectedDeck = undefined;
            else
                this._selectedDeck = this.getDeckEntity(deckId);
        }

        selectColor(colorId?: number): void {
            if (typeof colorId !== 'number')
                this._selectedColor = undefined;
            else
                this._selectedColor = this.getDeckColorEntity(colorId);
        }

        getCards(): (ISimpleParticpantCard | IPathParticpantCard)[] {
            return (typeof this._selectedDeck === 'undefined' || typeof this._selectedColor === 'undefined') ? [] : this._selectedDeck.cards.map(toParticpantCard, this._selectedColor);
        }
    }
    
    // export interface IDeckTypesServiceResult {
    //     get promise(): angular.IPromise<void>;
    //     get currentDeck(): dataEntities.IDeckTypeEntity | undefined;
    //     get fillColor(): string;
    //     get strokeColor(): string;
    //     get textColor(): string;
    //     getAllDeckTypes(): IDeckTypeListItem[];
    //     getDeckColors(): IColorItem[];
    //     selectDeck(deckId?: number): void;
    //     selectColor(colorId?: number): void;
    //     getCards(): (ISimpleParticpantCard | IPathParticpantCard)[];
    // }

    export interface IParticpantCard extends ICardItem {
        fillColor: string;
        strokeColor: string;
        textColor: string;
    }

    export interface ISimpleParticpantCard extends IParticpantCard, ISimpleCardItem {  }

    export interface IPathParticpantCard extends IParticpantCard, IPathCardItem {  }
}