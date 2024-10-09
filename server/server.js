const express = require("express");
const bodyParser = require("body-parser");
const path = require("path");
const cors = require('cors');
require("dotenv").config();
const mongoose = require('mongoose');

const app = express();
app.use(bodyParser.json());
app.use(cors());
mongoose.connect(process.env.MONGODB);


const userSchema = new mongoose.Schema({
    name: String,
    score: Number
});

const User = mongoose.model('User', userSchema);

app.post("/", async function (req, res) {

    if (req.body.score === undefined) {
        return res.status(400).send({ error: "Invalid score data" });
    }

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
    res.status(200).send( "<h1>Welcome to the server side of Whack'Em-All !!!</h1>");
});

// Start the server
app.listen(process.env.PORT ||8000, () => {
    console.log(`Server is running on port ${process.env.PORT}.`);
  });