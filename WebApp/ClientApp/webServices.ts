namespace webServices {
    export interface IUserResponseItem {
      id: string;
      displayName: string;
      userName: string;
      isAdmin: boolean;
    }

    interface IAppStateTeamItem {
      id: string;
      facilitatorId: string;
      title: string;
      description?: string;
    }

    interface IAppStateResponse extends IUserResponseItem {
      teams: IAppStateTeamItem[];
      facilitators: IUserResponseItem[];
    }

    export interface ITeamItem {
      teamId: string;
      teamName: string;
      teamDescription?: string;
    }

    export interface ITeamListItem extends ITeamItem {
      isFacilitator: boolean;
      facilitatorName?: string;
    }

    interface ITeamStateMeetingItem {
      meetingId: string;
      title: string;
      description?: string;
      meetingDate: string;
    }

    interface ITeamStateResponse {
      teamId: string;
      title: string;
      description?: string;
      facilitator: IUserResponseItem;
      meetings: ITeamStateMeetingItem[];
    }

    export interface IMeetingListItem {
      meetingId: string;
      title: string;
      description?: string;
      meetingDate: Date;
    }

    export interface ITeamState {
      teamId: string;
      title: string;
      description?: string;
      facilitator: IUserResponseItem;
      meetings: IMeetingListItem[];
    }

    interface ISprintGroupingResponse {
      title: string;
      description?: string;
      startDate?: string;
      plannedEndDate?: string;
    }

    interface IParticpantListItem extends IUserResponseItem {
      selectedCardId?: string;
      cardColorId: string;
      assignedPoints: number;
      sprintCapacity?: number;
    }

    export interface IColorSchemeListItem {
      schemeId: string;
      name: string;
      fill: string;
      stroke: string;
      text: string;
    }

    interface IScrumStateResponse extends ITeamStateMeetingItem {
      plannedStartDate?: string;
      plannedEndDate?: string;
      initiative?: ISprintGroupingResponse;
      epic?: ISprintGroupingResponse;
      milestone?: ISprintGroupingResponse;
      currentScopePoints: number;
      sprintCapacity?: number;
      team: IAppStateTeamItem;
      facilitator: IUserResponseItem;
      participants: IParticpantListItem[];
      colorSchemes: IColorSchemeListItem[];
      // TODO: Add Deck Information
    }

    export interface ISprintGrouping {
      title: string;
      description?: string;
      startDate?: Date;
      plannedEndDate?: Date;
    }

    export interface IScrumState extends ITeamItem {
      plannedStartDate?: Date;
      plannedEndDate?: Date;
      initiative?: ISprintGrouping;
      epic?: ISprintGrouping;
      milestone?: ISprintGrouping;
      currentScopePoints: number;
      sprintCapacity?: number;
      facilitator: IUserResponseItem;
      participants: IParticpantListItem[];
      colorSchemes: IColorSchemeListItem[];
      // TODO: Add Deck Information
    }

    function fromDateString(value?: string) : Date | undefined {
      if (typeof value !== 'string')
        return;
      return new Date(value);
    }

    function fromSprintGroupingResponse(value?: ISprintGroupingResponse): ISprintGrouping | undefined {
      if (typeof value === 'undefined')
        return;
      return {
        title: value.title,
        description: value.description,
        startDate: fromDateString(value.startDate),
        plannedEndDate: fromDateString(value.plannedEndDate)
      };
    }
    export class UserService {
      private _promise: angular.IPromise<void>;
      private _response?: IAppStateResponse;
  
      get promise(): angular.IPromise<void> { return this._promise; }

      get isAdmin(): boolean { return (<IAppStateResponse>this._response).isAdmin; }
      
      get userId(): string { return (<IAppStateResponse>this._response).id; }
      
      get displayName(): string | undefined { return (<IAppStateResponse>this._response).displayName; }
      
      get userName(): string { return (<IAppStateResponse>this._response).userName; }
      
      getTeams(): ITeamListItem[] {
        return (<IAppStateResponse>this._response).teams.map<ITeamListItem>(
          function (this: IAppStateResponse, value: IAppStateTeamItem ): ITeamListItem {
            var id = value.facilitatorId;
            var f: IUserResponseItem | undefined = this.facilitators.find(function (this: string, u: IUserResponseItem) { return u.id == this; }, value.facilitatorId);
            if (typeof f === 'undefined')
              f = {
                id: value.facilitatorId,
                userName: '(id:' + value.facilitatorId + ')',
                displayName: value.facilitatorId,
                isAdmin: false
              };
            if (id == this.id)
              return {
                isFacilitator: true,
                teamId: value.id,
                teamName: value.title,
                teamDescription: value.description,
              };
            return {
              isFacilitator: false,
              teamId: value.id,
              teamName: value.title,
              teamDescription: value.description,
              facilitatorName:
                typeof this.displayName === 'string'
                  ? this.displayName
                  : this.userName,
            };
          },
          <IAppStateResponse>this._response
        );
      }

      getTeamState(teamId: string): angular.IPromise<ITeamState> {
        return this.$http.get<ITeamStateResponse>('api/User/TeamState/' + teamId)
          .then(function (result: ng.IHttpResponse<ITeamStateResponse>): ITeamState {
            return {
              teamId: result.data.teamId,
              title: result.data.title,
              description: result.data.description,
              facilitator: result.data.facilitator,
              meetings: result.data.meetings.map<IMeetingListItem>(function(value: ITeamStateMeetingItem): IMeetingListItem {
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

      getScrumState(teamId: string): angular.IPromise<IScrumState> {
        return this.$http.get<IScrumStateResponse>('api/User/ScrumMeeting/' + teamId)
          .then(function (result: ng.IHttpResponse<IScrumStateResponse>): IScrumState {
            var scrumState: IScrumState = {
              initiative: fromSprintGroupingResponse(result.data.initiative),
              epic: fromSprintGroupingResponse(result.data.epic),
              milestone: fromSprintGroupingResponse(result.data.milestone),
              currentScopePoints: result.data.currentScopePoints,
              sprintCapacity: result.data.sprintCapacity,
              teamId: result.data.team.id,
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

      constructor(private $http: ng.IHttpService) {
        var svc = this;
        this._promise = $http
          .get<IAppStateResponse>('api/User/AppState')
          .then(function (result: ng.IHttpResponse<IAppStateResponse>): void {
            svc._response = result.data;
          });
      }
    }
  }