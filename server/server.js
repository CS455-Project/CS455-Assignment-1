const express = require("express");
const bodyParser = require("body-parser");
const path = require("path");
const cors = require('cors');
require("dotenv").config();
const mongoose = require('mongoose');


// Use CORS middleware
const app = express();
app.use(bodyParser.json());
app.use(cors());
mongoose.connect(process.env.MONGODB);
const port = 3000;

//app.use(express.static(path.join(__dirname, "public")));

// Serve the Blazor WebAssembly app
// app.get("/", function (req, res) {
//     res.sendFile(path.join(__dirname, "public", "index.html"));
// });


const userSchema = new mongoose.Schema({
    name: String,
    score: Number
});

const User = mongoose.model('User', userSchema);
// Endpoint to receive scores
// Endpoint to receive scores
app.post("/", async function (req, res) {

    if (req.body.score === undefined) {
        return res.status(400).send({ error: "Invalid score data" });
    }

    // Store the score in the array
    const new_user = new User({
        name: req.body.name,
        score: req.body.score
    })
    const doc = await new_user.save();
    res.json(doc);
    console.log(doc);
   
});

app.get("/leaderboard", async function (req, res) {
    try {
        const leaderboard = await User.find()
            .sort({ score: -1 })
            .limit(10)
            .select('name score -_id'); // Only return name and score, exclude _id
        res.status(200).json(leaderboard);
    } catch (error) {
        console.error("Error retrieving leaderboard:", error);
        res.status(500).send({ error: "Error retrieving leaderboard" });
    }
});

// Endpoint to retrieve scores
app.get("/", function (req, res) {
    res.status(200).send( "<h1>Welcome to the server side of Whack-A-Mole !!!</h1>");
});

// Start the server
app.listen(port, function () {
    console.log(`Server running on http://localhost:${port}`);
});
