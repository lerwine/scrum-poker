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
    function toParticpantCard(card, index) {
        var result;
        if (isPathCardEntity(card)) {
            result = {
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
        }
        else {
            result = {
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
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.fill : this._selectedColor.fill;
        }
        get strokeColor() {
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.stroke : this._selectedColor.stroke;
        }
        get textColor() {
            return (typeof this._deckDefinitions === 'undefined') ? "" : (typeof this._selectedColor !== 'object') ? this._deckDefinitions.votingCard.text : this._selectedColor.text;
        }
        constructor($http) {
            var svc = this;
            this._promise = $http.get('assets/deck-definitions.json').then(function (result) {
                svc._deckDefinitions = result.data;
            });
        }
        getDeckEntity(id) {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(id) || id < 0 || id >= this._deckDefinitions.deckTypes.length)
                return;
            return this._deckDefinitions.deckTypes[id];
        }
        getDeckColorEntity(colorId) {
            if (typeof this._deckDefinitions === 'undefined' || isNaN(colorId) || colorId < 0 || colorId >= this._deckDefinitions.deckColors.length)
                return;
            return this._deckDefinitions.deckColors[colorId];
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
        selectColor(colorId) {
            if (typeof colorId !== 'number')
                this._selectedColor = undefined;
            else
                this._selectedColor = this.getDeckColorEntity(colorId);
        }
        getCards() {
            return (typeof this._selectedDeck === 'undefined' || typeof this._selectedColor === 'undefined') ? [] : this._selectedDeck.cards.map(toParticpantCard, this._selectedColor);
        }
    }
    deckDefinitions.DeckTypesService = DeckTypesService;
})(deckDefinitions || (deckDefinitions = {}));
//# sourceMappingURL=deckDefinitions.js.map