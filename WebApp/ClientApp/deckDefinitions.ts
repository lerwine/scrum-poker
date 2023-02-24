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

    interface IToParticpantCardContext {
        color: dataEntities.IDeckColor;
        cards: (dataEntities.ISimpleCardEntity | dataEntities.IPathCardEntity)[];
    }
    function toParticpantCard(this: IToParticpantCardContext, symbol: string, index: number): ISimpleParticpantCard | IPathParticpantCard {
        var card: dataEntities.ISimpleCardEntity | dataEntities.IPathCardEntity | undefined;
        for (var i = 0; i < this.cards.length; i++) {
            var c: dataEntities.ISimpleCardEntity | dataEntities.IPathCardEntity = this.cards[i];
            if (c.symbol == symbol) {
                card = c;
                break;
            }
        }
        if (typeof card === 'undefined')
            card = <dataEntities.ISimpleCardEntity>{
                symbol: symbol,
                title: "Unamatched card type '" + symbol + "'!",
                type: "Ambiguous"
            };
        var result: ISimpleParticpantCard | IPathParticpantCard;
        if (isPathCardEntity(card)) {
            result = <IPathParticpantCard> {
                id: index,
                fillColor: this.color.fill,
                lowerSymbolPath: card.lowerSymbolPath,
                middleSymbolPath: card.middleSymbolPath,
                strokeColor: this.color.stroke,
                symbolText: (typeof card.symbol === 'string') ? card.symbol : card.symbol.text,
                textColor: this.color.text,
                title: (typeof card.title === 'string') ? card.title : card.title.text,
                type: card.type,
                upperSymbolPath: card.upperSymbolPath,
                value: 0
            };
        } else {
            result = <ISimpleParticpantCard> {
                id: index,
                fillColor: this.color.fill,
                strokeColor: this.color.stroke,
                symbolText: (typeof card.symbol === 'string') ? card.symbol : card.symbol.text,
                textColor: this.color.text,
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
            // TODO: Use item from this._deckDefinitions.colorSchemas
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.fill : this._selectedColor.fill;
        }
        get strokeColor(): string {
            // TODO: Use item from this._deckDefinitions.colorSchemas
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.stroke : this._selectedColor.stroke;
        }
        get textColor(): string {
            // TODO: Use item from this._deckDefinitions.colorSchemas
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.text : this._selectedColor.text;
        }

        constructor($http: ng.IHttpService) {
            var svc = this;
            this._promise = $http.get<dataEntities.IDeckDefinitions>('assets/card-configuration.json').then(function (result: ng.IHttpResponse<dataEntities.IDeckDefinitions>): void {
                svc._deckDefinitions = result.data;
            });
        }
        
        private getDeckEntity(id: number): dataEntities.IDeckTypeEntity | undefined {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(id) || id < 0 || id >= this._deckDefinitions.deckTypes.length)
                return;
            return this._deckDefinitions.deckTypes[id];
        }

        private getDeckColorEntity(schemaId: number, colorId: number): dataEntities.IDeckColor | undefined {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(schemaId) || schemaId < 0 || isNaN(colorId) || colorId < 0 || schemaId >= this._deckDefinitions.colorSchemas.length)
                return;
            var schema = this._deckDefinitions.colorSchemas[colorId];
            if (colorId >= schema.cardColors.length)
                return;
            return schema.cardColors[colorId];
        }

        private getDeckColorSchema(schemaId: number): dataEntities.IColorSchema | undefined {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(schemaId) || schemaId < 0 || schemaId >= this._deckDefinitions.colorSchemas.length)
                return;
            return this._deckDefinitions.colorSchemas[schemaId];
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

        selectColor(schemaid?: number, colorId?: number): void {
            if (typeof colorId !== 'number' || typeof schemaid !== 'number')
                this._selectedColor = undefined;
            else
                this._selectedColor = this.getDeckColorEntity(schemaid, colorId);
        }

        getCards(): (ISimpleParticpantCard | IPathParticpantCard)[] {
            if (typeof this._deckDefinitions === 'undefined' || typeof this._selectedDeck === 'undefined' || typeof this._selectedColor === 'undefined')
                return [];
            return this._selectedDeck.cards.map(toParticpantCard, {
                color: this._selectedColor,
                cards: this._deckDefinitions.cards
            });
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