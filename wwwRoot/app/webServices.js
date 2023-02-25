"use strict";
var webServices;
(function (webServices) {
    function fromDateString(value) {
        if (typeof value !== 'string')
            return;
        return new Date(value);
    }
    function fromSprintGroupingResponse(value) {
        if (typeof value === 'undefined')
            return;
        return {
            title: value.title,
            description: value.description,
            startDate: fromDateString(value.startDate),
            plannedEndDate: fromDateString(value.plannedEndDate)
        };
    }
    class UserService {
        get promise() { return this._promise; }
        get isAdmin() { return this._response.isAdmin; }
        get userId() { return this._response.userId; }
        get displayName() { return this._response.displayName; }
        get userName() { return this._response.userName; }
        getTeams() {
            return this._response.teams.map(function (value) {
                var id = value.facilitatorId;
                var f = this.facilitators.find(function (u) { return u.userId == this; }, value.facilitatorId);
                if (typeof f === 'undefined')
                    f = {
                        userId: value.facilitatorId,
                        userName: '(id:' + value.facilitatorId + ')',
                    };
                if (id == this.userId)
                    return {
                        isFacilitator: true,
                        teamId: value.teamId,
                        teamName: value.title,
                        teamDescription: value.description,
                    };
                return {
                    isFacilitator: false,
                    teamId: value.teamId,
                    teamName: value.title,
                    teamDescription: value.description,
                    facilitatorName: typeof this.displayName === 'string'
                        ? this.displayName
                        : this.userName,
                };
            }, this._response);
        }
        getTeamState(teamId) {
            return this.$http.get('api/User/TeamState/' + teamId)
                .then(function (result) {
                return {
                    teamId: result.data.teamId,
                    title: result.data.title,
                    description: result.data.description,
                    facilitator: result.data.facilitator,
                    meetings: result.data.meetings.map(function (value) {
                        return {
                            meetingId: value.meetingId,
                            title: value.title,
                            description: value.description,
                            meetingDate: new Date(value.meetingDate)
                        };
                    })
                };
            });
        }
        getScrumState(teamId) {
            return this.$http.get('api/User/ScrumMeeting/' + teamId)
                .then(function (result) {
                var scrumState = {
                    initiative: fromSprintGroupingResponse(result.data.initiative),
                    epic: fromSprintGroupingResponse(result.data.epic),
                    milestone: fromSprintGroupingResponse(result.data.milestone),
                    currentScopePoints: result.data.currentScopePoints,
                    sprintCapacity: result.data.sprintCapacity,
                    teamId: result.data.team.teamId,
                    teamName: result.data.team.title,
                    teamDescription: result.data.team.description,
                    facilitator: result.data.facilitator,
                    participants: result.data.participants,
                    colorSchemes: result.data.colorSchemes
                };
                if (typeof result.data.plannedStartDate === 'string')
                    scrumState.plannedStartDate = new Date(result.data.plannedStartDate);
                if (typeof result.data.plannedEndDate === 'string')
                    scrumState.plannedEndDate = new Date(result.data.plannedEndDate);
                return scrumState;
            });
        }
        constructor($http) {
            this.$http = $http;
            var svc = this;
            this._promise = $http
                .get('api/User/AppState')
                .then(function (result) {
                svc._response = result.data;
            });
        }
    }
    webServices.UserService = UserService;
})(webServices || (webServices = {}));
//# sourceMappingURL=webServices.js.map