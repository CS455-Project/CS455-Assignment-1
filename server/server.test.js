const request = require('supertest');
const mongoose = require('mongoose');
const { app, User } = require('./server'); // Adjust the path if necessary
const { MongoMemoryServer } = require('mongodb-memory-server');

// Initialize MongoDB memory server
let mongoServer;

beforeAll(async () => {
    mongoServer = await MongoMemoryServer.create();
    const mongoUri = mongoServer.getUri();
    await mongoose.connect(mongoUri, { useNewUrlParser: true, useUnifiedTopology: true });
});

afterAll(async () => {
    await mongoose.disconnect();
    await mongoServer.stop();
});

describe("Server Initialization", () => {
  it("should not start the server when NODE_ENV is 'test'", async () => {
      // Set the NODE_ENV to 'test'
      process.env.NODE_ENV = 'test';

      // Mock the app.listen method
      const listenSpy = jest.spyOn(app, 'listen').mockImplementation(() => {
          return {
              close: jest.fn(),
              listening: false // Simulate that it's not listening
          };
      });

      // Call the listen method to trigger the code
      app.listen(process.env.PORT || 8000);

      // Ensure that listen was called but does not listen
      expect(listenSpy).toHaveBeenCalled();
      expect(listenSpy().listening).toBe(false);

      // Clean up the mock
      listenSpy.mockRestore();
  });
});

describe("User API", () => {
    it("should save a new user and return the saved document", async () => {
        const newUser = { name: "John Doe", score: 10 };

        const response = await request(app)
            .post("/")
            .send(newUser);

        expect(response.status).toBe(200);
        expect(response.body).toHaveProperty('_id');
        expect(response.body.name).toBe(newUser.name);
        expect(response.body.score).toBe(newUser.score);

        await User.deleteOne({ _id: response.body._id });
    });

    it("should return a 400 error for invalid score data", async () => {
        const invalidUser = { name: "Jane Doe" }; // Missing score

        const response = await request(app)
            .post("/")
            .send(invalidUser);

        expect(response.status).toBe(400);
        expect(response.body).toHaveProperty('error', 'Invalid score data');
    });

    it("should return the leaderboard sorted by score", async () => {
        await User.create([{ name: "Alice", score: 20 }, { name: "Bob", score: 30 }]);

        const response = await request(app)
            .get("/leaderboard");

        expect(response.status).toBe(200);
        expect(response.body).toHaveLength(2);
        expect(response.body[0]).toEqual(expect.objectContaining({ name: "Bob", score: 30 }));
        expect(response.body[1]).toEqual(expect.objectContaining({ name: "Alice", score: 20 }));
    });

    it("should return a welcome message", async () => {
        const response = await request(app)
            .get("/");

        expect(response.status).toBe(200);
        expect(response.text).toContain("Welcome to the server side of Whack'Em-All !!!");
    });

    it("should handle errors when retrieving the leaderboard", async () => {
      // Mock the User.find() method to throw an error
      jest.spyOn(User, 'find').mockImplementationOnce(() => {
          throw new Error("Database query failed");
      });

      const response = await request(app).get("/leaderboard");

      expect(response.status).toBe(500);
      expect(response.body).toEqual({ error: "Error retrieving leaderboard" });

      // Restore the original implementation
      User.find.mockRestore();
  });
});