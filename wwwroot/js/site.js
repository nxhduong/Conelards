const connection = new signalR.HubConnectionBuilder()
    .withUrl("/Dealer")
    .configureLogging(signalR.LogLevel.Trace)
    .withAutomaticReconnect([0, 10, 30, 60, 90, 150])
    .build();

connection
    .start()
    .then(function () {
        connection.invoke("Init", new URLSearchParams(window.location.search).get('id'));
    })
    .catch(function (err) {
        console.error(err.toString());
    });

connection.on("YourTurn", function() {});