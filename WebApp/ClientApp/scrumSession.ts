namespace scrumSession {
    export interface ITitleAndIdentifier {
        title: string;
        identifer: string;
    }

    export enum StoryState {
        /**
         * Story is in the backlog.
         */
        Draft = 0,
        /**
         * Story has been added to the sprint.
         */
        Ready = 1,

        /**
         * Story has been cancelled.
         */
        Cancelled = 2
    }

    /**
     * Common interface for initiatives, epics, milestones, 
     */
    export interface ISprintGrouping extends ITitleAndIdentifier {
        description?: string;
        startDate?: string;
        plannedEndDate?: string;
    }

    /**
     * Represents the sprint planning session.
     */
    export interface ISessionEntity extends ITitleAndIdentifier {
        description: string;
        initiative?: ISprintGrouping;
        epic?: ISprintGrouping;
        milestone?: ISprintGrouping;
        plannedStartDate?: string;
        plannedEndDate?: string;
        currentScopePoints: number;
        sprintCapacity?: number;
        deckId: number;
        projects: ISprintGrouping[];
        themes: ISprintGrouping[];
        stories: IUserStoryEntity[];
    }

    /**
     * Represents a user story entity record.
     */
    export interface IUserStoryEntity extends ITitleAndIdentifier {
        /**
         * Detailed description.
         */
        description: string;

        /**
         * Story acceptance criteria.
         */
        acceptanceCriteria: string;

        /**
         * The index (unique identifier) of the associated project in the parent sprint planning session or undefined if it is not associated with any project.
         */
        projectId?: number;

        /**
         * The index (unique identifier) of the associated theme in the parent sprint planning session or undefined if it is not associated with any project.
         */
        themeId?: number;
        created: string;
        points?: number;
        state: StoryState;
        assignedTo?: number;
        order: number;
        preRequisiteIds: number[];
    }

    export interface IDeveloperEntity {
        id: number;
        displayName: string;
        sprintCapacity?: number;
        assignedPoints: number;
        isParticipant: boolean;
        sprintId: number;
    }

    export interface INonParticipantEntity extends IDeveloperEntity {
        isParticipant: false;
    }

    export interface IParticipantEntity extends IDeveloperEntity {
        userName: string;
        colorId: number;
        isParticipant: true;
    }
}