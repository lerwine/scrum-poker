"use strict";
var deckDefinitions;
(function (deckDefinitions) {
    function isPathCardEntity(card) {
        return typeof card.middleSymbolPath === 'string';
    }
    function toDeckTypeListItem(item, id) {
        return {
            id: id, name: item.name, description: item.description, previewUrl: 'assets/' + item.previewImage.fileName,
            height: item.previewImage.height, width: item.previewImage.width
        };
    }
    function toColorItem(d, id) {
        return { id: id, name: d.name };
    }
    function toParticpantCard(symbol, index) {
        var card;
        for (var i = 0; i < this.cards.length; i++) {
            var c = this.cards[i];
            if (c.symbol == symbol) {
                card = c;
                break;
            }
        }
        if (typeof card === 'undefined')
            card = {
                symbol: symbol,
                title: "Unamatched card type '" + symbol + "'!",
                type: "Ambiguous"
            };
        var result;
        if (isPathCardEntity(card)) {
            result = {
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
        }
        else {
            result = {
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
        if (typeof card.description !== 'undefined') {
            result.description = card.description.text;
            result.truncatedDescription = card.description.truncatedText;
            if (typeof card.description.briefDetails === 'string')
                result.briefDetails = card.description.briefDetails;
            if (typeof card.description.fullDetails === 'string')
                result.briefDetails = card.description.fullDetails;
        }
        return result;
    }
    class DeckTypesService {
        get promise() {
            return this._promise;
        }
        get currentDeck() {
            return this._selectedDeck;
        }
        get fillColor() {
            // TODO: Use item from this._deckDefinitions.colorSchemas
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.fill : this._selectedColor.fill;
        }
        get strokeColor() {
            // TODO: Use item from this._deckDefinitions.colorSchemas
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.stroke : this._selectedColor.stroke;
        }
        get textColor() {
            // TODO: Use item from this._deckDefinitions.colorSchemas
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.text : this._selectedColor.text;
        }
        constructor($http) {
            var svc = this;
            this._promise = $http.get('assets/card-configuration.json').then(function (result) {
                svc._deckDefinitions = result.data;
            });
        }
        getDeckEntity(id) {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(id) || id < 0 || id >= this._deckDefinitions.deckTypes.length)
                return;
            return this._deckDefinitions.deckTypes[id];
        }
        getDeckColorEntity(schemaId, colorId) {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(schemaId) || schemaId < 0 || isNaN(colorId) || colorId < 0 || schemaId >= this._deckDefinitions.colorSchemas.length)
                return;
            var schema = this._deckDefinitions.colorSchemas[colorId];
            if (colorId >= schema.cardColors.length)
                return;
            return schema.cardColors[colorId];
        }
        getDeckColorSchema(schemaId) {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(schemaId) || schemaId < 0 || schemaId >= this._deckDefinitions.colorSchemas.length)
                return;
            return this._deckDefinitions.colorSchemas[schemaId];
        }
        getAllDeckTypes() {
            return (typeof this._deckDefinitions === 'undefined') ? [] : this._deckDefinitions.deckTypes.map(toDeckTypeListItem);
        }
        getDeckColors() {
            return (typeof this._deckDefinitions === 'undefined') ? [] : this._deckDefinitions.deckColors.map(toColorItem);
        }
        selectDeck(deckId) {
            if (typeof deckId !== 'number')
                this._selectedDeck = undefined;
            else
                this._selectedDeck = this.getDeckEntity(deckId);
        }
        selectColor(schemaid, colorId) {
            if (typeof colorId !== 'number' || typeof schemaid !== 'number')
                this._selectedColor = undefined;
            else
                this._selectedColor = this.getDeckColorEntity(schemaid, colorId);
        }
        getCards() {
            if (typeof this._deckDefinitions === 'undefined' || typeof this._selectedDeck === 'undefined' || typeof this._selectedColor === 'undefined')
                return [];
            return this._selectedDeck.cards.map(toParticpantCard, {
                color: this._selectedColor,
                cards: this._deckDefinitions.cards
            });
        }
    }
    deckDefinitions.DeckTypesService = DeckTypesService;
})(deckDefinitions || (deckDefinitions = {}));
//# sourceMappingURL=deckDefinitions.js.map