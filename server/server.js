const express = require("express");
const bodyParser = require("body-parser");
const cors = require('cors');
require("dotenv").config();
const mongoose = require('mongoose');

const app = express();
app.use(bodyParser.json());
app.use(cors());

// Get port from command line or environment variables with fallback
const instanceId = process.argv[2] || '1';
const port = process.argv[3] || (8000 + parseInt(instanceId));

// MongoDB Connection Setup
const MONGODB_URI = process.env.MONGODB || 'mongodb://localhost:27017/whackemall';

if (process.env.NODE_ENV !== 'test') {
    mongoose.connect(MONGODB_URI, {
        useNewUrlParser: true,
        useUnifiedTopology: true,
        serverSelectionTimeoutMS: 5000
    })
    .then(() => {
        console.log(`MongoDB connected successfully to ${MONGODB_URI}`);
    })
    .catch((err) => {
        console.error('MongoDB connection error:', err);
        process.exit(1);
    });
}

const userSchema = new mongoose.Schema({
    name: String,
    score: Number
});

const User = mongoose.model('User', userSchema);

// Add server identification middleware
app.use((req, res, next) => {
    res.setHeader('X-Server-Instance', `Instance-${instanceId}`);
    next();
});

app.post("/", async function (req, res) {
    if (req.body.score === undefined) {
        return res.status(400).send({ error: "Invalid score data" });
    }

    try {
        const new_user = new User({
            name: req.body.name,
            score: req.body.score
        });
        const doc = await new_user.save();
        res.json(doc);
    } catch (error) {
        console.error("Error saving user:", error);
        res.status(500).send({ error: "Error saving user data" });
    }
});

app.get("/leaderboard", async function (req, res) {
    try {
        const leaderboard = await User.find()
            .sort({ score: -1 })
            .limit(10)
            .select('name score -_id'); 
        res.status(200).json(leaderboard);
    } catch (error) {
        console.error("Error retrieving leaderboard:", error);
        res.status(500).send({ error: "Error retrieving leaderboard" });
    }
});

app.get("/", function (req, res) {
    res.status(200).send(
        `<h1>Welcome to the server side of Whack'Em-All !!!</h1>
         <p>Server Instance: ${instanceId}</p>
         <p>Running on port: ${port}</p>`
    );
});

app.post("/leaderboard/delete", async function (req, res) {
    const playerName = req.query.name;

    if (!playerName) {
        return res.status(400).send({ error: "Player name is required." });
    }

    try {
        const result = await User.deleteMany({ name: playerName });
        if (result.deletedCount === 0) {
            return res.status(404).send({ error: "No entries found for the given name." });
        }
        res.status(200).send({ message: `${result.deletedCount} entries deleted.` });
    } catch (error) {
        console.error("Error deleting entries:", error);
        res.status(500).send({ error: "Error deleting entries" });
    }
});

// Check if port is available before starting the server
const checkPort = (port) => {
    return new Promise((resolve, reject) => {
        const tester = require('net').createServer()
            .once('error', err => (err.code === 'EADDRINUSE' ? resolve(false) : reject(err)))
            .once('listening', () => tester.once('close', () => resolve(true)).close())
            .listen(port);
    });
};

if (process.env.NODE_ENV !== 'test') {
    // Start server with port availability check
    (async () => {
        if (await checkPort(port)) {
            app.listen(port, () => {
                console.log(`Server Instance ${instanceId} is running on port ${port}`);
            });
        } else {
            console.error(`Port ${port} is already in use. Please use a different port.`);
            process.exit(1);
        }
    })();
}

module.exports = { app, User };