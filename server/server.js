const express = require("express");
const bodyParser = require("body-parser");
const path = require("path");
const cors = require('cors');
const app = express();
// Use CORS middleware
app.use(cors());

const port = 3000;

// Middleware to parse JSON requests
app.use(bodyParser.json());
app.use(express.static(path.join(__dirname, "public")));

// Array to hold scores
let scores = [];

// Serve the Blazor WebAssembly app
app.get("/", function (req, res) {
    res.sendFile(path.join(__dirname, "public", "index.html"));
});

// Endpoint to receive scores
// Endpoint to receive scores
app.post("/api/score", function (req, res) {
    const score = req.body.score;

    if (score === undefined) {
        return res.status(400).send({ error: "Invalid score data" });
    }

    // Store the score in the array
    scores.push(score);
    console.log("Score received from Blazor:", score);

    res.status(200).send({ message: "Score received successfully", score });
});

// Endpoint to retrieve scores
app.get("/api/score", function (req, res) {
    res.status(200).send({ scores: scores });
});

// Start the server
app.listen(port, function () {
    console.log(`Server running on http://localhost:${port}`);
});
