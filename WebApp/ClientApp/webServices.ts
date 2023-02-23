namespace webServices {
    export interface IUserResponseItem {
      userId: string;
      displayName?: string;
      userName: string;
    }

    interface IAppStateTeamItem {
      teamId: string;
      facilitatorId: string;
      title: string;
      description?: string;
    }

    interface IAppStateResponse extends IUserResponseItem {
      isAdmin: boolean;
      teams: IAppStateTeamItem[];
      facilitators: IUserResponseItem[];
    }

    export interface ITeamListItem {
      teamId: string;
      teamName: string;
      teamDescription?: string;
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
      colorSchemeId: string;
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
      description?: Date;
      startDate?: Date;
      plannedEndDate?: Date;
    }

    export interface IScrumState {
      plannedStartDate?: Date;
      plannedEndDate?: Date;
      initiative?: ISprintGrouping;
      epic?: ISprintGrouping;
      milestone?: ISprintGrouping;
      currentScopePoints: number;
      sprintCapacity?: number;
      team: ITeamListItem;
      facilitator: IUserResponseItem;
      participants: IParticpantListItem[];
      colorSchemes: IColorSchemeListItem[];
      // TODO: Add Deck Information
    }

    export class UserService {
      private _promise: angular.IPromise<void>;
      private _response?: IAppStateResponse;
  
      get promise(): angular.IPromise<void> { return this._promise; }

      get isAdmin(): boolean { return (<IAppStateResponse>this._response).isAdmin; }
      
      get userId(): string { return (<IAppStateResponse>this._response).userId; }
      
      get displayName(): string | undefined { return (<IAppStateResponse>this._response).displayName; }
      
      get userName(): string { return (<IAppStateResponse>this._response).userName; }
      
      getTeams(): ITeamListItem[] {
        return (<IAppStateResponse>this._response).teams.map<ITeamListItem>(
          function (this: IAppStateResponse, value: IAppStateTeamItem ): ITeamListItem {
            var id = value.facilitatorId;
            var f: IUserResponseItem | undefined = this.facilitators.find(function (this: string, u: IUserResponseItem) { return u.userId == this; }, value.facilitatorId);
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
            }
          });
      }

      // plannedStartDate?: Date;
      // plannedEndDate?: Date;
      // initiative?: ISprintGrouping;
      // epic?: ISprintGrouping;
      // milestone?: ISprintGrouping;
      // currentScopePoints: number;
      // sprintCapacity?: number;
      // team: ITeamListItem;
      // facilitator: IUserResponseItem;
      // participants: IParticpantListItem[];
      // colorSchemes: IColorSchemeListItem[];
      getScrumState(teamId: string): angular.IPromise<IScrumState> {
        return this.$http.get<IScrumStateResponse>('api/User/TeamState/' + teamId)
          .then(function (result: ng.IHttpResponse<IScrumStateResponse>): IScrumState {
            var result: IScrumState = {
              currentScopePoints: result.data.currentScopePoints
            };
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
            }
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