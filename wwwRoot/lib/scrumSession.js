"use strict";
var scrumSession;
(function (scrumSession) {
    let StoryState;
    (function (StoryState) {
        /**
         * Story is in the backlog.
         */
        StoryState[StoryState["Draft"] = 0] = "Draft";
        /**
         * Story has been added to the sprint.
         */
        StoryState[StoryState["Ready"] = 1] = "Ready";
        /**
         * Story has been cancelled.
         */
        StoryState[StoryState["Cancelled"] = 2] = "Cancelled";
    })(StoryState = scrumSession.StoryState || (scrumSession.StoryState = {}));
})(scrumSession || (scrumSession = {}));
//# sourceMappingURL=scrumSession.js.map