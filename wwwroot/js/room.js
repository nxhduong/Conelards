"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/Dealer")
    .configureLogging(signalR.LogLevel.Trace)
    .withAutomaticReconnect([0, 1, 3, 5, 10, 30, 60, 90, 120, 180, 300])
    .build();

connection
    .start()
    .catch(function (err) {
        console.error(err.toString());
    });

let table = {};
let myCards = {};
let selectedCards = {};

connection.on("UpdateState", function (table) {
    table = JSON.parse(table);
    const participantTable = document.querySelector("#participants");
    participantTable.innerHTML = null;

    // Hide role selector
    if (table.Start) {
        document.querySelector("#role").disabled = true;
        document.querySelector("#comfirm-button").hidden = true;
    }

    // Load participants
    for (const participant in { ...table.Players, ...table.Spectators }) {
        const row = document.createElement("tr");

        for (const property in participant) {
            const keyCell = document.createElement("td");
            keyCell.innerHTML = property;
            const valueCell = document.createElement("td");
            valueCell.innerHTML = participant[property];

            row.appendChild(keyCell);
            row.appendChild(Cell);
        }

        participantTable.appendChild(row);
    }

    // Load discard   
    const discardButton = document.createElement("button");

    discardButton.disabled = true;
    discardButton.style = "height: 100px; width: 50px;";
    discardButton.className = buttonColorFromCode(table.Discard.Color);

    document.querySelector("#discard").appendChild(discardButton);
})

connection.on("YourTurn", function (hand) {
    document.querySelector("#play-button").disabled = false;

    for (const [uniqueId, card] of JSON.parse(hand).entries()) {
        const cardButton = document.createElement("button");

        cardButton.style = "height: 100px; width: 50px;";
        cardButton.id = uniqueId;
        cardButton.onclick = ```chooseCard(${uniqueId})```;
        cardButton.className = buttonColorFromCode(card.Color);

        if (card.Number !== null) {
            cardButton.innerHTML = card.Number;
        }
        else {
            const cardSymbol = document.createElement("i");

            switch (card.Action) {
                case 0:
                    cardSymbol.className = "icon-search icon-ban-circle";
                    break;
                case 1:
                    cardSymbol.className = "icon-search icon-refresh";
                    break;
                case 2:
                    cardSymbol.className = "icon-search icon-tint";
                    break;
                case 3:
                    cardSymbol.className = "icon-search icon-random";
                    break;
                case 4:
                    cardSymbol.innerHTML = "+4";
                    break;
                default:
                    console.error("Invalid card action: ".concat(card.Action));
                    break;
            }

            cardButton.appendChild(cardSymbol);
        }

        myCards[uniqueId] = card;

        if (!(
            // All submitted cards match number/action with discard
            (card.Number !== null && card.Number === table.Discard.Number)
            || (card.Action !== null && card.Action === table.Discard.Action)
            // All submitted cards match number, one card matches color with discard card
            || (card.Color === table.Discard.Color)
                (card.Number === selectedCards[Object.keys(selectedCards)[0]].Number)
            && table.PenaltyStackCount === 0
            // Special cards
            || ((card.Action ?? 0) > 1 && card.Action === selectedCards[Object.keys(selectedCards)[0]].Action)
        )) {
            cardButton.disabled = true;
        }

        document.querySelector("#hand").appendChild(cardButton);
    }
    
});

function sendRoleAndReadyStatus()
{
    connection.invoke("Ready", document.querySelector("#role").value);

    document.querySelector("#confirm-button").innerHTML = "I'm ready!";
}

function buttonColorFromCode(colorCode) {
    switch (colorCode) {
        case 0:
            return "btn btn-danger";
            break;
        case 1:
            return "btn btn-success";
            break;
        case 2:
            return "btn btn-primary";
            break;
        case 3:
            return "btn btn-warning";
            break;
        case 4:
            return "btn btn-inverse";
            break;
        default:
            return "btn";
            break;
    }
}

function chooseCard(uniqueId) {
    if (!(Object.keys(selectedCards).length === 0
        || selectedCards[Object.keys(selectedCards)[0]].Number === myCards[uniqueId].Number
        || selectedCards[Object.keys(selectedCards)[0]].Action === myCards[uniqueId].Action
    )) {
        return;
    }

    if (myCards.hasOwnProperty(uniqueId)) {
        document.querySelector(```#${uniqueId}```).className = "btn btn-info";

        selectedCards[uniqueId] = myCards[uniqueId];
        delete myCards[uniqueId];
    } else {
        document.querySelector(```#${uniqueId}```).className = buttonColorFromCode(myCards[uniqueId].Color);

        myCards[uniqueId] = selectedCards[uniqueId];
        delete selectedCards[uniqueId];
    }
}

function playCards() {
    connection.invoke("Next", JSON.stringify(Object.values(selectedCards)));

    selectedCards = {};
    document.querySelector("#play-button").disabled = true;
}