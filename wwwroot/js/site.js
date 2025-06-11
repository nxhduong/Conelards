const connection = new signalR.HubConnectionBuilder()
    .withUrl("/Dealer")
    .configureLogging(signalR.LogLevel.Trace)
    .withAutomaticReconnect([0, 5, 10, 30, 60, 120, 180])
    .build();

connection
    .start()
    .catch(function (err) {
        console.error(err.toString());
    });

connection.on("UpdateStatus", function (username, table) {
    let table = JSON.parse(table);

    table.Participants[username]
    
    delete table;
})

connection.on("YourTurn", function () {
    connection.invoke("Next", "");
});

function ready()
{
    connection.invoke("Ready", document.querySelector("#role").value);
    document.querySelector("#confirmButton").innerHTML = "I'm ready!";
}