var scrumPokerApp = angular.module('scrumPokerApp', []);
scrumPokerApp.controller('sessionController', ['$scope', '$http', function($scope, $http) {
  $scope.title = 'My Planning Session';
  var CARDTYPE_POINTS = 0;
  var CARDTYPE_Q = 1;
  var CARDTYPE_IMPOSSIBLE = 2;
  var CARDTYPE_ABSTAIN = 3;
  var votingCard = { url: "assets/Card-Voting.svg", points: 0, description: "Vote in progress", type: CARDTYPE_Q };
  var votedCard = { url: "assets/Card-Voted-Green.svg", points: 0, description: "Vote in progress", type: CARDTYPE_Q };
  $scope.cardDeck = [
    { url: "assets/Card-Q-Green.svg", points: 0, description: "Further clarification is required to make an informed decision.", type: CARDTYPE_Q },
    { url: "assets/Card-0-Green.svg", points: 0, description: "Requirements for user story have already been fulfilled.", type: CARDTYPE_POINTS },
    { url: "assets/Card-Half-Green.svg", points: 0.5, description: "Half Story Point.", type: CARDTYPE_POINTS },
    { url: "assets/Card-1-Green.svg", points: 1, description: "1 Story Point.", type: CARDTYPE_POINTS },
    { url: "assets/Card-2-Green.svg", points: 2, description: "2 Story Points.", type: CARDTYPE_POINTS },
    { url: "assets/Card-3-Green.svg", points: 3, description: "3 Story Points.", type: CARDTYPE_POINTS },
    { url: "assets/Card-5-Green.svg", points: 5, description: "5 Story Points.", type: CARDTYPE_POINTS },
    { url: "assets/Card-8-Green.svg", points: 8, description: "8 Story Points.", type: CARDTYPE_POINTS },
    { url: "assets/Card-13-Green.svg", points: 13, description: "13 Story Points.", type: CARDTYPE_POINTS },
    { url: "assets/Card-20-Green.svg", points: 20, description: "20 Story Points.", type: CARDTYPE_POINTS },
    { url: "assets/Card-40-Green.svg", points: 40, description: "40 Story Points.", type: CARDTYPE_POINTS },
    { url: "assets/Card-100-Green.svg", points: 100, description: "100 Story Points.", type: CARDTYPE_POINTS },
    { url: "assets/Card-Abstain-Green.svg", points: 0, description: "Abstaining from vote.", type: CARDTYPE_IMPOSSIBLE },
    { url: "assets/Card-Infinity-Green.svg", points: 0, description: "User story cannot be completed.", type: CARDTYPE_ABSTAIN }
  ];
  $scope.presenterName = 'Mr. Martin';
  $scope.currentParticipant = { name: "Paul", selectedCard: votingCard, explanation: '', selectionFinalized: false, assignedPoints: 0 };
  $scope.allParticipants = [
    { name: "John", selectedCard: votingCard, explanation: '', selectionFinalized: false, assignedPoints: 0 },
    $scope.currentParticipant,
    { name: "Ringo", selectedCard: votedCard, explanation: '', selectionFinalized: true, assignedPoints: 0 },
    { name: "George", selectedCard: votedCard, explanation: '', selectionFinalized: true, assignedPoints: 0 }
  ];
  $scope.userStories = [
    { id: 1, title: "Story #1", description: "First user story description", notes: "", result: $scope.cardDeck[7], assignedTo: $scope.allParticipants[2] },
    { id: 2, title: "Story #2", description: "Second user story description", notes: "" },
    { id: 3, title: "Story #3", description: "Third user story description", notes: "" }
  ];
  $scope.sprintPoints = 120;
  $scope.plannedPoints = $scope.userStories.reduce(function(previousvalue, currentValue) {
    return (typeof currentValue.result == 'object') ? previousvalue + currentValue.result.points : previousvalue;
  }, 0);
  $scope.userStories.forEach(function(c) {
    if (typeof c.assignedTo !== 'undefined' && typeof c.result !== 'undefined')
        c.assignedTo.assignedPoints += c.result.points;
  });
  $scope.remainingPoints = $scope.sprintPoints - $scope.plannedPoints;
  $scope.currentStory = $scope.userStories[1];
  $scope.instructions = 'Select card representing your estimate.';
  $scope.explanation = '';
  $scope.pickCard = function(card) {
    if (!$scope.currentParticipant.selectionFinalized)
        $scope.selectedCard = card;
  };
  $scope.finalizeVote = function(card) {
    $scope.currentParticipant.selectionFinalized = true;
    $scope.currentParticipant.selectedCard = votedCard;
  };
}]);
